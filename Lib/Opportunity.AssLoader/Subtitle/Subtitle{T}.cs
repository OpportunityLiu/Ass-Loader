using Opportunity.AssLoader.Collections;
using Opportunity.AssLoader.Serializer;
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
    public partial class Subtitle
    {
        internal abstract Dictionary<string, ScriptInfoSerializeHelper> ScriptInfoFields { get; }
    }

    /// <summary>
    /// Subtitle file.
    /// </summary>
    /// <typeparam name="TScriptInfo">
    /// Type of the container of the "script info" section of the ass file.
    /// </typeparam>
    public class Subtitle<TScriptInfo> : Subtitle
        where TScriptInfo : ScriptInfoCollection
    {
        private static readonly Dictionary<string, ScriptInfoSerializeHelper> ScriptInfoFieldsStatic
            = ScriptInfoSerializeHelper.GetScriptInfoFields(typeof(TScriptInfo));

        internal sealed override Dictionary<string, ScriptInfoSerializeHelper> ScriptInfoFields => ScriptInfoFieldsStatic;

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

        private void saveStyle(TextWriter writer)
        {
            EntryHeader.Serialize(DefaultStyleFormat, writer);
            writer.WriteLine();
            foreach (var item in this.Styles)
            {
                item.Serialize(writer, DefaultStyleDef, null);
                writer.WriteLine();
            }
        }

        private void saveEvent(TextWriter writer)
        {
            EntryHeader.Serialize(DefaultEventFormat, writer);
            writer.WriteLine();
            foreach (var item in this.Events)
            {
                item.Serialize(writer, DefaultEventDef, null);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Default implementation.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        protected override void SerializeImplement(TextWriter writer, ISerializeInfo serializeInfo)
        {
            writer.WriteLine("[Script Info]");
            this.ScriptInfo.Serialize(writer, null);
            writer.WriteLine();

            writer.WriteLine("[V4+ Styles]");
            this.saveStyle(writer);
            writer.WriteLine();

            writer.WriteLine("[Events]");
            this.saveEvent(writer);
            writer.WriteLine();

            if (!Fonts.IsEmpty())
            {
                writer.WriteLine("[Fonts]");
                foreach (var item in Fonts)
                {
                    item.Serialize(writer, serializeInfo);
                }
                writer.WriteLine();
            }

            if (!Graphics.IsEmpty())
            {
                writer.WriteLine("[Graphics]");
                foreach (var item in Graphics)
                {
                    item.Serialize(writer, serializeInfo);
                }
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Container of information of the "script info" section.
        /// </summary>
        public TScriptInfo ScriptInfo { get; }
    }
}
