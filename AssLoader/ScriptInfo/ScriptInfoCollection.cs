using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.ObjectModel;
using AssLoader.Serializer;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AssLoader
{
    public abstract class ScriptInfoCollection : IDictionary<string, string>, INotifyPropertyChanged
    {
        protected ScriptInfoCollection()
        {
            var type = this.GetType();
            if(scriptInfoCache.ContainsKey(type))
                scriptInfoFields = scriptInfoCache[type];
            else
            {
                scriptInfoFields = new Dictionary<string, ScriptInfoSerializeHelper>(StringComparer.OrdinalIgnoreCase);
                do
                {
                    foreach(var fInfo in type.GetRuntimeFields())
                    {
                        var att = fInfo.GetCustomAttribute(typeof(ScriptInfoAttribute)) as ScriptInfoAttribute;
                        if(att == null)
                            continue;
                        var ser = (SerializeAttribute)fInfo.GetCustomAttribute<SerializeAttribute>();
                        scriptInfoFields.Add(att.FieldName, new ScriptInfoSerializeHelper(fInfo, att, ser));
                    }
                } while((type = type.GetTypeInfo().BaseType) != typeof(ScriptInfoCollection));
                scriptInfoCache.Add(this.GetType(), scriptInfoFields);
            }
            this.UndefinedFields = new ReadOnlyDictionary<string, string>(this.undefinedFields);
        }

        internal void ParseLine(string value)
        {
            string k, y;
            if(FormatHelper.TryPraseLine(out k, out y, value))
            {
                ScriptInfoSerializeHelper helper;
                if(scriptInfoFields.TryGetValue(k, out helper))
                    helper.Deserialize(this, y);
                else
                    undefinedFields[k] = y;
            }
        }

        public void Serialize(TextWriter writer)
        {
            ThrowHelper.ThrowIfNull(writer, "writer");
            foreach(var item in scriptInfoFields.Values)
            {
                var toWrite = item.Serialize(this);
                if(toWrite != null)
                    writer.WriteLine(toWrite);
            }

            //unknown script info entries.
            writer.WriteLine();
            foreach(var item in undefinedFields)
                writer.WriteLine("{0}: {1}", item.Key, item.Value);
        }

        private static Dictionary<Type, Dictionary<string, ScriptInfoSerializeHelper>> scriptInfoCache = new Dictionary<Type, Dictionary<string, ScriptInfoSerializeHelper>>();

        private Dictionary<string, ScriptInfoSerializeHelper> scriptInfoFields;

        private Dictionary<string, string> undefinedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, string> UndefinedFields
        {
            get;
            private set;
        }

        public void Add(string key, string value)
        {
            if(this.ContainsKey(key))
                throw new ArgumentException("Key contains in the collection.");
            this.undefinedFields[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return undefinedFields.ContainsKey(key) || scriptInfoFields.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return undefinedFields.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            string s;
            if(undefinedFields.TryGetValue(key, out s))
            {
                value = s;
                return true;
            }
            ScriptInfoSerializeHelper ssi;
            if(scriptInfoFields.TryGetValue(key, out ssi))
            {
                value = ssi.Serialize(this);
                return true;
            }
            value = null;
            return false;
        }

        public string this[string key]
        {
            get
            {
                string va;
                if(TryGetValue(key, out va))
                    return va;
                throw new KeyNotFoundException(key + " not found.");
            }
            set
            {
                if(scriptInfoFields.ContainsKey(key))
                    throw new InvalidOperationException("The key has defined, please use property.");
                else
                    undefinedFields[key] = value;
            }
        }

        public void Clear()
        {
            undefinedFields.Clear();
        }

        #region IDictionary<string,string> 成员

        ICollection<string> IDictionary<string, string>.Keys
        {
            get
            {
                if(keys == null)
                    return keys = new Collection(this, true);
                return keys;
            }
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get
            {
                if(values == null)
                    return values = new Collection(this, false);
                return values;
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
                if(isKey)
                    return collection.ContainsKey(item);
                else
                    return collection.Any(i => i.Value == item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                if(isKey)
                    collection.scriptInfoFields.Keys
                        .Concat(collection.undefinedFields.Keys)
                        .ToArray().CopyTo(array, arrayIndex);
                else
                    collection.scriptInfoFields.Values
                        .Select(item => item.Serialize(this))
                        .Concat(collection.undefinedFields.Values)
                        .ToArray().CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get
                {
                    return ((ICollection<KeyValuePair<string, string>>)collection).Count;
                }
            }

            bool ICollection<string>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection<string>.Remove(string item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                if(isKey)
                    return collection.scriptInfoFields.Keys
                        .Concat(collection.undefinedFields.Keys).GetEnumerator();
                else
                    return collection.scriptInfoFields.Values
                        .Select(item => item.Serialize(this))
                        .Concat(collection.undefinedFields.Values).GetEnumerator();
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
                .Concat(undefinedFields).ToArray().CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, string>>.Count
        {
            get
            {
                return scriptInfoFields.Count + undefinedFields.Count;
            }
        }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if(undefinedFields.ContainsKey(item.Key))
                return undefinedFields.Remove(item.Key);
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> 成员

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return this.scriptInfoFields
                 .Select(item => new KeyValuePair<string, string>(item.Key, item.Value.Serialize(this)))
                 .Concat(undefinedFields).GetEnumerator();
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
        protected virtual void PropertyChanging([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}