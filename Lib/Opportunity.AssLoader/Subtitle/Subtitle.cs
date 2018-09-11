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
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="ParseResult{TScriptInfo}"/> presents the parse result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static ParseResult<AssScriptInfo> Parse(string subtitle)
        {
            return Parse<AssScriptInfo>(subtitle);
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="ParseResult{TScriptInfo}"/> presents the parse result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static ParseResult<TScriptInfo> Parse<TScriptInfo>(string subtitle)
            where TScriptInfo : ScriptInfoCollection, new()
        {
            return Parse(subtitle, () => new TScriptInfo());
        }

        /// <summary>
        /// Parse the <see cref="string"/> of ass file.
        /// </summary>
        /// <typeparam name="TScriptInfo">Type of the container of the "script info" section of the ass file.</typeparam>
        /// <param name="subtitle">A <see cref="string"/> of ass file.</param>
        /// <returns>A <see cref="ParseResult{TScriptInfo}"/> presents the parse result.</returns>
        /// <param name="factory">The factory method to create <typeparamref name="TScriptInfo"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subtitle"/> or <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="subtitle"/> contains an ass file of wrong format.</exception>
        public static ParseResult<TScriptInfo> Parse<TScriptInfo>(string subtitle, Func<TScriptInfo> factory)
            where TScriptInfo : ScriptInfoCollection
        {
            if (subtitle is null)
                throw new ArgumentNullException(nameof(subtitle));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            return new ParseHelper<TScriptInfo>(subtitle, factory).GetResult();
        }

        internal static readonly string[] DefaultStyleFormat
            = EntryParser.ParseHeader("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,StrikeOut,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding".AsSpan());

        internal static readonly string[] DefaultEventFormat
            = EntryParser.ParseHeader("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text".AsSpan());

        internal static readonly FieldSerializeHelper[] DefaultStyleDef
            = DefaultStyleFormat.Select(h => Style.FieldInfo[h]).ToArray();
        internal static readonly FieldSerializeHelper[] DefaultEventDef
            = DefaultEventFormat.Select(h => SubEvent.FieldInfo[h]).ToArray();
    }
}
