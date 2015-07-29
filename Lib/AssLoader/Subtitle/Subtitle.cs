using AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    /// <summary>
    /// The static class contains methods to get instances of <see cref="Subtitle{TScriptInfo}"/> from ass files.
    /// </summary>
    public static class Subtitle
    {
        private class parseHelper<T> where T : ScriptInfoCollection, new()
        {
            public parseHelper(TextReader reader, bool isExact)
            {
                this.reader = reader;
                this.isExact = isExact;
            }

            public Subtitle<T> GetResult()
            {
                int lineNumber = 0;
                string line = null;
                var sec = isExact ? section.Unknown : section.ScriptInfo;
                string secStr = null;
                try
                {
                    while(true)
                    {
                        line = reader.ReadLine();
                        lineNumber++;
                        if(line == null)
                            return subtitle;

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
                                    sec = section.ScriptInfo;
                                    break;
                                case "[v4+ styles]":
                                case "[v4 styles+]":
                                case "[v4+styles]":
                                case "[v4styles+]":
                                    sec = section.Styles;
                                    break;
                                case "[events]":
                                    sec = section.Events;
                                    break;
                                default:
                                    sec = section.Unknown;
                                    secStr = temp.Substring(1, temp.Length - 2);
                                    if(isExact)
                                        throw new InvalidOperationException($"Unknown section \"{secStr}\" found.");
                                    break;
                            }
                        }
                        else // Section content
                        {
                            switch(sec)
                            {
                                case section.ScriptInfo:
                                    initScriptInfo(temp);
                                    break;
                                case section.Styles:
                                    initStyle(temp);
                                    break;
                                case section.Events:
                                    initEvent(temp);
                                    break;
                                default:
                                    if(isExact)
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
Content of the line: {line}", ex);
                    exception.Data.Add("Line number", lineNumber);
                    exception.Data.Add("Line content", line);
                    exception.Data.Add("Current section", sec.ToString());
                    throw exception;
                }
            }

            public Task<Subtitle<T>> GetResultAsync()
            {
                return Task.Run(() => GetResult());
            }

            private enum section
            {
                Unknown = 0,
                ScriptInfo,
                Styles,
                Events
            }

            private TextReader reader;

            private Subtitle<T> subtitle = new Subtitle<T>();

            private bool isExact;

            private EntryHeader styleFormat, eventFormat;

            private void initScriptInfo(string scriptInfoLine)
            {
                if(isExact)
                    subtitle.ScriptInfo.ParseLineExact(scriptInfoLine);
                else
                    subtitle.ScriptInfo.ParseLine(scriptInfoLine);
            }

            private void initStyle(string styleLine)
            {
                string key, value;
                if(FormatHelper.TryPraseLine(out key, out value, styleLine))
                {
                    switch(key.ToLower())
                    {
                        case "format":
                            styleFormat = new EntryHeader(value);
                            return;
                        case "style":
                            if(styleFormat == null)
                                styleFormat = DefaultStyleFormat;
                            var s = isExact ? Style.ParseExact(styleFormat, value) : Style.Parse(styleFormat, value);
                            try
                            {
                                subtitle.StyleDictionary.Add(s);
                            }
                            catch(ArgumentException) when (!isExact)
                            {
                            }
                            return;
                        default:
                            return;
                    }
                }
            }

            private void initEvent(string eventLine)
            {
                string key, value;
                if(FormatHelper.TryPraseLine(out key, out value, eventLine))
                {
                    if(string.Equals(key, "format", StringComparison.OrdinalIgnoreCase))
                    {
                        eventFormat = new EntryHeader(value);
                    }
                    else
                    {
                        if(eventFormat == null)
                            eventFormat = DefaultEventFormat;
                        var sub = isExact ? SubEvent.ParseExact(eventFormat, string.Equals(key, "comment", StringComparison.OrdinalIgnoreCase), value) : SubEvent.Parse(eventFormat, string.Equals(key, "comment", StringComparison.OrdinalIgnoreCase), value);
                        subtitle.EventCollection.Add(sub);
                    }
                }
            }

        }

        internal static readonly string[] EditorInfo =
        {
            "; This file is generated by AssLoader",
            "; https://assloader.codeplex.com",
            ";"
        };

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> Parse<TScriptInfo>(string subtitle)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using(var reader = new StringReader(subtitle))
                return Parse<TScriptInfo>(reader);
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> Parse<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(reader == null)
                throw new ArgumentNullException(nameof(reader));
            return new parseHelper<TScriptInfo>(reader, false).GetResult();
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> ParseExact<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(reader == null)
                throw new ArgumentNullException(nameof(reader));
            return new parseHelper<TScriptInfo>(reader, true).GetResult();
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(string subtitle)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using(var reader = new StringReader(subtitle))
                return await ParseAsync<TScriptInfo>(reader);
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(reader == null)
                throw new ArgumentNullException(nameof(reader));
            return await new parseHelper<TScriptInfo>(reader, false).GetResultAsync();
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseExactAsync<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if(reader == null)
                throw new ArgumentNullException(nameof(reader));
            return await new parseHelper<TScriptInfo>(reader, true).GetResultAsync();
        }

        internal readonly static EntryHeader DefaultStyleFormat = new EntryHeader("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,Strikeout,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding");

        internal readonly static EntryHeader DefaultEventFormat = new EntryHeader("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text");
    }
}
