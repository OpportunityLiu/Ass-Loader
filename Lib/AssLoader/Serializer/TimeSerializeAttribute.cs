using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Custom serializer for <see cref="TimeSpan"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class TimeSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Convert <see cref="TimeSpan"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override string Serialize(object value)
        {
            var va = (TimeSpan)value;
            return (va.Ticks / 36000000000).ToString(FormatHelper.DefaultFormat) + va.ToString(@"\:mm\:ss\.ff", FormatHelper.DefaultFormat);
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>   
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid time string which is of an "h:mm:ss.ff" format.</exception>
        public override object Deserialize(string value)
        {
            var num = value.Split(':');
            if(num.Length != 3)
                throw new FormatException("wrong time format");
            var h = long.Parse(num[0], FormatHelper.DefaultFormat);
            var m = long.Parse(num[1], FormatHelper.DefaultFormat);
            var tick = (long)(Math.Round(1E7 * double.Parse(num[2], FormatHelper.DefaultFormat)));
            return new TimeSpan(((h * 60 + m) * (60 * 10000000) + tick));
        }
    }
}
