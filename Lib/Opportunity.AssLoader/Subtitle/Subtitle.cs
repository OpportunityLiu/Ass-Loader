using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

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
            if (subtitle is null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return Parse<TScriptInfo>(reader);
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static Subtitle<TScriptInfo> ParseExact<TScriptInfo>(string subtitle)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            if (subtitle is null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return ParseExact<TScriptInfo>(reader);
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
            return Parse(reader, () => new TScriptInfo());
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
            return ParseExact(reader, () => new TScriptInfo());
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            return ParseAsync(reader, () => new TScriptInfo());
        }

        /// <summary>
        /// Parse the <see cref="TextReader"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="reader">A <see cref="TextReader"/> of ass file.</param>
        /// <returns>A <see cref="Subtitle{TScriptInfo}"/> presents the ass file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="reader"/> contains an ass file of wrong format.</exception>
        public static Task<Subtitle<TScriptInfo>> ParseExactAsync<TScriptInfo>(TextReader reader)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            return ParseExactAsync(reader, () => new TScriptInfo());
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
            if (subtitle is null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return Parse(reader, factory);
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
        public static Subtitle<TScriptInfo> ParseExact<TScriptInfo>(string subtitle, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (subtitle is null)
                throw new ArgumentNullException(nameof(subtitle));
            using (var reader = new StringReader(subtitle))
                return ParseExact(reader, factory);
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
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (factory is null)
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
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(reader, true, factory).GetResult();
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
        public static Task<Subtitle<TScriptInfo>> ParseAsync<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(reader, false, factory).GetResultAsync();
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
        public static Task<Subtitle<TScriptInfo>> ParseExactAsync<TScriptInfo>(TextReader reader, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(reader, true, factory).GetResultAsync();
        }

        internal static readonly string[] DefaultStyleFormat
            = EntryParser.ParseHeader("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,Strikeout,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding".AsSpan());

        internal static readonly string[] DefaultEventFormat
            = EntryParser.ParseHeader("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text".AsSpan());

        internal static readonly FieldSerializeHelper[] DefaultStyleDef
            = DefaultStyleFormat.Select(h => Style.FieldInfo[h]).ToArray();
        internal static readonly FieldSerializeHelper[] DefaultEventDef
            = DefaultEventFormat.Select(h => SubEvent.FieldInfo[h]).ToArray();
    }
}
