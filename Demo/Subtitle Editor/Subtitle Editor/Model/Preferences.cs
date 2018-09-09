using Opportunity.MvvmUniverse.Storage;
using SubtitleEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SubtitleEditor.Model
{
    internal class Preferences : StorageObject
    {
        public Preferences() : base(nameof(Preferences))
        {
        }

        [DefaultValue(ElementTheme.Light)]
        [ApplicationSetting(Windows.Storage.ApplicationDataLocality.Local)]
        public ElementTheme Theme
        {
            get => GetStorage<ElementTheme>();
            set
            {
                foreach (var item in ((App)Application.Current).WindowDictionary)
                {
                    item.Value.SetWindowPreferences(value);
                }
                SetStorage(value);
            }
        }
    }

    internal static class PreferencesLoader
    {
        public static async void SetWindowPreferences(this Window window, ElementTheme theme)
        {
            await window.Dispatcher.RunIdleAsync(s =>
            {
                ((FrameworkElement)window.Content).RequestedTheme = theme;
                var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                switch (theme)
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
                if (titlebar == null)
                    return;
                titlebar.BackgroundColor = this.BackgroundColor;
                titlebar.ForegroundColor = this.ForegroundColor;
                titlebar.InactiveBackgroundColor = this.InactiveBackgroundColor;
                titlebar.InactiveForegroundColor = this.InactiveForegroundColor;
                titlebar.ButtonBackgroundColor = this.ButtonBackgroundColor;
                titlebar.ButtonForegroundColor = this.ButtonForegroundColor;
                titlebar.ButtonPressedBackgroundColor = this.ButtonPressedBackgroundColor;
                titlebar.ButtonPressedForegroundColor = this.ButtonPressedForegroundColor;
                titlebar.ButtonHoverBackgroundColor = this.ButtonHoverBackgroundColor;
                titlebar.ButtonHoverForegroundColor = this.ButtonHoverForegroundColor;
                titlebar.ButtonInactiveBackgroundColor = this.ButtonInactiveBackgroundColor;
                titlebar.ButtonInactiveForegroundColor = this.ButtonInactiveForegroundColor;
            }

            public static TitleBarColor Dark { get; } = new TitleBarColor()
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

            public static TitleBarColor Light { get; } = new TitleBarColor()
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
