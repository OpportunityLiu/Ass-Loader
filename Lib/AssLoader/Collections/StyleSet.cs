using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="Style"/>, will delete repeted <see cref="Style"/>s autometically.
    /// </summary>
    public sealed class StyleSet : ObservableCollection<Style>
    {
        /// <summary>
        /// Create new instance of <see cref="StyleSet"/>.
        /// </summary>
        public StyleSet() : base()
        {
        }

        private HashSet<string> styleNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Check whether the <see cref="Style"/> with the <see cref="Style.Name"/> is presented in the <see cref="StyleSet"/>.
        /// </summary>
        /// <param name="name">The <see cref="Style.Name"/> to look up.</param>
        /// <returns>
        /// True if the <see cref="Style"/> is in the <see cref="StyleSet"/>;
        /// false if the <see cref="Style"/> is not in the <see cref="StyleSet"/>;
        /// </returns>
        public bool ContainsName(string name)
        {
            return styleNameSet.Contains(name);
        }

        /// <summary>
        /// Get the index of the <see cref="Style"/> with the <see cref="Style.Name"/>.
        /// </summary>
        /// <param name="name">The <see cref="Style.Name"/> to look up.</param>
        /// <returns>
        /// The index of the <see cref="Style"/> that the <see cref="Style.Name"/> is <paramref name="name"/>, -1 if not found.
        /// </returns>
        public int IndexOf(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            for(int i = 0; i < this.Count; i++)
            {
                if(string.Equals(this[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            styleNameSet.Clear();
        }

        protected override void InsertItem(int index, Style item)
        {
            if(styleNameSet.Add(item.Name))
            {
                base.InsertItem(index, item);
            }
            else
            {
                base.SetItem(IndexOf(item.Name), item);
            }
        }

        protected override void SetItem(int index, Style item)
        {
            styleNameSet.Remove(this[index].Name);
            if(styleNameSet.Add(item.Name))
            {
                base.SetItem(index, item);
            }
            else
            {
                base.RemoveItem(index);
                base.SetItem(IndexOf(item.Name), item);
            }
        }

        protected override void RemoveItem(int index)
        {
            styleNameSet.Remove(this[index].Name);
            base.RemoveItem(index);
        }
    }
}
