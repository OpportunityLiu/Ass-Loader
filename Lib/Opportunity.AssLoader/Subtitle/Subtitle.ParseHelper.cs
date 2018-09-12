using Opportunity.AssLoader.Collections;
using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                this.result.Freeeze(this.subtitle);
                return this.result;
            }

            private void handleCurrentLine()
            {
                // Skip empty lines and comment lines.
                if (this.currentData.IsEmpty || this.currentData[0] == ';')
                    return;

                if (this.currentData[0] == '[' && this.currentData[this.currentData.Length - 1] == ']') // Section header
                {
                    var secHeader = this.currentData.Slice(1, this.currentData.Length - 2).Trim();

                    if (secHeader.Equals("script info".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("scriptinfo".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.ScriptInfo;
                    else if (secHeader.Equals("v4+ styles".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Styles;
                    else if (false
                        || secHeader.Equals("v4 styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("v4styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("v4 styles+".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("v4+styles".AsSpan(), StringComparison.OrdinalIgnoreCase)
                        || secHeader.Equals("v4styles+".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        this.section = Section.Styles;
                        ((IDeserializeInfo)this.result).AddException(
                            new InvalidOperationException($"Unknown section [{secHeader.ToString()}] found, assume [V4+ Styles]."));
                    }
                    else if (secHeader.Equals("events".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        this.section = Section.Events;
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
                        ((IDeserializeInfo)this.result).AddException(new InvalidOperationException("Content found without a section header."));
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
                    if (this.subtitle.StyleSet.ContainsName(s.Name))
                        ((IDeserializeInfo)this.result).AddException(
                            new ArgumentException($"Style with the name \"{s.Name}\" is already in the StyleSet."));
                    this.subtitle.StyleSet.Add(s);
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
                    this.subtitle.EventCollection.Add(sub);
                }
            }
        }
    }
}
