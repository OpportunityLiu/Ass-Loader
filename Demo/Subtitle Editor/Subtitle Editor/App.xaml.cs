using SubtitleEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SubtitleEditor
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(Microsoft.ApplicationInsights.WindowsCollectors.Metadata | Microsoft.ApplicationInsights.WindowsCollectors.Session | Microsoft.ApplicationInsights.WindowsCollectors.UnhandledException);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void initView(IActivatedEventArgs args)
        {
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            var view = ApplicationView.GetForCurrentView();

            var doc = ViewModel.ViewModelLocator.GetForView(view.Id).Document;
            System.ComponentModel.PropertyChangedEventHandler titleUpdater = (sender, e) =>
            {
                if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(Document.Subtitle) || e.PropertyName == nameof(Document.SubtitleFile) || e.PropertyName == nameof(Document.IsModified))
                {
                    var document = (Document)sender;
                    view.Title = $"{(document.IsModified ? "*" : "")} {document.SubtitleFile?.Name ?? document.Subtitle?.ScriptInfo.Title ?? LocalizedStrings.Untitled}";
                }
            };
            doc.PropertyChanged += titleUpdater;
            titleUpdater(doc, new System.ComponentModel.PropertyChangedEventArgs(null));

            view.SetPreferredMinSize(new Size(10000, 10000));

            Frame rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if(args.PreviousExecutionState == ApplicationExecutionState.Terminated || args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                //TODO: 从之前挂起的应用程序加载状态
            }

            var current = Window.Current;

            // 将框架放在当前窗口中
            current.Content = rootFrame;
            rootFrame.Navigate(typeof(View.MainPage));

            // 确保当前窗口处于活动状态
            current.Activate();

            ViewModel.PreferencesLoader.LoadWindowPreferences(current);
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="args">有关启动请求和过程的详细信息。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await createNewViewIfNeeded(() =>
            {
                var ioc = ViewModel.ViewModelLocator.GetForCurrentView();
                ioc.Document.NewSubtitle();
                initView(args);
            });
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            await createNewViewIfNeeded(async () =>
            {
                var ioc = ViewModel.ViewModelLocator.GetForCurrentView();
                var toOpen = (StorageFile)args.Files.First(f => f is StorageFile);
                await ioc.Document.OpenFileAsync(toOpen);
                initView(args);
            });
        }

        private async Task createNewViewIfNeeded(Windows.UI.Core.DispatchedHandler initAction)
        {
            if(Window.Current.Content == null)
            {
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, initAction);
                return;
            }
            var newViewId = 0;
            var newView = Windows.ApplicationModel.Core.CoreApplication.CreateNewView();
            var init = newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, initAction);
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => newViewId = ApplicationView.GetForCurrentView().Id);
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await init;
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            WindowCollection.Add(args.Window);
        }

        public System.Collections.Concurrent.BlockingCollection<Window> WindowCollection
        {
            get;
        } = new System.Collections.Concurrent.BlockingCollection<Window>();

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
