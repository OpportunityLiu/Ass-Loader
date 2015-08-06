using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace SubtitleEditor.Converters
{
    abstract class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertDictionary[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertDictionary.FirstOrDefault(kv => kv.Value == value).Key;
        }

        protected abstract Dictionary<object, object> ConvertDictionary
        {
            get;
        }
    }

    class ElementThemeConverter : EnumConverter
    {
        protected override Dictionary<object, object> ConvertDictionary
        {
            get;
        } = new Dictionary<object, object>()
        {
            [ElementTheme.Dark]=LocalizedStrings.ElementThemeDark,
            [ElementTheme.Light]=LocalizedStrings.ElementThemeLight
        };
    }
}
