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

namespace SubtitleEditor.ViewModel
{
    class PreferencesViewModel : ViewModelBase
    {
        public static PreferencesViewModel Instance
        {
            get;
        } = new PreferencesViewModel();

        private PreferencesViewModel()
        {
            if(NotInit)
                return;
            NotInit = true;
            init(ElementTheme.Light, nameof(Theme));
        }

        public ElementTheme Theme
        {
            get
            {
                return load<ElementTheme>();
            }
            set
            {
                foreach(var item in ((App)Application.Current).WindowCollection)
                {
                    item.LoadWindowPreferences();
                }
                save(value);
            }
        }

        public bool NotInit
        {
            get
            {
                return load<bool>();
            }
            private set
            {
                save(value);
            }
        }

        private void save<T>(T value, [CallerMemberName]string propertyName = null)
        {
            object oldValue;
            if(roaming.Values.TryGetValue(propertyName, out oldValue) && value.ToString().Equals(oldValue))
                return;
            roaming.Values[propertyName] = value.ToString();
            RaisePropertyChanged(propertyName);
        }

        private T load<T>([CallerMemberName]string propertyName = null)
        {
            object value;
            if(roaming.Values.TryGetValue(propertyName, out value))
            {
                var v = value.ToString();
                var type = typeof(T);
                var info = type.GetTypeInfo();
                if(info.IsEnum)
                {
                    return (T)Enum.Parse(type, v);
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(v, type);
                    }
                    catch(InvalidCastException) { }
                    catch(FormatException) { }
                }
            }
            return default(T);
        }

        private void init<T>(T value, [CallerMemberName]string propertyName = null)
        {
            roaming.Values[propertyName] = value.ToString();
        }

        private readonly ApplicationDataContainer roaming = ApplicationData.Current.RoamingSettings;
    }

    static class PreferencesLoader
    {
        public static async void LoadWindowPreferences(this Window window)
        {
            var theme = PreferencesViewModel.Instance.Theme;
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