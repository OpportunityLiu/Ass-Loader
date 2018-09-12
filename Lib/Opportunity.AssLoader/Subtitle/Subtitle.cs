using Opportunity.AssLoader.Collections;
using Opportunity.AssLoader.Serializer;
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
    /// The abstract class contains methods to get instances of <see cref="Subtitle{TScriptInfo}"/> from ass files.
    /// </summary>
    public abstract partial class Subtitle
    {
        internal Subtitle() { }

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
            = EntryHeader.Parse("Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,StrikeOut,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding");

        internal static readonly string[] DefaultEventFormat
            = EntryHeader.Parse("Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text");

        internal static readonly FieldSerializeHelper[] DefaultStyleDef
            = DefaultStyleFormat.Select(h => Style.FieldInfo[h]).ToArray();
        internal static readonly FieldSerializeHelper[] DefaultEventDef
            = DefaultEventFormat.Select(h => SubEvent.FieldInfo[h]).ToArray();

        /// <summary>
        /// Container of information of the "style" section.
        /// </summary>
        public StyleSet Styles { get; } = new StyleSet();

        /// <summary>
        /// Container of information of the "event" section.
        /// </summary>
        public EventCollection Events { get; } = new EventCollection();

        /// <summary>
        /// Container of information of the "event" section.
        /// </summary>
        public IList<EmbeddedFont> Fonts { get; } = new List<EmbeddedFont>();

        /// <summary>
        /// Container of information of the "event" section.
        /// </summary>
        public IList<EmbeddedGraphic> Graphics { get; } = new List<EmbeddedGraphic>();

        /// <summary>
        /// Used to customize serialization. Will be called by <see cref="Serialize(TextWriter)"/> and <see cref="Serialize()"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        protected abstract void SerializeImplement(TextWriter writer, ISerializeInfo serializeInfo);

        private sealed class SerializeInfo : ISerializeInfo
        {

        }

        /// <summary>
        /// Write the ass file to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write into.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void Serialize(TextWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            SerializeImplement(writer, new SerializeInfo());
            writer.Flush();
        }

        /// <summary>
        /// Write the ass file to <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> presents the ass file.</returns>
        public string Serialize()
        {
            var predictsize = 500
                + Styles.Count * 100
                + Events.Count * 70
                + Fonts.Sum(f => f?.Data?.Length ?? 0) / 3 * 4
                + Graphics.Sum(f => f?.Data?.Length ?? 0) / 3 * 4;

            using (var writer = new StringWriter(new StringBuilder(predictsize), FormatHelper.DefaultFormat))
            {
                this.Serialize(writer);
                return writer.ToString();
            }
        }
    }
}
