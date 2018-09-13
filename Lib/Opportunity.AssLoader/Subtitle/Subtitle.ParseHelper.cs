using Opportunity.AssLoader.Collections;
using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Non-generic interface of <see cref="ParseResult{TScriptInfo}"/>.
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        /// The <see cref="Subtitle"/> presents the ass file.
        /// </summary>
        Subtitle Result { get; }

        /// <summary>
        /// Excetions during parsing.
        /// </summary>
        IReadOnlyList<Exception> Exceptions { get; }
    }

    /// <summary>
    /// Represent result of <see cref="Subtitle.Parse"/>.
    /// </summary>
    /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
    public sealed class ParseResult<TScriptInfo> : IDeserializeInfo, IParseResult
        where TScriptInfo : ScriptInfoCollection
    {
        void IDeserializeInfo.AddException(Exception ex)
        {
            if (Freezed)
                throw new InvalidOperationException();
            ex.Data["Line Number"] = this.LineNumber;
            ((IList<Exception>)Exceptions).Add(ex);
        }

        internal void Freeeze(Subtitle<TScriptInfo> result)
        {
            Result = result;
            this.LineNumber = -1;
            Exceptions = new ReadOnlyCollection<Exception>((IList<Exception>)Exceptions);
        }

        internal bool Freezed => Result != null;

        internal int LineNumber;

        /// <summary>
        /// The <see cref="Subtitle{TScriptInfo}"/> presents the ass file.
        /// </summary>
        public Subtitle<TScriptInfo> Result { get; private set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Subtitle IParseResult.Result => Result;

        /// <summary>
        /// Excetions during parsing.
        /// </summary>
        public IReadOnlyList<Exception> Exceptions { get; private set; } = new List<Exception>();
    }

    public partial class Subtitle
    {
        private ref struct ParseHelper<T>
            where T : ScriptInfoCollection
        {
            public ParseHelper(string data, Func<T> factory) : this()
            {
                this.remainData = data.AsSpan();
                this.subtitle = new Subtitle<T>(factory());
                this.result = new ParseResult<T>();
            }

            public ParseResult<T> GetResult()
            {
                this.result.LineNumber = 1;
                var line = default(ReadOnlySpan<char>);
                while (!this.remainData.IsEmpty)
                {
                    var brindex = this.remainData.IndexOfAny('\r', '\n');
                    if (brindex < 0)
                    {
                        line = this.remainData;
                        this.remainData = default;
                    }
                    else
                    {
                        line = this.remainData.Slice(0, brindex);
                        if (this.remainData.Length > brindex + 1 && this.remainData[brindex] == '\r' && this.remainData[brindex + 1] == '\n')
                            brindex += 2;
                        else
                            brindex += 1;
                        this.remainData = this.remainData.Slice(brindex);
                    }

                    this.currentData = line.Trim();
                    this.handleCurrentLine();
                    this.result.LineNumber++;
                }
                endFile();
                this.result.Freeeze(this.subtitle);
                return this.result;
            }

            private void handleCurrentLine()
            {
                // Skip empty lines
                if (this.currentData.IsEmpty)
                    return;

                if (this.currentData[0] == '[' && this.currentData[this.currentData.Length - 1] == ']') // Section header
                {
                    endFile();

                    var secHeader = this.currentData.Slice(1, this.currentData.Length - 2).Trim();

                    if (secHeader.Equals("Script Info".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("ScriptInfo".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.ScriptInfo;
                    else if (secHeader.Equals("V4+ Styles".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Styles;
                    else if (false
                        || secHeader.Equals("V4 styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("V4styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("V4 styles+".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("V4+styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("V4styles+".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        this.section = Section.Styles;
                        ((IDeserializeInfo)this.result).AddException(
                            new InvalidOperationException($"Unknown section [{secHeader.ToString()}] found, assume [V4+ Styles]."));
                    }
                    else if (secHeader.Equals("Events".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Events;
                    else if (secHeader.Equals("Fonts".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Fonts;
                    else if (secHeader.Equals("Graphics".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Graphics;
                    else
                    {
                        this.section = Section.Unknown;
                        ((IDeserializeInfo)this.result).AddException(
                            new InvalidOperationException($"Unknown section [{secHeader.ToString()}] found."));
                    }
                }
                else // Section content
                {
                    switch (this.section)
                    {
                    case Section.FileHeader:
                        ((IDeserializeInfo)this.result).AddException(new InvalidOperationException("Content found without a section header, assume [Script Info]."));
                        goto case Section.ScriptInfo;
                    case Section.ScriptInfo:
                        this.initScriptInfo();
                        break;
                    case Section.Styles:
                        this.initStyle();
                        break;
                    case Section.Events:
                        this.initEvent();
                        break;
                    case Section.Graphics:
                    case Section.Fonts:
                        this.initFile();
                        break;
                    }
                }
            }

            private enum Section
            {
                Unknown = -1,
                FileHeader = 0,
                ScriptInfo,
                Styles,
                Events,
                Graphics,
                Fonts,
            }

            private Section section;

            private ReadOnlySpan<char> remainData;

            private ReadOnlySpan<char> currentData;

            private readonly Subtitle<T> subtitle;

            private readonly ParseResult<T> result;

            private FieldSerializeHelper[] styleFormat, eventFormat;

            private void initFormat(ref FieldSerializeHelper[] formatStore, FieldSerializeHelper[] defaultValue, ReadOnlySpan<char> value, Dictionary<string, FieldSerializeHelper> infoDic)
            {
                IDeserializeInfo sinfo = this.result;
                try
                {
                    formatStore = EntryHeader.Parse(value).Select(h =>
                    {
                        if (!infoDic.TryGetValue(h, out var v))
                            sinfo.AddException(new FormatException($"Unknown field `{h}`"));
                        return v;
                    }).ToArray();
                }
                catch (Exception ex)
                {
                    formatStore = defaultValue;
                    sinfo.AddException(new FormatException("Failed to parse format line, use default format, chack InnerException for more info.", ex));
                }
            }

            private void initScriptInfo()
            {
                if (this.currentData[0] == ';')
                    return;
                this.subtitle.ScriptInfo.ParseLine(this.currentData, this.result);
            }

            private void initStyle()
            {
                if (!FormatHelper.TryPraseLine(out var key, out var value, this.currentData))
                {
                    ((IDeserializeInfo)this.result).AddException(new FormatException("Wrong line format."));
                    return;
                }

                if (key.Equals("format".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    initFormat(ref this.styleFormat, DefaultStyleDef, value, Style.FieldInfo);
                }
                else if (key.Equals("style".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    if (this.styleFormat is null)
                    {
                        this.styleFormat = DefaultStyleDef;
                        ((IDeserializeInfo)this.result).AddException(new FormatException("Format defination for section [V4+ Styles] not found."));
                    }

                    var s = new Style();
                    s.Parse(value, this.styleFormat, this.result);
                    if (this.subtitle.Styles.ContainsName(s.Name))
                        ((IDeserializeInfo)this.result).AddException(
                            new ArgumentException($"Style with the name \"{s.Name}\" is already in the StyleSet."));
                    this.subtitle.Styles.Add(s);
                }
            }

            private void initEvent()
            {
                if (!FormatHelper.TryPraseLine(out var key, out var value, this.currentData))
                {
                    ((IDeserializeInfo)this.result).AddException(new FormatException("Wrong line format."));
                    return;
                }

                if (key.Equals("format".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    initFormat(ref this.eventFormat, DefaultEventDef, value, SubEvent.FieldInfo);
                }
                else
                {
                    if (this.eventFormat is null)
                    {
                        this.eventFormat = DefaultEventDef;
                        ((IDeserializeInfo)this.result).AddException(new FormatException("Format defination for section [Event] not found."));
                    }

                    var isCom = key.Equals("Comment".AsSpan(), StringComparison.OrdinalIgnoreCase);
                    if (!isCom && !key.Equals("Dialogue".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        ((IDeserializeInfo)this.result).AddException(
                            new FormatException($"Unsupported event type \"{key.ToString()}\", assuming as \"Dialogue\"."));
                    }
                    var sub = new SubEvent { IsComment = isCom };
                    sub.Parse(value, this.eventFormat, this.result);
                    this.subtitle.Events.Add(sub);
                }
            }

            private EmbeddedFile currentFile;
            private UUDecoder currentFileData;

            private void endFile()
            {
                if (this.currentFile != null)
                {
                    this.currentFile.Data = this.currentFileData.ToArray();
                    this.currentFile = null;
                    this.currentFileData = null;
                }
            }

            private void initFile()
            {
                IDeserializeInfo di = this.result;
                if (this.currentData[0] <= '`' && this.currentData[0] > ' ')
                {
                    // data line
                    if (this.currentFile is null)
                    {
                        di.AddException(new FormatException("Wrong line format."));
                        return;
                    }
                    this.currentFileData.ReadLine(this.currentData);
                    return;
                }

                if (!FormatHelper.TryPraseLine(out var key, out var value, this.currentData))
                {
                    di.AddException(new FormatException("Wrong line format."));
                    return;
                }
                endFile();

                this.currentFileData = new UUDecoder();
                if (this.section == Section.Fonts)
                {
                    var file = new EmbeddedFont(toValidName(value));
                    this.currentFile = file;
                    this.subtitle.Fonts.Add(file);
                    if (!key.Equals("fontname".AsSpan(), StringComparison.Ordinal))
                        di.AddException(new FormatException($"Wrong file header `{key.ToString()}` in [Fonts]."));
                }
                else // Section.Graphics
                {
                    var file = new EmbeddedGraphic(toValidName(value));
                    this.currentFile = file;
                    this.subtitle.Graphics.Add(file);
                    if (!key.Equals("filename".AsSpan(), StringComparison.Ordinal))
                        di.AddException(new FormatException($"Wrong file header `{key.ToString()}` in [Graphics]."));
                }

                string toValidName(ReadOnlySpan<char> filename)
                {
                    if (filename.IsEmpty)
                    {
                        di.AddException(new FormatException($"No file name, use `no name`."));
                        return "no name";
                    }
                    var invch = Path.GetInvalidFileNameChars().AsSpan();
                    Span<char> fn = stackalloc char[filename.Length];
                    filename.CopyTo(fn);
                    var invid = -1;
                    var setex = false;
                    while ((invid = fn.IndexOfAny(invch)) >= 0)
                    {
                        fn[invid] = '_';
                        if (!setex)
                        {
                            di.AddException(new FormatException($"Invalid file name `{filename.ToString()}`."));
                            setex = true;
                        }
                    }
                    return fn.ToString();
                }
            }
        }
    }
}
