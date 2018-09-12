using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Entry of ass file.
    /// </summary>
    public abstract class Entry
    {
        internal Entry() { }

        /// <summary>
        /// Name of this <see cref="Entry"/>.
        /// </summary>
        protected abstract string EntryName { get; }

        internal void Serialize(TextWriter writer, FieldSerializeHelper[] fieldInfo, ISerializeInfo serializeInfo)
        {
            writer.Write(this.EntryName);
            writer.Write(": ");

            for (var i = 0; i < fieldInfo.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                var fi = fieldInfo[i];
                if (fi != null)
                    fi.Serialize(writer, this, serializeInfo);
            }
        }

        internal void Parse(ReadOnlySpan<char> fields, FieldSerializeHelper[] fieldInfo, IDeserializeInfo deserializeInfo)
        {
            var i = 0;
            foreach (var item in fields.Split(',', fieldInfo.Length))
            {
                var field = item.Trim();
                var fi = fieldInfo[i];
                if (fi != null)
                    fi.Deserialize(field, this, deserializeInfo);
                i++;
            }

            if (i < fieldInfo.Length)
                deserializeInfo.AddException(new FormatException($"Not enough fields, need {fieldInfo.Length}, provided {i}"));
        }
    }
}
