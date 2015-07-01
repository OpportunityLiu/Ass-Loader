using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AssLoader.Collections
{
    public sealed class EventCollection : ObservableCollection<SubEvent>
    {
        internal EventCollection()
            : base()
        {
        }
    }
}
