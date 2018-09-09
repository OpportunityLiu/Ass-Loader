using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Views;
using SubtitleEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SubtitleEditor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    internal sealed partial class PreferencesPage : MvvmPage
    {
        public PreferencesPage()
        {
            this.ViewModel = ThreadLocalSingleton.GetOrCreate<PreferencesViewModel>();
            this.InitializeComponent();
            this.cbTheme.ItemsSource = new[] { ElementTheme.Dark, ElementTheme.Light };
        }

        public new PreferencesViewModel ViewModel
        {
            get => (PreferencesViewModel)base.ViewModel;
            set => base.ViewModel = value;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Bindings.Initialize();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.Bindings.StopTracking();
            base.OnNavigatingFrom(e);
        }
    }
}
