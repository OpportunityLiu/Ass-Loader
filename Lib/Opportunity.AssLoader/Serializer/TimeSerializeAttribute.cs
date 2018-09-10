using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>   
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid time string which is of an "h:mm:ss.ff" format.</exception>
        public override object Deserialize(ReadOnlySpan<char> value)
        {
            var s1 = value.IndexOf(':');
            if (s1 < 0)
                goto Error;
            var t1 = value.Slice(0, s1);
            var h = long.Parse(t1.ToString(), FormatHelper.DefaultFormat);
            value = value.Slice(s1 + 1);
            var s2 = value.IndexOf(':');
            if (s2 < 0)
                goto Error;
            var t2 = value.Slice(0, s2);
            var m = long.Parse(t2.ToString(), FormatHelper.DefaultFormat);
            value = value.Slice(s2 + 1);
            var tick = (long)(Math.Round(1E7 * double.Parse(value.ToString(), FormatHelper.DefaultFormat)));
            return new TimeSpan(((h * 60 + m) * (60 * 10000000) + tick));

            Error:
            throw new FormatException("wrong time format");
        }
    }
}
