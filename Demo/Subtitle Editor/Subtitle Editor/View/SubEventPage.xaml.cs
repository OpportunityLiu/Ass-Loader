using Opportunity.Helpers.ObjectModel;
using Opportunity.MvvmUniverse.Views;
using SubtitleEditor.ViewModel;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SubtitleEditor.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    internal sealed partial class SubEventPage : MvvmPage
    {
        public SubEventPage()
        {
            this.ViewModel = ThreadLocalSingleton.GetOrCreate<SubEventViewModel>();
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public new SubEventViewModel ViewModel
        {
            get => (SubEventViewModel)base.ViewModel;
            set => base.ViewModel = value;
        }
    }
}
