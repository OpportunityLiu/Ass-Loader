using SubtitleEditor.Converters;
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
    sealed partial class PreferencesPage : Page
    {
        public PreferencesPage()
        {
            this.InitializeComponent();
            var ioc = ViewModelLocator.GetForCurrentView();
            this.ViewModel = ioc.PreferencesView;
        }

        public PreferencesViewModel ViewModel
        {
            get
            {
                return (PreferencesViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Bindings.Initialize();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Bindings.StopTracking();
            base.OnNavigatingFrom(e);
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PreferencesViewModel), typeof(PreferencesPage), new PropertyMetadata(null));
    }

    class ElementThemeConverter : EnumConverter<ElementTheme>
    {
        protected override Dictionary<ElementTheme, object> ConvertDictionary
        {
            get;
        } = new Dictionary<ElementTheme, object>()
        {
            [ElementTheme.Dark] = LocalizedStrings.Resources.ElementThemeDark,
            [ElementTheme.Light] = LocalizedStrings.Resources.ElementThemeLight
        };
    }
}
