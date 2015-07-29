using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="SubEvent"/>.
    /// </summary>
    public sealed class EventCollection : ObservableCollection<SubEvent>
    {
        /// <summary>
        /// Create new instance of <see cref="EventCollection"/>.
        /// </summary>
        public EventCollection() : base()
        {
        }

        public void SortByTime()
        {
            var result = (from item in this
                          orderby item.StartTime, item.Style, item.EndTime
                          select item).ToArray();
            sort(result);
        }

        public void SortByStyle()
        {
            var result = (from item in this
                          orderby item.Style, item.StartTime, item.EndTime
                          select item).ToArray();
            sort(result);
        }

        private void sort(IEnumerable<SubEvent> sortedItems)
        {
            sorting = true;
            this.ClearItems();
            foreach(var item in sortedItems)
            {
                this.Add(item);
            }
            sorting = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }

        private bool sorting = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Pause notification
            if(sorting)
                return;
            base.OnCollectionChanged(e);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            // Pause notification
            if(sorting)
                return;
            base.OnPropertyChanged(e);
        }
    }
}
