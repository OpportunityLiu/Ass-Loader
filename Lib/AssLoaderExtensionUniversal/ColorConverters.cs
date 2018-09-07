using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using UIColor = Windows.UI.Color;

namespace Opportunity.AssLoader.Extension
{
    /// <summary>
    /// A converter between <see cref="UIColor"/> and <see cref="Color"/>.
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert a value of <see cref="Color"/> to <see cref="UIColor"/>.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">Unused.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Color)value).ToUIColor();
        }

        /// <summary>
        /// Convert a value of <see cref="UIColor"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The <see cref="UIColor"/> to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">Unused.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((UIColor)value).ToAssColor();
        }
    }

    /// <summary>
    /// A converter between <see cref="SolidColorBrush"/> and <see cref="Color"/>.
    /// </summary>
    public class SolidColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// Convert a value of <see cref="Color"/> to <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">Unused.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush(((Color)value).ToUIColor());
        }

        /// <summary>
        /// Convert a value of <see cref="SolidColorBrush"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The <see cref="SolidColorBrush"/> to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">Unused.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((SolidColorBrush)value).Color.ToAssColor();
        }
    }
}
