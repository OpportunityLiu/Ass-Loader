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
using SubtitleEditor.Converters;
using AssLoader;

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
            var ioc = ViewModelLocator.GetForCurrentView();
            this.ViewModel = ioc.ScriptInfoView;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Showing += paneShowing;
        }

        private void page_Unloaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= paneShowing;
        }

        private bool needFocus;

        private void ScriptInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(!needFocus || focus == null)
                return;
            var max = 12.0;
            FrameworkElement found = null;
            foreach(FrameworkElement item in stackPanelMetaData.Children)
            {
                if(item == focus)
                {
                    found = item;
                    break;
                }
                max += item.ActualHeight + 16.0;
            }
            if(found == null)
            {
                if(e.NewSize.Width >= 1096)
                    max = 12.0;
                else
                    max += 24.0;
                foreach(FrameworkElement item in stackPanelScriptData.Children)
                {
                    if(item == focus)
                    {
                        found = item;
                        break;
                    }
                    max += item.ActualHeight + 16.0;
                }
            }
            if(found != null)
            {
                var min = max - e.NewSize.Height + found.ActualHeight + 16.0;
                var current = root.VerticalOffset;
                if(current < min)
                    root.ChangeView(null, min, null);
                else if(current > max)
                    root.ChangeView(null, max, null);
            }
            needFocus = false;
        }

        private void paneShowing(InputPane sender, InputPaneVisibilityEventArgs args)
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

        private void field_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = sender;
        }

        private void field_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!needFocus)
                focus = null;
        }

        private void numberedTextBox_LostFocus(object sender, RoutedEventArgs args)
        {
            var s = (TextBox)sender;
            var num = string.Concat(s.Text.TakeWhile(c => c != '.').Where(c => c >= '0' && c <= '9')).TrimStart('0');
            s.Text = string.IsNullOrEmpty(num) ? "0" : num;
        }
    }

    class WrapStyleConverter : EnumConverter<WrapStyle>
    {
        protected override Dictionary<WrapStyle, object> ConvertDictionary
        {
            get;
        } = new Dictionary<WrapStyle, object>()
        {
            [WrapStyle.Smart] = LocalizedStrings.Resources.WrapStyleSmart,
            [WrapStyle.None] = LocalizedStrings.Resources.WrapStyleNone,
            [WrapStyle.EndOfLine] = LocalizedStrings.Resources.WrapStyleEndOfLine,
            [WrapStyle.Smart2] = LocalizedStrings.Resources.WrapStyleSmart2
        };
    }

    class CollisionStyleConverter : EnumConverter<CollisionStyle>
    {
        protected override Dictionary<CollisionStyle, object> ConvertDictionary
        {
            get;
        } = new Dictionary<CollisionStyle, object>()
        {
            [CollisionStyle.Normal] = LocalizedStrings.Resources.CollisionStyleNormal,
            [CollisionStyle.Reverse] = LocalizedStrings.Resources.CollisionStyleReverse
        };
    }
}
