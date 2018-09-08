using Opportunity.AssLoader.Collections;
using System;
using System.IO;
using System.Threading.Tasks;

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
                string line = null;
                var sec = this.isExact ? Section.Unknown : Section.ScriptInfo;
                string secStr = null;
                try
                {
                    while(true)
                    {
                        line = this.reader.ReadLine();
                        lineNumber++;
                        if(line == null)
                            return this.subtitle;

                        var temp = line.Trim(null);

                        // Skip empty lines and comment lines.
                        if(temp.Length == 0 || temp[0] == ';')
                            continue;

                        if(temp[0] == '[' && temp[temp.Length - 1] == ']') // Section header
                        {
                            switch(temp.ToLower())
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
                                secStr = temp.Substring(1, temp.Length - 2);
                                if(this.isExact)
                                    throw new InvalidOperationException($"Unknown section \"{secStr}\" found.");
                                break;
                            }
                        }
                        else // Section content
                        {
                            switch(sec)
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
                                if(this.isExact)
                                    throw new InvalidOperationException("Content found without a section header.");
                                break;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    var exception = new ArgumentException($@"Error occurs during parsing.
Line number: {lineNumber}
Content of the line:
{line}", ex);
                    exception.Data.Add("Line number", lineNumber);
                    exception.Data.Add("Line content", line);
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

            private EntryHeader styleFormat, eventFormat;

            private void initScriptInfo(string scriptInfoLine)
            {
                if(this.isExact)
                    this.subtitle.ScriptInfo.ParseLineExact(scriptInfoLine);
                else
                    this.subtitle.ScriptInfo.ParseLine(scriptInfoLine);
            }

            private void initStyle(string styleLine)
            {
                if(FormatHelper.TryPraseLine(out var key, out var value, styleLine))
                {
                    switch(key.ToLower())
                    {
                    case "format":
                        this.styleFormat = new EntryHeader(value);
                        return;
                    case "style":
                        if(this.styleFormat == null)
                            this.styleFormat = DefaultStyleFormat;
                        Style s;
                        if(this.isExact)
                        {
                            s = Style.ParseExact(this.styleFormat, value);
                            if(this.subtitle.StyleSet.ContainsName(s.Name))
                                throw new ArgumentException($"Style with the name \"{s.Name}\" is already in the StyleSet.");
                        }
                        else
                        {
                            s = Style.Parse(this.styleFormat, value);
                        }
                        this.subtitle.StyleSet.Add(s);
                        return;
                    default:
                        return;
                    }
                }
            }

            private void initEvent(string eventLine)
            {
                if(FormatHelper.TryPraseLine(out var key, out var value, eventLine))
                {
                    if(string.Equals(key, "format", StringComparison.OrdinalIgnoreCase))
                    {
                        this.eventFormat = new EntryHeader(value);
                    }
                    else
                    {
                        if(this.eventFormat == null)
                            this.eventFormat = DefaultEventFormat;
                        var sub = this.isExact 
                            ? SubEvent.ParseExact(this.eventFormat, string.Equals(key, "comment", StringComparison.OrdinalIgnoreCase), value) 
                            : SubEvent.Parse(this.eventFormat, string.Equals(key, "comment", StringComparison.OrdinalIgnoreCase), value);
                        this.subtitle.EventCollection.Add(sub);
                    }
                }
            }

        }
    }
}
