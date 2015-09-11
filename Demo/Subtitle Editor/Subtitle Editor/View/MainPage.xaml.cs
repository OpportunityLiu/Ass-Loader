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
using SubtitleEditor.ViewModel;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using SubtitleEditor.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace SubtitleEditor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = ViewModelLocator.GetForCurrentView().MainView;
            this.checkedTab = ViewModel.DocumentTabs.First();
        }

        private void MainPage_Loading(FrameworkElement sender, object args)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += backRequested;
            pane = InputPane.GetForCurrentView();
            pane.Showing += inputPaneChanged;
            pane.Hiding += inputPaneChanged;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= backRequested;
            pane.Showing -= inputPaneChanged;
            pane.Hiding -= inputPaneChanged;
        }

        private async void backRequested(object sender, BackRequestedEventArgs e)
        {
            if(e.Handled)
                return;
            var inner = frameInner.Content as IGoBack;
            if(inner != null)
            {
                e.Handled = inner.GoBack();
            }
            if(e.Handled)
                return;
            if(e.Handled = cleaning)
                return;
            if(e.Handled = ViewModel.NeedCleanUp)
            {
                cleaning = true;
                await ViewModel.CleanUpBeforeNewOrOpen();
                cleaning = false;
            }
        }

        private bool cleaning = false;

        private InputPane pane;

        private static string deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

        private void inputPaneChanged(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            args.EnsuredFocusedElementInView = true;
            var height = args.OccludedRect.Height;
            if(deviceFamily == "Windows.Mobile")
            {
                height -= 48;
            }
            if(height < 1)
            {
                inputPane.Visibility = Visibility.Collapsed;
                return;
            }

            inputPane.Height = height;
            if(height < 32)
                inputPane.Content = null;
            else
            {
                inputPane.Content = "\xE087"; //keyboard icon
                if(height < 100)
                    inputPane.FontSize = height * 0.8;
                else
                    inputPane.FontSize = 64;
            }
            inputPane.Visibility = Visibility.Visible;
        }

        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainPage), new PropertyMetadata(null));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            navigating(ViewModel.DocumentTabs.First(tab => tab.IsChecked));
        }

        private void SplitViewButton_Click(object sender, RoutedEventArgs e)
        {
            commandBar.IsOpen = false;
            var to = !splitView.IsPaneOpen;
            splitView.IsPaneOpen = to;
            if(to)
            {
                splitViewExpand.Begin();
                if(splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                    splitViewExpand.SkipToFill();
            }
        }

        private void closePaneIfNeeded()
        {
            if(splitView.IsPaneOpen == false)
                return;
            if(splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                return;
            splitView.IsPaneOpen = false;
        }

        private void navigating(SplitViewTabData to)
        {
            if(to == checkedTab && frameInner.Content != null)
                return;
            checkedTab.IsChecked = false;
            checkedTab = to;
            playNavigationAnimation(true);
            to.IsChecked = true;
            frameInner.Navigate(to.PageType);
            ((Page)this.frameInner.Content).Focus(FocusState.Programmatic);
        }

        private void playNavigationAnimation(bool aniamtion)
        {
            var index = ViewModel.DocumentTabs.IndexOf(checkedTab);
            if(index < 0)
            {
                tabTagTransform.To = ViewModel.DocumentTabs.Count * 48 + splitViewGrid.RowDefinitions[3].ActualHeight + splitViewGrid.RowDefinitions[4].ActualHeight;
            }
            else
            {
                tabTagTransform.To = index * 48;
            }
            Navigating.Begin();
            if(!aniamtion)
                Navigating.SkipToFill();
        }

        private SplitViewTabData checkedTab;

        private void SplitViewTabButton_Click(object sender, RoutedEventArgs e)
        {
            closePaneIfNeeded();
            var checkedTabTemp = (SplitViewTabData)((FrameworkElement)sender).DataContext;
            navigating(checkedTabTemp);
        }

        private void SplitViewCommand_Click(object sender, RoutedEventArgs e)
        {
            closePaneIfNeeded();
        }

        private void splitView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width < 720)
            {
                splitView.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                if(e.NewSize.Width < 1280)
                {
                    splitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                }
                else
                {
                    splitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                }
            }
        }

        private void splitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            splitViewClose.Begin();
            if(splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                splitViewClose.SkipToFill();
        }

        private void splitViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.PreviousSize.Height != e.NewSize.Height)
                playNavigationAnimation(false);
        }

        private void inputPane_Tapped(object sender, TappedRoutedEventArgs e)
        {
            pane.TryHide();
        }
    }
}
