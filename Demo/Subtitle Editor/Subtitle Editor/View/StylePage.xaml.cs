using SubtitleEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
            gridDetail.Visibility = Visibility.Collapsed;
            this.ViewModel = ViewModelLocator.GetForCurrentView().StyleView;
            gridDetail.Opacity = 0;
            this.leftWidth = (double)Resources["LeftSubPageWidth"];
            this.onePageMinWidth = (double)Resources["OnePageMinWidth"];
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
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

        public bool CanGoBack => state == pageState.r;

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StyleViewModel), typeof(StylePage), new PropertyMetadata(null));

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
                borderDetail.ClearValue(MarginProperty);
                toSubPageAnmation(false);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if(state == pageState.lr)
                    return;
                state = pageState.lr;
                listView.Width = leftWidth;
                listView.HorizontalAlignment = HorizontalAlignment.Left;
                borderDetail.Margin = new Thickness(leftWidth + 4, 0, 0, 0);
                toSubPageAnmation(false);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void toSubPageAnmation(bool useAnmation)
        {
            listView.Visibility = Visibility.Visible;
            borderDetail.Visibility = Visibility.Visible;
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

        public event CanGoBackChangedEventHandler CanGoBackChanged;

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listView.SelectedItem == null && !previousIsNull)
            {
                hideRight.Begin();
                previousIsNull = true;
            }
            else if(previousIsNull)
            {
                gridDetail.Visibility = Visibility.Visible;
                showRight.Begin();
                previousIsNull = false;
            }
            if(state == pageState.lr)
            {
                return;
            }
            else
            {
                state = listView.SelectedItem == null ? pageState.l : pageState.r;
                toSubPageAnmation(true);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void toPage_Completed(object sender, object e)
        {
            switch(state)
            {
            case pageState.lr:
                listView.Visibility = Visibility.Visible;
                borderDetail.Visibility = Visibility.Visible;
                rectangleSplit.Visibility = Visibility.Visible;
                break;
            case pageState.l:
                listView.Visibility = Visibility.Visible;
                borderDetail.Visibility = Visibility.Collapsed;
                rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            case pageState.r:
                listView.Visibility = Visibility.Collapsed;
                borderDetail.Visibility = Visibility.Visible;
                rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            default:
                break;
            }
        }

        private void hideRight_Completed(object sender, object e)
        {
            if(ViewModel.SelectedStyle == null)
                gridDetail.Visibility = Visibility.Collapsed;
        }

        public void GoBack()
        {
            listView.Focus(FocusState.Programmatic);
            listView.SelectedItem = null;
        }
    }
}
