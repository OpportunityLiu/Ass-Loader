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
    sealed partial class StylePage : Page, IGoBack
    {
        public StylePage()
        {
            this.InitializeComponent();
            stackPanelDetail.Visibility = Visibility.Collapsed;
            stackPanelDetail.Opacity = 0;
            var ioc = ViewModelLocator.GetForCurrentView();
            this.ViewModel = ioc.StyleView;
            this.leftWidth = (double)Resources["LeftSubPageWidth"];
            this.onePageMinWidth= (double)Resources["OnePageMinWidth"];
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void buttonBack_Tapped(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public StyleViewModel ViewModel
        {
            get
            {
                return (StyleViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StyleViewModel), typeof(StylePage), new PropertyMetadata(null));

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ViewModel.Delete((AssLoader.Style)((FrameworkElement)sender).DataContext);
        }

        private enum pageState
        {
            unknown,
            lr,
            l,
            r
        }

        private pageState state;

        private readonly double leftWidth, onePageMinWidth;

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width <= onePageMinWidth)
            {
                if(state == pageState.l || state == pageState.r)
                    return;
                state = listView.SelectedItem == null ? pageState.l : pageState.r;
                listView.ClearValue(WidthProperty);
                listView.ClearValue(HorizontalAlignmentProperty);
                scrollViewerDetail.ClearValue(MarginProperty);
                toSubPageAnmation(false);
            }
            else
            {
                if(state==pageState.lr)
                    return;
                state = pageState.lr;
                listView.Width = leftWidth;
                listView.HorizontalAlignment = HorizontalAlignment.Left;
                scrollViewerDetail.Margin = new Thickness(leftWidth, 0, 0, 0);
                toSubPageAnmation(false);
            }
        }

        private void toSubPageAnmation(bool useAnmation)
        {
            listView.Visibility = Visibility.Visible;
            scrollViewerDetail.Visibility = Visibility.Visible;
            switch(state)
            {
            case pageState.lr:
                leftAnimation.To = 0;
                rightAnimation.To = 0;
                showLeft.Begin();
                if(!useAnmation)
                    showLeft.SkipToFill();
                break;
            case pageState.l:
                leftAnimation.To = 0;
                rightAnimation.To = root.ActualWidth;
                showLeft.Begin();
                if(!useAnmation)
                    showLeft.SkipToFill();
                break;
            case pageState.r:
                leftAnimation.To = -root.ActualWidth;
                rightAnimation.To = 0;
                hideLeft.Begin();
                if(!useAnmation)
                    hideLeft.SkipToFill();
                break;
            default:
                break;
            }
            toPage.Begin();
            if(!useAnmation)
                toPage.SkipToFill();
        }

        private bool previousIsNull = true;

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listView.SelectedItem == null && !previousIsNull)
            {
                hideRight.Begin();
                previousIsNull = true;
            }
            else if(previousIsNull)
            {
                stackPanelDetail.Visibility = Visibility.Visible;
                showRight.Begin();
                previousIsNull = false;
            }
            if(state == pageState.lr)
                return;
            else
                state = listView.SelectedItem == null ? pageState.l : pageState.r;
            toSubPageAnmation(true);
        }

        private void toPage_Completed(object sender, object e)
        {
            switch(state)
            {
            case pageState.lr:
                listView.Visibility = Visibility.Visible;
                scrollViewerDetail.Visibility = Visibility.Visible;
                rectangleSplit.Visibility = Visibility.Visible;
                break;
            case pageState.l:
                listView.Visibility = Visibility.Visible;
                scrollViewerDetail.Visibility = Visibility.Collapsed;
                rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            case pageState.r:
                listView.Visibility = Visibility.Collapsed;
                scrollViewerDetail.Visibility = Visibility.Visible;
                rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            default:
                break;
            }
        }

        private void hideRight_Completed(object sender, object e)
        {
            if(ViewModel.SelectedStyle == null)
                stackPanelDetail.Visibility = Visibility.Collapsed;
        }
    }
}
