using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AssLoader.Collections
{
    public sealed class StyleDictionary : IDictionary<string, Style>, ICollection<Style>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        internal StyleDictionary()
        {
        }

        private Dictionary<string, Style> inner = new Dictionary<string, Style>(StringComparer.OrdinalIgnoreCase);

        private void add(Style item)
        {
            inner.Add(item.Name, item);
            if(CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            countChanging();
        }

        private void add(string key, Style item)
        {
            if(!string.Equals(key, item.Name, StringComparison.OrdinalIgnoreCase))
                item = item.Clone(key);
            add(item);
        }

        #region ICollection<Style> 成员

        public void Add(Style item)
        {
            ThrowHelper.ThrowIfNull(item, "item");
            add(item);
        }

        public void Clear()
        {
            inner.Clear();
            if(CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            countChanging();
        }

        bool ICollection<Style>.Contains(Style item)
        {
            return inner.Values.Contains(item);
        }

        public void CopyTo(Style[] array, int arrayIndex)
        {
            inner.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        bool ICollection<Style>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(Style item)
        {
            ThrowHelper.ThrowIfNull(item, "item");
            if(inner.Values.Contains(item))
                return Remove(item.Name);
            return false;
        }

        #endregion

        #region IEnumerable<Style> 成员

        public Dictionary<string, Style>.ValueCollection.Enumerator GetEnumerator()
        {
            return inner.Values.GetEnumerator();
        }

        IEnumerator<Style> IEnumerable<Style>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDictionary<string,Style> 成员

        public void Add(string key, Style value)
        {
            ThrowHelper.ThrowIfNullOrEmpty(key, "key");
            ThrowHelper.ThrowIfNull(value, "value");
            add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return inner.ContainsKey(key);
        }

        public Dictionary<string, Style>.KeyCollection Keys
        {
            get
            {
                return inner.Keys;
            }
        }

        ICollection<string> IDictionary<string, Style>.Keys
        {
            get
            {
                return Keys;
            }
        }

        public bool Remove(string key)
        {
            Style toRemove;
            if(!inner.TryGetValue(key, out toRemove))
                return false;
            inner.Remove(key);
            if(CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, toRemove));
            countChanging();
            return true;
        }

        public bool TryGetValue(string key, out Style value)
        {
            return inner.TryGetValue(key, out value);
        }

        public Dictionary<string, Style>.ValueCollection Values
        {
            get
            {
                return inner.Values;
            }
        }

        ICollection<Style> IDictionary<string, Style>.Values
        {
            get
            {
                return Values;
            }
        }

        public Style this[string key]
        {
            get
            {
                return inner[key];
            }
            set
            {
                ThrowHelper.ThrowIfNull(value, "value");
                ThrowHelper.ThrowIfNullOrEmpty(key, "key");
                if(!string.Equals(key, value.Name, StringComparison.OrdinalIgnoreCase))
                    value = value.Clone(key);
                Style old;
                if(TryGetValue(key, out old))
                {
                    inner[key] = value;
                    if(CollectionChanged != null)
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old));
                }
                else
                    add(key, value);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,Style>> 成员

        void ICollection<KeyValuePair<string, Style>>.Add(KeyValuePair<string, Style> item)
        {
            ThrowHelper.ThrowIfNull(item.Value, "item.Value");
            ThrowHelper.ThrowIfNullOrEmpty(item.Key, "item.Key");
            add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, Style>>.Contains(KeyValuePair<string, Style> item)
        {
            ThrowHelper.ThrowIfNull(item.Value, "item.Value");
            return ContainsKey(item.Value.Name);
        }

        void ICollection<KeyValuePair<string, Style>>.CopyTo(KeyValuePair<string, Style>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Style>>)inner).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, Style>>.Remove(KeyValuePair<string, Style> item)
        {
            return Remove(item.Value);
        }

        bool ICollection<KeyValuePair<string, Style>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,Style>> 成员

        IEnumerator<KeyValuePair<string, Style>> IEnumerable<KeyValuePair<string, Style>>.GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged 成员

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        private void countChanging()
        {
            if(PropertyChanged != null)
                PropertyChanged(this, countChanged);
        }

        private static readonly PropertyChangedEventArgs countChanged = new PropertyChangedEventArgs("Count");

        #endregion
    }
}
