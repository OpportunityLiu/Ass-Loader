﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.ObjectModel;
using Opportunity.AssLoader.Serializer;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Opportunity.AssLoader.Collections
{
    /// <summary>
    /// Container of the "script info" section.
    /// </summary>
    public abstract class ScriptInfoCollection : IDictionary<string, string>, INotifyPropertyChanged
    {
        /// <summary>
        /// Create new instance of <see cref="ScriptInfoCollection"/>.
        /// </summary>
        protected ScriptInfoCollection()
        {
            var type = this.GetType();
            if(!scriptInfoCache.TryGetValue(type, out this.scriptInfoFields))
            {
                var temp = new List<Dictionary<string, ScriptInfoSerializeHelper>>();
                do
                {
                    var dict = new Dictionary<string, ScriptInfoSerializeHelper>(StringComparer.OrdinalIgnoreCase);
                    foreach(var fInfo in type.GetRuntimeFields())
                    {
                        var att = fInfo.GetCustomAttribute<ScriptInfoAttribute>();
                        if(att == null)
                            continue;
                        var ser = fInfo.GetCustomAttribute<SerializeAttribute>();
                        dict.Add(att.FieldName, new ScriptInfoSerializeHelper(fInfo, att, ser));
                    }
                    temp.Add(dict);
                } while((type = type.GetTypeInfo().BaseType) != typeof(ScriptInfoCollection));
                this.scriptInfoFields = (from dict in temp.Reverse<Dictionary<string, ScriptInfoSerializeHelper>>()
                                    from entry in dict
                                    select entry).ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
                scriptInfoCache.Add(this.GetType(), this.scriptInfoFields);
            }
            this.UndefinedFields = new ReadOnlyDictionary<string, string>(this.undefinedFields);
        }

        internal void ParseLine(string value)
        {
            if(FormatHelper.TryPraseLine(out var k, out var v, value))
            {
                if(this.scriptInfoFields.TryGetValue(k, out var helper))
                    helper.Deserialize(this, v);
                else
                    this.undefinedFields[k] = v;
            }
        }

        internal void ParseLineExact(string value)
        {
            if(FormatHelper.TryPraseLine(out var k, out var v, value))
            {
                if(this.scriptInfoFields.TryGetValue(k, out var helper))
                    helper.DeserializeExact(this, v);
                else
                    this.undefinedFields[k] = v;
            }
        }

        /// <summary>
        /// Write info of this <see cref="ScriptInfoCollection"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write into.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void Serialize(TextWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException(nameof(writer));
            foreach(var item in this.scriptInfoFields.Values)
            {
                var toWrite = item.Serialize(this);
                if(toWrite != null)
                    writer.WriteLine(toWrite);
            }
            if(this.undefinedFields.Count == 0)
                return;

            //unknown script info entries.
            writer.WriteLine();
            foreach(var item in this.undefinedFields)
                writer.WriteLine($"{item.Key}: {item.Value}");
        }

        private static Dictionary<Type, Dictionary<string, ScriptInfoSerializeHelper>> scriptInfoCache = new Dictionary<Type, Dictionary<string, ScriptInfoSerializeHelper>>();

        private Dictionary<string, ScriptInfoSerializeHelper> scriptInfoFields;

        private Dictionary<string, string> undefinedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Fields that undifined in this <see cref="ScriptInfoCollection"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> UndefinedFields
        {
            get;
            private set;
        }

        /// <summary>
        /// Add a key-value pair into <see cref="UndefinedFields"/>.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/> is in the <see cref="ScriptInfoCollection"/>.</exception>
        public void Add(string key, string value)
        {
            if(this.ContainsKey(key))
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
            return this.undefinedFields.ContainsKey(key) || this.scriptInfoFields.ContainsKey(key);
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
            if(this.undefinedFields.TryGetValue(key, out var s))
            {
                value = s;
                return true;
            }
            if(this.scriptInfoFields.TryGetValue(key, out var ssi))
            {
                value = ssi.Serialize(this);
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
                if(TryGetValue(key, out var va))
                    return va;
                throw new KeyNotFoundException($"\"{key}\" not found.");
            }
            set
            {
                if(this.scriptInfoFields.ContainsKey(key))
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

        ICollection<string> IDictionary<string, string>.Keys
        {
            get
            {
                if(this.keys == null)
                    return this.keys = new Collection(this, true);
                return this.keys;
            }
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get
            {
                if(this.values == null)
                    return this.values = new Collection(this, false);
                return this.values;
            }
        }

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
                if(this.isKey)
                    return this.collection.ContainsKey(item);
                else
                    return this.collection.Any(i => i.Value == item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                if(this.isKey)
                    this.collection.scriptInfoFields.Keys
                        .Concat(this.collection.undefinedFields.Keys)
                        .ToArray().CopyTo(array, arrayIndex);
                else
                    this.collection.scriptInfoFields.Values
                        .Select(item => item.Serialize(this))
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
                if(this.isKey)
                    return this.collection.scriptInfoFields.Keys
                        .Concat(this.collection.undefinedFields.Keys).GetEnumerator();
                else
                    return this.collection.scriptInfoFields.Values
                        .Select(item => item.Serialize(this))
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
            this.scriptInfoFields
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value.Serialize(this)))
                .Concat(this.undefinedFields).ToArray().CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, string>>.Count => this.scriptInfoFields.Count + this.undefinedFields.Count;

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if(this.undefinedFields.ContainsKey(item.Key))
                return this.undefinedFields.Remove(item.Key);
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> 成员

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return this.scriptInfoFields
                 .Select(item => new KeyValuePair<string, string>(item.Key, item.Value.Serialize(this)))
                 .Concat(this.undefinedFields).GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
        }

        #endregion

        #region INotifyPropertyChanged 成员

        /// <summary>
        /// Occurs when the property changes.
        /// </summary>
        /// <remarks>
        /// The event handler receives an argument of type
        /// <seealso cref="PropertyChangedEventHandler" />
        /// containing data related to this event.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the event <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changing property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set the field and raise the event <see cref="PropertyChanged"/> if needed.
        /// </summary>
        /// <param name="propertyName">The name of the changing property.</param>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set.</param>
        protected void Set<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if(Equals(field, value))
            {
                return;
            }
            field = value;
            RaisePropertyChanged(propertyName);
        }

        #endregion
    }
}