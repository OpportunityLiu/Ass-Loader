using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="SubEvent"/>.
    /// </summary>
    public class EventCollection : List<SubEvent>
    {
        /// <summary>
        /// Create new instance of <see cref="EventCollection"/>.
        /// </summary>
        public EventCollection() : base() { }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by <see cref="SubEvent.StartTime"/>.
        /// </summary>
        public void SortByTime()
        {
            this.Sort((a, b) =>
            {
                var c1 = TimeSpan.Compare(a.StartTime, b.StartTime);
                if (c1 != 0)
                    return c1;
                var c2 = string.Compare(a.Style, b.Style);
                if (c2 != 0)
                    return c2;
                return TimeSpan.Compare(a.EndTime, b.EndTime);
            });
        }

        /// <summary>
        /// Reorder the items in this <see cref="EventCollection"/> by <see cref="SubEvent.Style"/>.
        /// </summary>
        public void SortByStyle()
        {
            this.Sort((a, b) =>
            {
                var c2 = string.Compare(a.Style, b.Style);
                if (c2 != 0)
                    return c2;
                var c1 = TimeSpan.Compare(a.StartTime, b.StartTime);
                if (c1 != 0)
                    return c1;
                return TimeSpan.Compare(a.EndTime, b.EndTime);
            });
        }
    }
}
