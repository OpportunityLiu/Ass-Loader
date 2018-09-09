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
        /// <summary>
        /// Create new instance of <see cref="Entry"/>.
        /// </summary>
        protected Entry() { }

        private Dictionary<string, FieldSerializeHelper> getFieldInfo() => FieldSerializeHelper.GetScriptInfoFields(GetType());

        /// <summary>
        /// Name of this <see cref="Entry"/>.
        /// </summary>
        protected abstract string EntryName { get; }

        /// <summary>
        /// Serilaize this <see cref="Entry"/> to the ass form, with the given <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <param name="writer">A <see cref="TextWriter"/> to write into.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Serialize(EntryHeader format, TextWriter writer)
        {
            if (format is null) throw new ArgumentNullException(nameof(format));
            if (writer is null) throw new ArgumentNullException(nameof(writer));

            writer.Write(this.EntryName);
            writer.Write(": ");

            var fieldInfo = getFieldInfo();
            var f = format.Data;

            for (var i = 0; i < f.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                writer.Write(fieldInfo[f[i]].Serialize(this));
            }
        }

        /// <summary>
        /// Parse from <paramref name="fields"/>, deserialize and save to this <see cref="Entry"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        protected void Parse(string fields, EntryHeader format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(fields))
                throw new ArgumentNullException(nameof(fields));
            var data = new EntryData(fields, format.Count);
            var fieldInfo = getFieldInfo();
            for (var i = 0; i < format.Count; i++)
            {
                if (!fieldInfo.TryGetValue(format[i], out var target))
                    continue;
                target.Deserialize(this, data[i]);
            }
        }

        /// <summary>
        /// Parse exactly from <paramref name="fields"/>, deserialize and save to this <see cref="Entry"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Fields of <see cref="Entry"/> and fields of <paramref name="format"/> doesn't match
        /// </exception>
        protected void ParseExact(string fields, EntryHeader format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(fields))
                throw new ArgumentNullException(nameof(fields));
            var data = new EntryData(fields, format.Count);
            var fieldInfo = getFieldInfo();
            for (var i = 0; i < format.Count; i++)
                fieldInfo[format[i]].DeserializeExact(this, data[i]);
        }

        /// <summary>
        /// Make a copy of this <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A subclass of <see cref="Entry"/>.</typeparam>
        /// <returns>A copy of this <typeparamref name="T"/>.</returns>
        protected T Clone<T>() where T : Entry, new()
        {
            var re = new T();
            var fieldInfo = getFieldInfo();
            foreach (var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }

        /// <summary>
        /// Make a copy of this <see cref="Entry"/>.
        /// </summary>
        /// <returns>A copy of this <see cref="Entry"/>.</returns>
        protected Entry Clone()
        {
            var re = (Entry)Activator.CreateInstance(this.GetType());
            var fieldInfo = getFieldInfo();
            foreach (var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }

        /// <summary>
        /// Make a copy of this <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A subclass of <see cref="Entry"/>.</typeparam>
        /// <param name="factory">The factroy method to create new instance of the subclass.</param>
        /// <returns>A copy of this <typeparamref name="T"/>.</returns>
        protected T Clone<T>(Func<T> factory) where T : Entry
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            var re = factory();
            var fieldInfo = getFieldInfo();
            foreach (var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }

        /// <summary>
        /// Make a copy of this <see cref="Entry"/>.
        /// </summary>
        /// <param name="factory">The factroy method to create new instance of the subclass.</param>
        /// <returns>A copy of this <see cref="Entry"/>.</returns>
        protected Entry Clone(Func<Entry> factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            var re = factory();
            var fieldInfo = getFieldInfo();
            foreach (var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }
    }
}
