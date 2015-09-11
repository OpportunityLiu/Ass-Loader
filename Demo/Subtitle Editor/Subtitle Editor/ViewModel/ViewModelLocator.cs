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
using Microsoft.Practices.ServiceLocation;
using SubtitleEditor.Model;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace SubtitleEditor.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    class ViewModelLocator
    {
        public static ViewModelLocator GetForView(int viewId)
        {
            lock (syncRoot)
            {
                ViewModelLocator value;
                if(locators.TryGetValue(viewId, out value))
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
                ViewModelLocator value;
                if(locators.TryGetValue(viewId, out value))
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

        private static object syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ioc.Register<MainViewModel>();
            ioc.Register<ScriptInfoViewModel>();
            ioc.Register<StyleViewModel>();
            ioc.Register<SubEventViewModel>();
            ioc.Register<PreferencesViewModel>();
            ioc.Register<DocumentViewModel>();
            ioc.Register<Document>();
        }

        private SimpleIoc ioc = new SimpleIoc();

        public MainViewModel MainView => ioc.GetInstance<MainViewModel>();

        public ScriptInfoViewModel ScriptInfoView => ioc.GetInstance<ScriptInfoViewModel>();

        public StyleViewModel StyleView => ioc.GetInstance<StyleViewModel>();

        public SubEventViewModel SubEventView => ioc.GetInstance<SubEventViewModel>();

        public PreferencesViewModel PreferencesView => ioc.GetInstance<PreferencesViewModel>();

        public DocumentViewModel DocumentView => ioc.GetInstance<DocumentViewModel>();

        public Document Document => ioc.GetInstance<Document>();

        public void Cleanup()
        {
            MainView.Cleanup();
            ScriptInfoView.Cleanup();
            StyleView.Cleanup();
            SubEventView.Cleanup();
            PreferencesView.Cleanup();
            DocumentView.Cleanup();
            Document.Cleanup();
            ioc.Reset();
        }
    }
}