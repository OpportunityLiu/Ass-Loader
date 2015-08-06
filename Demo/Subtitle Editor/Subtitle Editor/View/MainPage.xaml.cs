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
            var pane = InputPane.GetForCurrentView();
            pane.Showing += inputPaneChanged;
            pane.Hiding += inputPaneChanged;
        }

        private void inputPaneChanged(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            args.EnsuredFocusedElementInView = true;
            var height = args.OccludedRect.Height;
            if(height < 1)
            {
                inputPane.Visibility = Visibility.Collapsed;
                return;
            }

            inputPane.Height = args.OccludedRect.Height;
            if(height < 32)
                inputPane.Content = null;
            else
            {
                inputPane.Content = "\xE087"; //keyboard icon
                if(height < 100)
                    //snap to 4 px grid
                    inputPane.FontSize = ((int)height / 8) * 4;
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
            setPane(!splitView.IsPaneOpen);
        }

        private void setPane(bool isOpen)
        {
            if(splitView.IsPaneOpen == isOpen)
                return;
            splitView.IsPaneOpen = isOpen;
            if(isOpen)
                splitViewExpand.Begin();
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
            if(aniamtion)
                Navigating.Begin();
            else
                Navigating.SkipToFill();
        }

        private SplitViewTabData checkedTab;

        private void SplitViewTabButton_Click(object sender, RoutedEventArgs e)
        {
            setPane(false);
            if(checkedTab == ViewModel.Preferences)
                ViewModel.IsPreferencesShown = false;
            var checkedTabTemp = (SplitViewTabData)((FrameworkElement)sender).DataContext;
            navigating(checkedTabTemp);
        }

        private async void SplitViewTabButton_Click2(object sender, RoutedEventArgs e)
        {
            setPane(false);
            if(await ViewModel.ShowPreferences())
                return;
            var checkedTabTemp = ViewModel.Preferences;
            ViewModel.IsPreferencesShown = true;
            navigating(checkedTabTemp);
        }

        private void SplitViewCommand_Click(object sender, RoutedEventArgs e)
        {
            setPane(false);
        }

        private void page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            playNavigationAnimation(false);
        }

        private void splitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            splitViewClose.Begin();
        }
    }
}
