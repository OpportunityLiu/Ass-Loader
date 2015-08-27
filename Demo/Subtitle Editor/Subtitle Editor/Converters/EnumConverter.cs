using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace SubtitleEditor.Converters
{
    abstract class EnumConverter<T> : IValueConverter
        where T :struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertDictionary[(T)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertDictionary.FirstOrDefault(kv => kv.Value == value).Key;
        }

        protected abstract Dictionary<T, object> ConvertDictionary
        {
            get;
        }
    }
}
