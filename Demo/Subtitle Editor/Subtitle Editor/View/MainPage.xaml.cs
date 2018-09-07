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
            this.checkedTab = this.ViewModel.DocumentTabs.First();
        }

        private void MainPage_Loading(FrameworkElement sender, object args)
        {
            this.pane = InputPane.GetForCurrentView();
            this.navigationManager = SystemNavigationManager.GetForCurrentView();
            this.navigationManager.BackRequested += this.backRequested;
            this.pane.Showing += this.inputPaneChanged;
            this.pane.Hiding += this.inputPaneChanged;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.navigationManager.BackRequested -= this.backRequested;
            this.pane.Showing -= this.inputPaneChanged;
            this.pane.Hiding -= this.inputPaneChanged;
        }

        private SystemNavigationManager navigationManager;

        private InputPane pane;

        private void inputPaneChanged(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            args.EnsuredFocusedElementInView = true;
            var height = args.OccludedRect.Height;
            if(height < 1)
            {
                this.inputPane.Visibility = Visibility.Collapsed;
                return;
            }

            this.inputPane.Height = height;
            this.inputPane.Visibility = Visibility.Visible;
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainPage), new PropertyMetadata(null));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.navigating(this.ViewModel.DocumentTabs.First(tab => tab.IsChecked));
        }

        private void SplitViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.commandBar.IsOpen = false;
            var to = !this.splitView.IsPaneOpen;
            this.splitView.IsPaneOpen = to;
            if(to)
            {
                this.splitViewExpand.Begin();
                if(this.splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                    this.splitViewExpand.SkipToFill();
            }
        }

        private void closePaneIfNeeded()
        {
            if(this.splitView.IsPaneOpen == false)
                return;
            if(this.splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                return;
            this.splitView.IsPaneOpen = false;
        }

        private void navigating(SplitViewTabData to)
        {
            if(to == this.checkedTab && this.frameInner.Content != null)
                return;
            this.checkedTab.IsChecked = false;
            this.checkedTab = to;
            this.playNavigationAnimation(true);
            to.IsChecked = true;
            this.frameInner.Navigate(to.PageType);
            ((Page)this.frameInner.Content).Focus(FocusState.Programmatic);
        }

        private void playNavigationAnimation(bool aniamtion)
        {
            var index = this.ViewModel.DocumentTabs.IndexOf(this.checkedTab);
            if(index < 0)
            {
                this.tabTagTransform.To = this.ViewModel.DocumentTabs.Count * 48 + this.splitViewGrid.RowDefinitions[3].ActualHeight + this.splitViewGrid.RowDefinitions[4].ActualHeight;
            }
            else
            {
                this.tabTagTransform.To = index * 48;
            }
            this.Navigating.Begin();
            if(!aniamtion)
                this.Navigating.SkipToFill();
        }

        private SplitViewTabData checkedTab;

        private void SplitViewTabButton_Click(object sender, RoutedEventArgs e)
        {
            this.closePaneIfNeeded();
            var checkedTabTemp = (SplitViewTabData)((FrameworkElement)sender).DataContext;
            this.navigating(checkedTabTemp);
        }

        private void SplitViewCommand_Click(object sender, RoutedEventArgs e)
        {
            this.closePaneIfNeeded();
        }

        private void splitView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width < 720)
            {
                this.splitView.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                if(e.NewSize.Width < 1280)
                {
                    this.splitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                }
                else
                {
                    this.splitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                }
            }
        }

        private void splitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            this.splitViewClose.Begin();
            if(this.splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                this.splitViewClose.SkipToFill();
        }

        private void splitViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.PreviousSize.Height != e.NewSize.Height)
                this.playNavigationAnimation(false);
        }

        private void frameInner_Navigated(object sender, NavigationEventArgs e)
        {
            if(this.innerContent != null)
            {
                this.innerContent.CanGoBackChanged -= this.InnerContent_CanGoBackChanged;
            }
            this.innerContent = e.Content as IGoBack;
            if(this.innerContent != null)
            {
                this.innerContent.CanGoBackChanged += this.InnerContent_CanGoBackChanged;
            }
        }

        private IGoBack innerContent;

        private void InnerContent_CanGoBackChanged(IGoBack sender, EventArgs e)
        {
            this.navigationManager.AppViewBackButtonVisibility = sender.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void backRequested(object sender, BackRequestedEventArgs e)
        {
            if(this.innerContent?.CanGoBack == true)
            {
                this.innerContent.GoBack();
                e.Handled = true;
            }
        }
    }
}
