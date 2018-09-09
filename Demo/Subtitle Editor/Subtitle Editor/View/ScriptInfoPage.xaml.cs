using Opportunity.AssLoader;
using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Views;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SubtitleEditor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    internal sealed partial class ScriptInfoPage : MvvmPage
    {
        public ScriptInfoPage()
        {
            this.ViewModel = ThreadLocalSingleton.GetOrCreate<ScriptInfoViewModel>();
            this.InitializeComponent();
            this.cbCollisions.ItemsSource = Enum.GetValues(typeof(CollisionStyle));
            this.cbWrapStyle.ItemsSource = Enum.GetValues(typeof(WrapStyle));
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
            InputPane.GetForCurrentView().Showing += this.paneShowing;
        }

        private void page_Unloaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= this.paneShowing;
        }

        private bool needFocus;

        private void ScriptInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.needFocus || this.focus == null)
                return;
            var max = 12.0;
            FrameworkElement found = null;
            foreach (FrameworkElement item in this.stackPanelMetaData.Children)
            {
                if (item == this.focus)
                {
                    found = item;
                    break;
                }
                max += item.ActualHeight + 16.0;
            }
            if (found == null)
            {
                if (e.NewSize.Width >= 1096)
                    max = 12.0;
                else
                    max += 24.0;
                foreach (FrameworkElement item in this.stackPanelScriptData.Children)
                {
                    if (item == this.focus)
                    {
                        found = item;
                        break;
                    }
                    max += item.ActualHeight + 16.0;
                }
            }
            if (found != null)
            {
                var min = max - e.NewSize.Height + found.ActualHeight + 16.0;
                var current = this.root.VerticalOffset;
                if (current < min)
                    this.root.ChangeView(null, min, null);
                else if (current > max)
                    this.root.ChangeView(null, max, null);
            }
            this.needFocus = false;
        }

        private void paneShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            this.needFocus = true;
        }

        private object focus;

        public new ScriptInfoViewModel ViewModel
        {
            get => (ScriptInfoViewModel)base.ViewModel;
            set => base.ViewModel = value;
        }

        private void field_GotFocus(object sender, RoutedEventArgs e)
        {
            this.focus = sender;
        }

        private void field_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.needFocus)
                this.focus = null;
        }

        private void numberedTextBox_LostFocus(object sender, RoutedEventArgs args)
        {
            var s = (TextBox)sender;
            var num = string.Concat(s.Text.TakeWhile(c => c != '.').Where(c => c >= '0' && c <= '9')).TrimStart('0');
            s.Text = string.IsNullOrEmpty(num) ? "0" : num;
        }
    }
}
