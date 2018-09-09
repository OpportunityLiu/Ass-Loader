using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Views;
using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubtitleEditor.ViewModel
{
    internal class PreferencesViewModel : ViewModelBase
    {
        public PreferencesViewModel()
        {
        }

        public Preferences Data { get; } = Singleton.GetOrCreate<Preferences>();
    }
}