using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScriptInfoSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.ScriptInfoCollection, Opportunity.AssLoader.ScriptInfoAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Container of the "script info" section.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class ScriptInfoCollection : IDictionary<string, string>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                var d = this.Parent.ScriptInfoFields;
                var ud = this.undefinedFields;
                var du = 0;
                foreach (var item in d.Values)
                {
                    var value = item.GetValue(this);
                    if (value is null || Equals(value, item.Info.DefaultValue))
                        continue;
                    du++;
                }
                return $"Defined = {du}/{d.Count}, Undefined = {ud.Count}";
            }
        }

        internal Subtitle Parent;

        /// <summary>
        /// Create new instance of <see cref="ScriptInfoCollection"/>.
        /// </summary>
        public ScriptInfoCollection()
        {
            this.UndefinedFields = new ReadOnlyDictionary<string, string>(this.undefinedFields);
        }

        internal void ParseLine(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            if (FormatHelper.TryPraseLine(out var k, out var v, value))
            {
                var key = k.ToString();
                if (this.Parent.ScriptInfoFields.TryGetValue(key, out var helper))
                    helper.Deserialize(v, this, deserializeInfo);
                else
                {
                    deserializeInfo.AddException(new ArgumentException($"Undefined property `{key}`."));
                    this.undefinedFields[key] = v.ToString();
                }
            }
            else
                deserializeInfo.AddException(new ArgumentException("Wrong line format."));
        }

        /// <summary>
        /// Write info of this <see cref="ScriptInfoCollection"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write into.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public virtual void Serialize(TextWriter writer, ISerializeInfo serializeInfo)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var sb = new StringBuilder();
            var bfw = new StringWriter(sb);
            foreach (var item in this.Parent.ScriptInfoFields.Values)
            {
                item.Serialize(bfw, this, serializeInfo);
                if (sb.Length == 0)
                    continue;
                writer.Write(item.Info.FieldName);
                writer.Write(": ");
                writer.WriteLine(sb.ToString());
                sb.Clear();
            }
            if (this.undefinedFields.Count == 0)
                return;

            //unknown script info entries.
            writer.WriteLine();
            foreach (var item in this.undefinedFields)
                writer.WriteLine($"{item.Key}: {item.Value}");
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, string> undefinedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Fields that undifined in this <see cref="ScriptInfoCollection"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> UndefinedFields { get; }

        /// <summary>
        /// Add a key-value pair into <see cref="UndefinedFields"/>.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/> is in the <see cref="ScriptInfoCollection"/>.</exception>
        public void Add(string key, string value)
        {
            if (this.ContainsKey(key))
                throw new ArgumentException("Key contains in the collection.", nameof(key));
            this.undefinedFields[key] = value;
        }

        /// <summary>
        /// Make sure this <see cref="ScriptInfoCollection"/> contains <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to find in <see cref="ScriptInfoCollection"/>.</param>
        /// <returns>True if <paramref name="key"/> found in defined fields or <see cref="UndefinedFields"/>.</returns>
        public bool ContainsKey(string key)
        {
            return this.undefinedFields.ContainsKey(key) || this.Parent.ScriptInfoFields.ContainsKey(key);
        }

        /// <summary>
        /// Remove the key-value pair in <see cref="UndefinedFields"/>.
        /// </summary>
        /// <param name="key">The key of value to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <returns>True if value removed sucessfully.</returns>
        public bool Remove(string key)
        {
            return this.undefinedFields.Remove(key);
        }

        /// <summary>
        /// Try to get the string form of value.
        /// </summary>
        /// <param name="key">The key of value to get.</param>
        /// <param name="value">The string form of value if <paramref name="key"/> found in the <see cref="ScriptInfoCollection"/>.</param>
        /// <returns>True if <paramref name="key"/> found in the <see cref="ScriptInfoCollection"/>.</returns>
        public bool TryGetValue(string key, out string value)
        {
            if (this.undefinedFields.TryGetValue(key, out var s))
            {
                value = s;
                return true;
            }
            if (this.Parent.ScriptInfoFields.TryGetValue(key, out var ssi))
            {
                var w = new StringWriter();
                ssi.Serialize(w, this, null);
                value = w.ToString();
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Get or set value of the key.
        /// </summary>
        /// <param name="key">The key of value.</param>
        /// <param name="value">The string form of value.</param>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> doesn't found in the <see cref="ScriptInfoCollection"/> during getting value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="key"/> is found in the defined fields of this <see cref="ScriptInfoCollection"/> during setting value.
        /// </exception>
        public string this[string key]
        {
            get
            {
                if (this.TryGetValue(key, out var va))
                    return va;
                throw new KeyNotFoundException($"\"{key}\" not found.");
            }
            set
            {
                if (this.Parent.ScriptInfoFields.ContainsKey(key))
                    throw new InvalidOperationException("The key has defined, please use property.");
                else
                    this.undefinedFields[key] = value;
            }
        }

        /// <summary>
        /// Remove all key-value pairs in <see cref="UndefinedFields"/>.
        /// </summary>
        public void Clear()
        {
            this.undefinedFields.Clear();
        }

        #region IDictionary<string,string> 成员

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ICollection<string> IDictionary<string, string>.Keys
        {
            get
            {
                if (this.keys == null)
                    return this.keys = new Collection(this, true);
                return this.keys;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ICollection<string> IDictionary<string, string>.Values
        {
            get
            {
                if (this.values == null)
                    return this.values = new Collection(this, false);
                return this.values;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ICollection<string> keys, values;

        private class Collection : ICollection<string>
        {
            private readonly bool isKey;

            private readonly ScriptInfoCollection collection;

            internal Collection(ScriptInfoCollection collection, bool isKey)
            {
                this.collection = collection;
                this.isKey = isKey;
            }

            #region ICollection<string> 成员

            void ICollection<string>.Add(string item)
            {
                throw new NotImplementedException();
            }

            void ICollection<string>.Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(string item)
            {
                if (this.isKey)
                    return this.collection.ContainsKey(item);
                else
                    return this.collection.Any(i => i.Value == item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                var f = this.collection.Parent.ScriptInfoFields;
                if (this.isKey)
                    f.Keys
                        .Concat(this.collection.undefinedFields.Keys)
                        .ToArray().CopyTo(array, arrayIndex);
                else
                    f.Values
                        .Select(item => { var w = new StringWriter(); item.Serialize(w, this.collection, null); return w.ToString(); })
                        .Concat(this.collection.undefinedFields.Values)
                        .ToArray().CopyTo(array, arrayIndex);
            }

            public int Count => ((ICollection<KeyValuePair<string, string>>)this.collection).Count;

            bool ICollection<string>.IsReadOnly => true;

            bool ICollection<string>.Remove(string item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                var f = this.collection.Parent.ScriptInfoFields;
                if (this.isKey)
                    return f.Keys
                        .Concat(this.collection.undefinedFields.Keys).GetEnumerator();
                else
                    return f.Values
                        .Select(item => { var w = new StringWriter(); item.Serialize(w, this.collection, null); return w.ToString(); })
                        .Concat(this.collection.undefinedFields.Values).GetEnumerator();
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> 成员

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return this.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            this.Parent.ScriptInfoFields
                .Select(item => { var w = new StringWriter(); item.Value.Serialize(w, this, null); return new KeyValuePair<string, string>(item.Key, w.ToString()); })
                .Concat(this.undefinedFields).CopyTo(array, arrayIndex);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int ICollection<KeyValuePair<string, string>>.Count => this.Parent.ScriptInfoFields.Count + this.undefinedFields.Count;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (this.undefinedFields.ContainsKey(item.Key))
                return this.undefinedFields.Remove(item.Key);
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> 成员

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return this.Parent.ScriptInfoFields
                .Select(item => { var w = new StringWriter(); item.Value.Serialize(w, this, null); return new KeyValuePair<string, string>(item.Key, w.ToString()); })
                .Concat(this.undefinedFields).GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
        }

        #endregion
    }
}