﻿using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// The static class contains methods to get instances of <see cref="Subtitle{TScriptInfo}"/> from ass files.
    /// </summary>
    public static partial class Subtitle
    {
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
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
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
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            return new ParseHelper<TScriptInfo>(reader, false, () => new TScriptInfo()).GetResult();
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
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            return new ParseHelper<TScriptInfo>(reader, true, () => new TScriptInfo()).GetResult();
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
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
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
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            return await new ParseHelper<TScriptInfo>(reader, false, () => new TScriptInfo()).GetResultAsync();
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
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            return await new ParseHelper<TScriptInfo>(reader, true, () => new TScriptInfo()).GetResultAsync();
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> Parse<TScriptInfo>(string subtitle, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return Parse<TScriptInfo>(reader, factory);
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> Parse<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(reader, false, factory).GetResult();
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> ParseExact<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(reader, true, factory).GetResult();
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(string subtitle, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (subtitle == null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return await ParseAsync<TScriptInfo>(reader, factory);
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            return await new ParseHelper<TScriptInfo>(reader, false, factory).GetResultAsync();
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static async Task<Subtitle<TScriptInfo>> ParseExactAsync<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            return await new ParseHelper<TScriptInfo>(reader, true, factory).GetResultAsync();
        }

        internal static readonly EntryHeader DefaultStyleFormat = new EntryHeader("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,Strikeout,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding");

        internal static readonly EntryHeader DefaultEventFormat = new EntryHeader("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text");
    }
}
