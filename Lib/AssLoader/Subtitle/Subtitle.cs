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
        private class parseHelper<T> where T:ScriptInfoCollection,new()
        {
            public parseHelper(TextReader reader,bool isExact)
            {
                this.reader = reader;
                this.isExact = isExact;
            }

            public Subtitle<T> getResult()
            {
                try
                {
                    var sec = section.Unknown;
                    while(true)
                    {
                        var temp = reader.ReadLine();
                        if(temp == null)
                            return subtitle;
                        temp = temp.Trim(null);
                        if(string.IsNullOrEmpty(temp) || temp[0] == ';')
                            continue;
                        if(temp[0] == '[' && temp[temp.Length - 1] == ']')
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
                                    break;
                            }
                        else
                            switch(sec)
                            {
                                case section.ScriptInfo:
                                    subtitle.ScriptInfo.ParseLine(temp);
                                    break;
                                case section.Styles:
                                    initStyle(temp);
                                    break;
                                case section.Events:
                                    initEvent(temp);
                                    break;
                                default:
                                    break;
                            }
                    }
                }
                catch(Exception ex) when (ex is ArgumentException || ex is FormatException)
                {
                    throw new ArgumentException("Error occurs during parsing.", ex);
                }
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
                                styleFormat = StyleFormat;
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
                            eventFormat = EventFormat;
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
                throw new ArgumentNullException("subtitle");
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
                throw new ArgumentNullException("reader");
            var helper = new parseHelper<TScriptInfo>(reader, false);
            return helper.getResult();
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
                throw new ArgumentNullException("reader");
            var helper = new parseHelper<TScriptInfo>(reader, true);
            return helper.getResult();
        }

        internal readonly static EntryHeader StyleFormat = new EntryHeader("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,Strikeout,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding");

        internal readonly static EntryHeader EventFormat = new EntryHeader("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text");
    }
}
