using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace SubtitleEditor.Converters
{
    class BoolVisibilityConverter : IValueConverter
    {
        public bool VisibleBoolValue
        {
            get;
            set;
        } = true;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value == VisibleBoolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch((Visibility)value)
            {
            case Visibility.Visible:
                return VisibleBoolValue;
            case Visibility.Collapsed:
                return !VisibleBoolValue;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
