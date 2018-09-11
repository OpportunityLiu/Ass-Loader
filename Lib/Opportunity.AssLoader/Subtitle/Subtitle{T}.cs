using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ScriptInfoSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.ScriptInfoCollection, Opportunity.AssLoader.ScriptInfoAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Subtitle file.
    /// </summary>
    /// <typeparam name="TScriptInfo">
    /// Type of the container of the "script info" section of the ass file.
    /// </typeparam>
    public class Subtitle<TScriptInfo> : ISubtitle
        where TScriptInfo : ScriptInfoCollection
    {
        private static readonly Dictionary<string, ScriptInfoSerializeHelper> ScriptInfoFieldsStatic
            = ScriptInfoSerializeHelper.GetScriptInfoFields(typeof(TScriptInfo));

        Dictionary<string, ScriptInfoSerializeHelper> ISubtitle.ScriptInfoFields => ScriptInfoFieldsStatic;

        /// <summary>
        /// Create a new instance of <see cref="Subtitle{TScriptInfo}"/>.
        /// </summary>
        /// <param name="scriptInfo">
        /// The <typeparamref name="TScriptInfo"/>of the <see cref="Subtitle{TScriptInfo}"/>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="scriptInfo"/> is <see langword="null"/>.</exception>
        public Subtitle(TScriptInfo scriptInfo)
        {
            this.ScriptInfo = scriptInfo ?? throw new ArgumentNullException(nameof(scriptInfo));
            scriptInfo.Parent = this;
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

            writer.WriteLine("[Script Info]");
            this.ScriptInfo.Serialize(writer, null);
            writer.WriteLine();

            writer.WriteLine("[V4+ Styles]");
            this.saveStyle(writer);
            writer.WriteLine();

            writer.WriteLine("[Events]");
            this.saveEvent(writer);

            writer.Flush();
        }

        private void saveStyle(TextWriter writer)
        {
            EntryParser.Serialize(Subtitle.DefaultStyleFormat, writer);
            writer.WriteLine();
            foreach (var item in this.StyleSet)
            {
                item.Serialize(writer, Subtitle.DefaultStyleDef, null);
                writer.WriteLine();
            }
        }

        private void saveEvent(TextWriter writer)
        {
            EntryParser.Serialize(Subtitle.DefaultEventFormat, writer);
            writer.WriteLine();
            foreach (var item in this.EventCollection)
            {
                item.Serialize(writer, Subtitle.DefaultEventDef, null);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Write the ass file to <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> presents the ass file.</returns>
        public string Serialize()
        {
            using (var writer = new StringWriter(new StringBuilder((StyleSet.Count + EventCollection.Count) * 50), FormatHelper.DefaultFormat))
            {
                this.Serialize(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Container of information of the "script info" section.
        /// </summary>
        public TScriptInfo ScriptInfo { get; }

        /// <summary>
        /// Container of information of the "style" section.
        /// </summary>
        public StyleSet StyleSet { get; } = new StyleSet();

        /// <summary>
        /// Container of information of the "event" section.
        /// </summary>
        public EventCollection EventCollection { get; } = new EventCollection();
    }
}
