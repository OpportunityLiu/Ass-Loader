using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.Storage;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.Foundation.Metadata;

namespace SubtitleEditor.ViewModel
{
    class PreferencesViewModel : ViewModelBase
    {
        public PreferencesViewModel()
        {
            instances.Add(new WeakReference<PreferencesViewModel>(this));
            if(NotInit)
                return;
            NotInit = true;
            init(ElementTheme.Light, nameof(Theme));
        }

        private CoreDispatcher dispatcher = Window.Current?.Dispatcher;

        public ElementTheme Theme
        {
            get
            {
                return Load<ElementTheme>();
            }
            set
            {
                foreach(var item in ((App)Application.Current).WindowDictionary)
                {
                    item.Value.LoadWindowPreferences();
                }
                Save(value);
            }
        }

        public bool NotInit
        {
            get
            {
                return Load<bool>();
            }
            private set
            {
                Save(value);
            }
        }

        public static async void Save<T>(T value, [CallerMemberName]string propertyName = null)
        {
            object oldValue;
            if(roaming.Values.TryGetValue(propertyName, out oldValue) && value.ToString().Equals(oldValue))
                return;
            roaming.Values[propertyName] = value.ToString();
            for(int i = 0; i < instances.Count; i++)
            {
                PreferencesViewModel tar;
                while(!instances[i].TryGetTarget(out tar))
                {
                    instances.RemoveAt(i);
                    if(i >= instances.Count)
                        break;
                }
                await tar.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => tar.RaisePropertyChanged(propertyName));
            }
        }

        public static T Load<T>([CallerMemberName]string propertyName = null)
        {
            object value;
            if(roaming.Values.TryGetValue(propertyName, out value))
            {
                var v = value.ToString();
                Tuple<Type, bool> pInfo;
                if(!propertyCache.TryGetValue(propertyName, out pInfo))
                {
                    var t = typeof(T);
                    pInfo = new Tuple<Type, bool>(t, t.GetTypeInfo().IsEnum);
                    propertyCache[propertyName] = pInfo;
                }
                if(pInfo.Item2)
                {
                    return (T)Enum.Parse(pInfo.Item1, v);
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(v, pInfo.Item1);
                    }
                    catch(InvalidCastException) { }
                    catch(FormatException) { }
                }
            }
            return default(T);
        }

        private static List<WeakReference<PreferencesViewModel>> instances = new List<WeakReference<PreferencesViewModel>>();

        //propertyName,propertyType,isEnum
        private static Dictionary<string, Tuple<Type, bool>> propertyCache = new Dictionary<string, Tuple<Type, bool>>();

        private static void init<T>(T value, [CallerMemberName]string propertyName = null)
        {
            roaming.Values[propertyName] = value.ToString();
        }

        private static readonly ApplicationDataContainer roaming = ApplicationData.Current.RoamingSettings;
    }

    static class PreferencesLoader
    {
        public static async void LoadWindowPreferences(this Window window)
        {
            var theme = PreferencesViewModel.Load<ElementTheme>(nameof(PreferencesViewModel.Theme));
            await window.Dispatcher.RunIdleAsync(s =>
            {
                ((FrameworkElement)window.Content).RequestedTheme = theme;
                var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                switch(theme)
                {
                case ElementTheme.Light:
                    TitleBarColor.Light.SetColors(titlebar);
                    break;
                case ElementTheme.Dark:
                    TitleBarColor.Dark.SetColors(titlebar);
                    break;
                }
                if(ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    switch(theme)
                    {
                    case ElementTheme.Light:
                        TitleBarColor.Light.SetColors(StatusBar.GetForCurrentView());
                        break;
                    case ElementTheme.Dark:
                        TitleBarColor.Dark.SetColors(StatusBar.GetForCurrentView());
                        break;
                    }
                }
            });
        }

        private struct TitleBarColor
        {
            private Color
                BackgroundColor,
                ForegroundColor,
                InactiveBackgroundColor,
                InactiveForegroundColor,
                ButtonBackgroundColor,
                ButtonForegroundColor,
                ButtonPressedBackgroundColor,
                ButtonPressedForegroundColor,
                ButtonHoverBackgroundColor,
                ButtonHoverForegroundColor,
                ButtonInactiveBackgroundColor,
                ButtonInactiveForegroundColor;

            public void SetColors(ApplicationViewTitleBar titlebar)
            {
                if(titlebar == null)
                    return;
                titlebar.BackgroundColor = BackgroundColor;
                titlebar.ForegroundColor = ForegroundColor;
                titlebar.InactiveBackgroundColor = InactiveBackgroundColor;
                titlebar.InactiveForegroundColor = InactiveForegroundColor;
                titlebar.ButtonBackgroundColor = ButtonBackgroundColor;
                titlebar.ButtonForegroundColor = ButtonForegroundColor;
                titlebar.ButtonPressedBackgroundColor = ButtonPressedBackgroundColor;
                titlebar.ButtonPressedForegroundColor = ButtonPressedForegroundColor;
                titlebar.ButtonHoverBackgroundColor = ButtonHoverBackgroundColor;
                titlebar.ButtonHoverForegroundColor = ButtonHoverForegroundColor;
                titlebar.ButtonInactiveBackgroundColor = ButtonInactiveBackgroundColor;
                titlebar.ButtonInactiveForegroundColor = ButtonInactiveForegroundColor;
            }

            public void SetColors(StatusBar statusBar)
            {
                if(statusBar == null)
                    return;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = BackgroundColor;
                statusBar.ForegroundColor = ForegroundColor;
            }

            public static TitleBarColor Dark
            {
                get;
            } = new TitleBarColor()
            {
                BackgroundColor = Color.FromArgb(255, 31, 31, 31),
                ForegroundColor = Colors.White,
                InactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31),
                InactiveForegroundColor = Color.FromArgb(255, 122, 122, 122),
                ButtonBackgroundColor = Color.FromArgb(255, 31, 31, 31),
                ButtonForegroundColor = Colors.White,
                ButtonInactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31),
                ButtonInactiveForegroundColor = Color.FromArgb(255, 122, 122, 122),
                ButtonHoverBackgroundColor = Color.FromArgb(255, 53, 53, 53),
                ButtonHoverForegroundColor = Colors.White,
                ButtonPressedBackgroundColor = Color.FromArgb(255, 76, 76, 76),
                ButtonPressedForegroundColor = Colors.White
            };

            public static TitleBarColor Light
            {
                get;
            } = new TitleBarColor()
            {
                BackgroundColor = Color.FromArgb(255, 230, 230, 230),
                ForegroundColor = Colors.Black,
                InactiveBackgroundColor = Color.FromArgb(255, 230, 230, 230),
                InactiveForegroundColor = Color.FromArgb(255, 140, 140, 140),
                ButtonBackgroundColor = Color.FromArgb(255, 230, 230, 230),
                ButtonForegroundColor = Colors.Black,
                ButtonInactiveBackgroundColor = Color.FromArgb(255, 230, 230, 230),
                ButtonInactiveForegroundColor = Color.FromArgb(255, 140, 140, 140),
                ButtonHoverBackgroundColor = Color.FromArgb(255, 206, 206, 206),
                ButtonHoverForegroundColor = Colors.Black,
                ButtonPressedBackgroundColor = Color.FromArgb(255, 184, 184, 184),
                ButtonPressedForegroundColor = Colors.Black
            };
        }
    }
}