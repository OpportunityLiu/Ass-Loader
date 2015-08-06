using SubtitleEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SubtitleEditor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    sealed partial class ScriptInfoPage : Page
    {
        public ScriptInfoPage()
        {
            this.InitializeComponent();
            this.SizeChanged += ScriptInfoPage_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InputPane.GetForCurrentView().Showing += ScriptInfoPage_Showing;
            this.ViewModel = ViewModelLocator.GetForCurrentView().ScriptInfoView;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= ScriptInfoPage_Showing;
            this.ViewModel = null;
            base.OnNavigatedFrom(e);
        }

        private bool needFocus;

        private void ScriptInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(needFocus && focus != null)
            {
                root.ScrollIntoView(focus);
            }
            needFocus = false;
        }

        private void ScriptInfoPage_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            needFocus = true;
        }

        private object focus;

        public ScriptInfoViewModel ViewModel
        {
            get
            {
                return (ScriptInfoViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ScriptInfoViewModel), typeof(ScriptInfoPage), new PropertyMetadata(null));

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = sender;
        }
    }
}
