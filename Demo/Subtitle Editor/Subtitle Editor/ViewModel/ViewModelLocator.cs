/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SubtitleEditor"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using SubtitleEditor.Model;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace SubtitleEditor.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
    {
        public static ViewModelLocator GetForView(int viewId)
        {
            lock (syncRoot)
            {
                if (locators.TryGetValue(viewId, out var value))
                    return value;
                value = new ViewModelLocator();
                locators[viewId] = value;
                return value;
            }
        }

        public static void ClearForView(int viewId)
        {
            lock (syncRoot)
            {
                if (locators.TryGetValue(viewId, out var value))
                    value.Cleanup();
                locators.Remove(viewId);
            }
        }

        public static ViewModelLocator GetForCurrentView()
        {
            return GetForView(ApplicationView.GetForCurrentView().Id);
        }

        public static void ClearForCurrentView()
        {
            ClearForView(ApplicationView.GetForCurrentView().Id);
        }

        private static Dictionary<int, ViewModelLocator> locators = new Dictionary<int, ViewModelLocator>();

        private static readonly object syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            this.ioc.Register<MainViewModel>();
            this.ioc.Register<ScriptInfoViewModel>();
            this.ioc.Register<StyleViewModel>();
            this.ioc.Register<SubEventViewModel>();
            this.ioc.Register<PreferencesViewModel>();
            this.ioc.Register<DocumentViewModel>();
            this.ioc.Register<Document>();
        }

        private readonly SimpleIoc ioc = new SimpleIoc();

        public MainViewModel MainView => this.ioc.GetInstance<MainViewModel>();

        public ScriptInfoViewModel ScriptInfoView => this.ioc.GetInstance<ScriptInfoViewModel>();

        public StyleViewModel StyleView => this.ioc.GetInstance<StyleViewModel>();

        public SubEventViewModel SubEventView => this.ioc.GetInstance<SubEventViewModel>();

        public PreferencesViewModel PreferencesView => this.ioc.GetInstance<PreferencesViewModel>();

        public DocumentViewModel DocumentView => this.ioc.GetInstance<DocumentViewModel>();

        public Document Document => this.ioc.GetInstance<Document>();

        public void Cleanup()
        {
            this.MainView.Cleanup();
            this.ScriptInfoView.Cleanup();
            this.StyleView.Cleanup();
            this.SubEventView.Cleanup();
            this.PreferencesView.Cleanup();
            this.DocumentView.Cleanup();
            this.Document.Cleanup();
            this.ioc.Reset();
        }
    }
}