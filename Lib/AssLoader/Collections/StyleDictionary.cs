using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AssLoader.Collections
{
    /// <summary>
    /// Observable dictionary of <see cref="Style"/>.
    /// </summary>
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

        /// <summary>
        /// Add an item to the <see cref="StyleDictionary"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="StyleDictionary"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public void Add(Style item)
        {
            ThrowHelper.ThrowIfNull(item, "item");
            add(item);
        }

        /// <summary>
        /// Clear all the items in the <see cref="StyleDictionary"/>.
        /// </summary>
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

        void  ICollection<Style>.CopyTo(Style[] array, int arrayIndex)
        {
            inner.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Number of items in the <see cref="StyleDictionary"/>.
        /// </summary>
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

        /// <summary>
        /// Removes a particular item from the <see cref="StyleDictionary"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item removed successfully.</returns>
        public bool Remove(Style item)
        {
            ThrowHelper.ThrowIfNull(item, "item");
            if(inner.Values.Contains(item))
                return Remove(item.Name);
            return false;
        }

        #endregion

        #region IEnumerable<Style> 成员

        /// <summary>
        /// Returns an <see cref="IEnumerator{Style}"/> for this <see cref="StyleDictionary"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{Style}"/> for this <see cref="StyleDictionary"/>.</returns>
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

        /// <summary>
        /// Add an item to the <see cref="StyleDictionary"/>.
        /// </summary>
        /// <param name="key">The key of the <paramref name="value"/> to add, 
        /// if not equals to <see cref="Style.Name"/> of <paramref name="value"/>, 
        /// a renamed clone of <paramref name="value"/> will be added.</param>
        /// <param name="value">The item to add to the <see cref="StyleDictionary"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public void Add(string key, Style value)
        {
            ThrowHelper.ThrowIfNullOrEmpty(key, "key");
            ThrowHelper.ThrowIfNull(value, "value");
            add(key, value);
        }

        /// <summary>
        /// Returns whatever a <see cref="Style"/> of a particular <see cref="Style.Name"/> is in this <see cref="StyleDictionary"/>.
        /// </summary>
        /// <param name="key">The name to test.</param>
        /// <returns>True if <paramref name="key"/> founded in the <see cref="StyleDictionary"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        public bool ContainsKey(string key)
        {
            return inner.ContainsKey(key);
        }

        /// <summary>
        /// The collection of <see cref="Style.Name"/> in the <see cref="StyleDictionary"/>.
        /// </summary>
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

        /// <summary>
        /// Removes a particular key from the <see cref="StyleDictionary"/>.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the key removed successfully.</returns>
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

        /// <summary>
        /// Get the <see cref="Style"/> with the <see cref="Style.Name"/>.
        /// </summary>
        /// <param name="key">The <see cref="Style.Name"/> of the <see cref="Style"/>.</param>
        /// <param name="value">Returns the <see cref="Style"/> with the <see cref="Style.Name"/> if founded, 
        /// otherwise <c>null</c> will be returned.</param>
        /// <returns>Whatever the <see cref="Style"/> with the <see cref="Style.Name"/> is founded in the <see cref="StyleDictionary"/>.</returns>
        public bool TryGetValue(string key, out Style value)
        {
            return inner.TryGetValue(key, out value);
        }

        /// <summary>
        /// The collection of <see cref="Style"/> in the <see cref="StyleDictionary"/>.
        /// </summary>
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

        /// <summary>
        /// Get or set the <see cref="Style"/> with the <see cref="Style.Name"/> of <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <see cref="Style.Name"/> of <see cref="Style"/>.</param>
        /// <returns>The <see cref="Style"/> with the <see cref="Style.Name"/> of <paramref name="key"/> in the <see cref="StyleDictionary"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="key"/> is null.</exception>
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

        /// <summary>
        /// Occurs when the collection changes, either by adding or removing an item.
        /// </summary>
        /// <remarks>
        /// The event handler receives an argument of type
        /// <seealso cref="NotifyCollectionChangedEventArgs" />
        /// containing data related to this event.
        /// </remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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

        private void countChanging()
        {
            if(PropertyChanged != null)
                PropertyChanged(this, countChanged);
        }

        private static readonly PropertyChangedEventArgs countChanged = new PropertyChangedEventArgs("Count");

        #endregion
    }
}
