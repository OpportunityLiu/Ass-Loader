using Opportunity.AssLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SubtitleEditor.Converters
{
    internal abstract class EnumConverter<T> : IValueConverter
        where T : struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.ConvertDictionary[(T)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.ConvertDictionary.FirstOrDefault(kv => kv.Value == value).Key;
        }

        protected abstract Dictionary<T, object> ConvertDictionary
        {
            get;
        }
    }

    internal class WrapStyleConverter : EnumConverter<WrapStyle>
    {
        protected override Dictionary<WrapStyle, object> ConvertDictionary
        {
            get;
        } = new Dictionary<WrapStyle, object>()
        {
            [WrapStyle.Smart] = LocalizedStrings.Resources.WrapStyleSmart,
            [WrapStyle.None] = LocalizedStrings.Resources.WrapStyleNone,
            [WrapStyle.EndOfLine] = LocalizedStrings.Resources.WrapStyleEndOfLine,
            [WrapStyle.Smart2] = LocalizedStrings.Resources.WrapStyleSmart2
        };
    }

    internal class CollisionStyleConverter : EnumConverter<CollisionStyle>
    {
        protected override Dictionary<CollisionStyle, object> ConvertDictionary
        {
            get;
        } = new Dictionary<CollisionStyle, object>()
        {
            [CollisionStyle.Normal] = LocalizedStrings.Resources.CollisionStyleNormal,
            [CollisionStyle.Reverse] = LocalizedStrings.Resources.CollisionStyleReverse
        };
    }
}
