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

        internal void Serialize(FieldSerializeHelper[] fieldInfo, TextWriter writer)
        {
            writer.Write(this.EntryName);
            writer.Write(": ");

            for (var i = 0; i < fieldInfo.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                writer.Write(fieldInfo[i].Serialize(this));
            }
        }

        internal void Parse(ReadOnlySpan<char> fields, FieldSerializeHelper[] fieldInfo)
        {
            var data = EntryParser.Parse(fields, fieldInfo.Length);
            for (var i = 0; i < data.Length; i++)
            {
                fieldInfo[i].Deserialize(this, data[i].AsSpan());
            }
        }

        internal void ParseExact(ReadOnlySpan<char> fields, FieldSerializeHelper[] fieldInfo)
        {
            var data = EntryParser.Parse(fields, fieldInfo.Length);
            if (data.Length < fieldInfo.Length)
                throw new FormatException($"Not enough fields, need {fieldInfo.Length}, provided {data.Length}");
            for (var i = 0; i < data.Length; i++)
            {
                fieldInfo[i].DeserializeExact(this, data[i].AsSpan());
            }
        }
    }
}
