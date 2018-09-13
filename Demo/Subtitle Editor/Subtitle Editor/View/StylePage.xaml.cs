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
using Windows.UI;
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
    internal sealed partial class StylePage : MvvmPage, IGoBack
    {
        public StylePage()
        {
            this.ViewModel = ThreadLocalSingleton.GetOrCreate<StyleViewModel>();
            this.InitializeComponent();
            this.gridDetail.Visibility = Visibility.Collapsed;
            this.gridDetail.Opacity = 0;
            this.leftWidth = (double)this.Resources["LeftSubPageWidth"];
            this.onePageMinWidth = (double)this.Resources["OnePageMinWidth"];
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public new StyleViewModel ViewModel
        {
            get => (StyleViewModel)base.ViewModel;
            set => base.ViewModel = value;
        }

        public bool CanGoBack => this.state == pageState.r;

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
            if (e.NewSize.Width <= this.onePageMinWidth)
            {
                if (this.state == pageState.l || this.state == pageState.r)
                    return;
                this.state = this.listView.SelectedItem == null ? pageState.l : pageState.r;
                this.listView.ClearValue(WidthProperty);
                this.listView.ClearValue(HorizontalAlignmentProperty);
                this.borderDetail.ClearValue(MarginProperty);
                this.toSubPageAnmation(false);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (this.state == pageState.lr)
                    return;
                this.state = pageState.lr;
                this.listView.Width = this.leftWidth;
                this.listView.HorizontalAlignment = HorizontalAlignment.Left;
                this.borderDetail.Margin = new Thickness(this.leftWidth + 4, 0, 0, 0);
                this.toSubPageAnmation(false);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void toSubPageAnmation(bool useAnmation)
        {
            this.listView.Visibility = Visibility.Visible;
            this.borderDetail.Visibility = Visibility.Visible;
            switch (this.state)
            {
            case pageState.lr:
                this.leftAnimation.To = 0;
                this.rightAnimation.To = 0;
                this.showLeft.Begin();
                if (!useAnmation)
                    this.showLeft.SkipToFill();
                break;
            case pageState.l:
                this.leftAnimation.To = 0;
                this.rightAnimation.To = this.root.ActualWidth;
                this.showLeft.Begin();
                if (!useAnmation)
                    this.showLeft.SkipToFill();
                break;
            case pageState.r:
                this.leftAnimation.To = -this.root.ActualWidth;
                this.rightAnimation.To = 0;
                this.hideLeft.Begin();
                if (!useAnmation)
                    this.hideLeft.SkipToFill();
                break;
            default:
                break;
            }
            this.toPage.Begin();
            if (!useAnmation)
                this.toPage.SkipToFill();
        }

        private bool previousIsNull = true;

        public event CanGoBackChangedEventHandler CanGoBackChanged;

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listView.SelectedItem == null && !this.previousIsNull)
            {
                this.hideRight.Begin();
                this.previousIsNull = true;
            }
            else if (this.previousIsNull)
            {
                this.gridDetail.Visibility = Visibility.Visible;
                this.showRight.Begin();
                this.previousIsNull = false;
            }
            if (this.state == pageState.lr)
            {
                return;
            }
            else
            {
                this.state = this.listView.SelectedItem == null ? pageState.l : pageState.r;
                this.toSubPageAnmation(true);
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void toPage_Completed(object sender, object e)
        {
            switch (this.state)
            {
            case pageState.lr:
                this.listView.Visibility = Visibility.Visible;
                this.borderDetail.Visibility = Visibility.Visible;
                this.rectangleSplit.Visibility = Visibility.Visible;
                break;
            case pageState.l:
                this.listView.Visibility = Visibility.Visible;
                this.borderDetail.Visibility = Visibility.Collapsed;
                this.rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            case pageState.r:
                this.listView.Visibility = Visibility.Collapsed;
                this.borderDetail.Visibility = Visibility.Visible;
                this.rectangleSplit.Visibility = Visibility.Collapsed;
                break;
            default:
                break;
            }
        }

        private void hideRight_Completed(object sender, object e)
        {
            if (this.ViewModel.SelectedStyle == null)
                this.gridDetail.Visibility = Visibility.Collapsed;
        }

        private void slColor_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var v = (byte)e.NewValue;
            scbSamplePresenter.Color = Color.FromArgb(255, v, v, v);
        }

        public void GoBack()
        {
            this.listView.Focus(FocusState.Programmatic);
            this.listView.SelectedItem = null;
        }
    }
}
