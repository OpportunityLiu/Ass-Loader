using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AssLoader.Collections
{
    /// <summary>
    /// Observable collection of <see cref="SubEvent"/>.
    /// </summary>
    public sealed class EventCollection : ObservableCollection<SubEvent>
    {
        internal EventCollection()
            : base()
        {
        }
    }
}
