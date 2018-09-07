using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Opportunity.AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="SubEvent"/>.
    /// </summary>
    public class EventCollection : ObservableCollection<SubEvent>
    {
        /// <summary>
        /// Create new instance of <see cref="EventCollection"/>.
        /// </summary>
        public EventCollection() : base()
        {
        }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by the <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{SubEvent}"/> to compare the <see cref="SubEvent"/> in this <see cref="EventCollection"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <c>null</c>.</exception>
        public void Sort(IComparer<SubEvent> comparer)
        {
            if(comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if(this.Items is List<SubEvent> l)
            {
                l.Sort(comparer);
            }
            else
            {
                var items = this.Items.OrderBy(i => i, comparer).ToList();
                this.Items.Clear();
                foreach(var item in items)
                {
                    this.Items.Add(item);
                }

            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
        }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by the <paramref name="comparison"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{SubEvent}"/> to compare the <see cref="SubEvent"/> in this <see cref="EventCollection"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <c>null</c>.</exception>
        public void Sort(Comparison<SubEvent> comparison)
        {
            if(comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            Sort(Comparer<SubEvent>.Create(comparison));
        }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by <see cref="SubEvent.StartTime"/>.
        /// </summary>
        public void SortByTime()
        {
            Sort((a, b) =>
            {
                var c1 = TimeSpan.Compare(a.StartTime, b.StartTime);
                if(c1 != 0)
                    return c1;
                var c2 = string.Compare(a.Style, b.Style);
                if(c2 != 0)
                    return c2;
                return TimeSpan.Compare(a.EndTime, b.EndTime);
            });
        }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by <see cref="SubEvent.Style"/>.
        /// </summary>
        public void SortByStyle()
        {
            Sort((a, b) =>
            {
                var c2 = string.Compare(a.Style, b.Style);
                if(c2 != 0)
                    return c2;
                var c1 = TimeSpan.Compare(a.StartTime, b.StartTime);
                if(c1 != 0)
                    return c1;
                return TimeSpan.Compare(a.EndTime, b.EndTime);
            });
        }
    }
}
