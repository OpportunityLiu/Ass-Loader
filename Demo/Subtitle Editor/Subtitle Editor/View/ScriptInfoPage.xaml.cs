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
            InputPane.GetForCurrentView().Showing += ScriptInfoPage_Showing;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= ScriptInfoPage_Showing;
            base.OnNavigatedFrom(e);
        }

        private bool needFocus, isNarrow;

        private void ScriptInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width > 1100)
            {
                if(isNarrow)
                {
                    isNarrow = false;
                    stackPanelScriptData.SetValue(RelativePanel.RightOfProperty, stackPanelMetaData);
                    stackPanelScriptData.ClearValue(RelativePanel.BelowProperty);
                }
            }
            else
            {
                if(!isNarrow)
                {
                    isNarrow = true;
                    stackPanelScriptData.SetValue(RelativePanel.BelowProperty, stackPanelMetaData);
                    stackPanelScriptData.ClearValue(RelativePanel.RightOfProperty);
                }
            }

            if(needFocus && focus != null)
            {
                var max = -48.0;
                var found = false;
                foreach(FrameworkElement item in stackPanelMetaData.Children)
                {
                    max += item.ActualHeight + 16.0;
                    if(item == focus)
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    if(!isNarrow)
                        max = -48.0;
                    else
                        max += 48.0;
                    foreach(FrameworkElement item in stackPanelScriptData.Children)
                    {
                        max += item.ActualHeight + 16.0;
                        if(item == focus)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                var min = max - e.NewSize.Height + 120.0;
                var current = root.VerticalOffset;
                if(current < min)
                    root.ChangeView(null, min, null);
                else if(current > max)
                    root.ChangeView(null, max, null);
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

        private void field_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = sender;
        }

        private void numberedTextBox_LostFocus(object sender, RoutedEventArgs args)
        {
            var s = (TextBox)sender;
            var num = string.Concat(s.Text.Where(c => c >= '0' && c <= '9')).TrimStart('0');
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
            [WrapStyle.Smart] = LocalizedStrings.WrapStyleSmart,
            [WrapStyle.None] = LocalizedStrings.WrapStyleNone,
            [WrapStyle.EndOfLine] = LocalizedStrings.WrapStyleEndOfLine,
            [WrapStyle.Smart2] = LocalizedStrings.WrapStyleSmart2
        };
    }

    class CollisionStyleConverter : EnumConverter<CollisionStyle>
    {
        protected override Dictionary<CollisionStyle, object> ConvertDictionary
        {
            get;
        } = new Dictionary<CollisionStyle, object>()
        {
            [CollisionStyle.Normal] = LocalizedStrings.CollisionStyleNormal,
            [CollisionStyle.Reverse] = LocalizedStrings.CollisionStyleReverse
        };
    }
}
