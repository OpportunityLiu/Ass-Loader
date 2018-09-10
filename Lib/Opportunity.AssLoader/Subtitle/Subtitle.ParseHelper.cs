using Opportunity.AssLoader.Collections;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

namespace Opportunity.AssLoader
{
    public static partial class Subtitle
    {
        private class ParseHelper<T> where T : ScriptInfoCollection
        {
            public ParseHelper(TextReader reader, bool isExact, Func<T> factory)
            {
                this.reader = reader;
                this.isExact = isExact;
                this.subtitle = new Subtitle<T>(factory());
            }

            public Subtitle<T> GetResult()
            {
                var lineNumber = 0;
                ReadOnlySpan<char> line = null;
                var sec = this.isExact ? Section.Unknown : Section.ScriptInfo;
                try
                {
                    while (true)
                    {
                        var strline = this.reader.ReadLine();
                        if (strline is null)
                            return this.subtitle;

                        line = strline.AsSpan();
                        lineNumber++;

                        var temp = line.Trim();
                        // Skip empty lines and comment lines.
                        if (temp.Length == 0 || temp[0] == ';')
                            continue;

                        if (temp[0] == '[' && temp[temp.Length - 1] == ']') // Section header
                        {
                            var secHeader = temp.ToString().ToLowerInvariant();
                            switch (secHeader)
                            {
                            case "[script info]":
                            case "[scriptinfo]":
                                sec = Section.ScriptInfo;
                                break;
                            case "[v4+ styles]":
                            case "[v4 styles+]":
                            case "[v4+styles]":
                            case "[v4styles+]":
                                sec = Section.Styles;
                                break;
                            case "[events]":
                                sec = Section.Events;
                                break;
                            default:
                                sec = Section.Unknown;
                                var secStr = temp.Slice(1, temp.Length - 2);
                                if (this.isExact)
                                    throw new InvalidOperationException($"Unknown section \"{secStr.ToString()}\" found.");
                                break;
                            }
                        }
                        else // Section content
                        {
                            switch (sec)
                            {
                            case Section.ScriptInfo:
                                this.initScriptInfo(temp);
                                break;
                            case Section.Styles:
                                this.initStyle(temp);
                                break;
                            case Section.Events:
                                this.initEvent(temp);
                                break;
                            default:
                                if (this.isExact)
                                    throw new InvalidOperationException("Content found without a section header.");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var sline = line.ToString();
                    var exception = new ArgumentException($@"Error occurs during parsing.
Line number: {lineNumber}
Content of the line:
{sline}", ex);
                    exception.Data.Add("Line number", lineNumber);
                    exception.Data.Add("Line content", sline);
                    exception.Data.Add("Current section", sec.ToString());
                    throw exception;
                }
            }

            public Task<Subtitle<T>> GetResultAsync()
            {
                return Task.Run(() => this.GetResult());
            }

            private enum Section
            {
                Unknown = 0,
                ScriptInfo,
                Styles,
                Events
            }

            private readonly TextReader reader;

            private readonly Subtitle<T> subtitle;

            private readonly bool isExact;

            private FieldSerializeHelper[] styleFormat, eventFormat;

            private void initScriptInfo(ReadOnlySpan<char> scriptInfoLine)
            {
                if (this.isExact)
                    this.subtitle.ScriptInfo.ParseLineExact(scriptInfoLine);
                else
                    this.subtitle.ScriptInfo.ParseLine(scriptInfoLine);
            }

            private void initStyle(ReadOnlySpan<char> styleLine)
            {
                if (FormatHelper.TryPraseLine(out var key, out var value, styleLine))
                {
                    if (key.Equals("format".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        this.styleFormat = EntryParser.ParseHeader(value).Select(h => Style.FieldInfo[h]).ToArray();
                    }
                    else if (key.Equals("style".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        if (this.styleFormat == null)
                            this.styleFormat = DefaultStyleDef;
                        var s = new Style();
                        if (this.isExact)
                        {
                            s.ParseExact(value, this.styleFormat);
                            if (this.subtitle.StyleSet.ContainsName(s.Name))
                                throw new ArgumentException($"Style with the name \"{s.Name}\" is already in the StyleSet.");
                        }
                        else
                        {
                            s.Parse(value, this.styleFormat);
                        }
                        this.subtitle.StyleSet.Add(s);
                    }
                }
            }

            private void initEvent(ReadOnlySpan<char> eventLine)
            {
                if (FormatHelper.TryPraseLine(out var key, out var value, eventLine))
                {
                    if (key.Equals("format".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        this.eventFormat = EntryParser.ParseHeader(value).Select(h => SubEvent.FieldInfo[h]).ToArray();
                    }
                    else
                    {
                        if (this.eventFormat == null)
                            this.eventFormat = DefaultEventDef;
                        var isCom = key.Equals("Comment".AsSpan(), StringComparison.OrdinalIgnoreCase);
                        if (!isCom && !key.Equals("Dialogue".AsSpan(), StringComparison.OrdinalIgnoreCase))
                        {
                            if (this.isExact)
                                throw new FormatException($"Unsupported event type \"{key.ToString()}\", only \"Dialogue\" is supported.");
                            else
                                return;
                        }
                        var sub = new SubEvent { IsComment = isCom };
                        if (this.isExact)
                            sub.ParseExact(value, this.eventFormat);
                        else
                            sub.Parse(value, this.eventFormat);
                        this.subtitle.EventCollection.Add(sub);
                    }
                }
            }

        }
    }
}
