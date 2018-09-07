using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Opportunity.AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="Style"/>, will delete repeted <see cref="Style"/>s autometically.
    /// </summary>
    public class StyleSet : ObservableCollection<Style>
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
            if(FormatHelper.FieldStringValueValid(ref name))
            {
                return this.styleNameSet.Contains(name);
            }
            else
                throw new ArgumentNullException(nameof(name));
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
            if(!FormatHelper.FieldStringValueValid(ref name))
                throw new ArgumentNullException(nameof(name));
            for(var i = 0; i < this.Count; i++)
            {
                if(string.Equals(this[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Delete all items in the <see cref="StyleSet"/>.
        /// </summary>
        protected sealed override void ClearItems()
        {
            base.ClearItems();
            this.styleNameSet.Clear();
        }

        /// <summary>
        /// Insert items to the <see cref="StyleSet"/>.
        /// </summary>
        /// <param name="index">The index of the inserting item.</param>
        /// <param name="item">The <see cref="Style"/> to insert.</param>
        protected sealed override void InsertItem(int index, Style item)
        {
            if(this.styleNameSet.Add(item.Name))
            {
                base.InsertItem(index, item);
            }
            else
            {
                base.SetItem(IndexOf(item.Name), item);
            }
        }

        /// <summary>
        /// Set the item of the given index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="item">The new value of the item.</param>
        protected sealed override void SetItem(int index, Style item)
        {
            this.styleNameSet.Remove(this[index].Name);
            if(this.styleNameSet.Add(item.Name))
            {
                base.SetItem(index, item);
            }
            else
            {
                base.RemoveItem(index);
                base.SetItem(IndexOf(item.Name), item);
            }
        }

        /// <summary>
        /// Remove the item at the index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        protected sealed override void RemoveItem(int index)
        {
            this.styleNameSet.Remove(this[index].Name);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Move the item to a new index.
        /// </summary>
        /// <param name="oldIndex">The index of the item to move.</param>
        /// <param name="newIndex">The new index of the item.</param>
        protected sealed override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
        }
    }
}
