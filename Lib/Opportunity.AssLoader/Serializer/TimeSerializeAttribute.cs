using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private const long TICKS_PER_HS = 100000;
        private const long TICKS_PER_SEC = 10000000;
        private const long TICKS_PER_MIN = TICKS_PER_SEC * 60;
        private const long TICKS_PER_HR = TICKS_PER_MIN * 60;

        private static readonly char[] ZEROS = "00000000000000000000".ToCharArray();

        /// <summary>
        /// Convert <see cref="TimeSpan"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override void Serialize(TextWriter writer, object value, ISerializeInfo serializeInfo)
        {
            var va = ((TimeSpan)value).Ticks;
            var h = va / TICKS_PER_HR;
            va -= h * TICKS_PER_HR;
            var m = va / TICKS_PER_MIN;
            va -= m * TICKS_PER_MIN;
            var s = va / TICKS_PER_SEC;
            va -= s * TICKS_PER_SEC;
            var hs = va / TICKS_PER_HS;
            va -= hs * TICKS_PER_HS;

            writevalue(writer, h, 0);
            writer.Write(':');
            writevalue(writer, m, 2);
            writer.Write(':');
            writevalue(writer, s, 2);
            writer.Write('.');
            writevalue(writer, hs, 2);

            void writevalue(TextWriter w, long v, int l)
            {
                if (v < 10)
                {
                    if (l > 1)
                        w.Write(ZEROS, 0, l - 1);
                    w.Write((char)('0' + v));
                }
                else if (v < 100)
                {
                    if (l > 2)
                        w.Write(ZEROS, 0, l - 2);
                    w.Write((char)('0' + v / 10));
                    w.Write((char)('0' + v % 10));
                }
                else
                    w.Write(v.ToString(FormatHelper.DefaultFormat).PadLeft(l, '0'));
            }
        }

        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>   
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid time string which is of an "h:mm:ss.ff" format.</exception>
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            long h = 0, ths = 0;
            int m = 0, s = 0;

            var s1 = value.IndexOf(':');
            if (s1 < 0)
                goto Error;
            var t1 = value.Slice(0, s1);
            if (!long.TryParse(t1.ToString(), NumberStyles.Any, FormatHelper.DefaultFormat, out h))
                goto Error;

            value = value.Slice(s1 + 1);
            var s2 = value.IndexOf(':');
            if (s2 < 0)
                goto Error;
            var t2 = value.Slice(0, s2);
            if (!int.TryParse(t2.ToString(), NumberStyles.Any, FormatHelper.DefaultFormat, out m))
                goto Error;

            value = value.Slice(s2 + 1);
            var s3 = value.IndexOfAny(':', '.');
            var t3 = (s3 >= 0) ? value.Slice(0, s3) : value;

            if (!int.TryParse(t3.ToString(), NumberStyles.Any, FormatHelper.DefaultFormat, out s))
                goto Error;

            if (s3 < 0)
                goto Success;

            value = value.Slice(s3 + 1);
            var ee = TICKS_PER_SEC;
            foreach (var ch in value)
            {
                if (char.IsWhiteSpace(ch))
                    continue;
                if ('0' > ch || ch > '9')
                    goto Error;

                ee /= 10;
                ths += (ch - '0') * ee;

            }

            goto Success;
            Error:
            deserializeInfo.AddException(new FormatException("wrong time format"));
            Success:
            return new TimeSpan(ths + s * TICKS_PER_SEC + m * TICKS_PER_MIN + h * TICKS_PER_HR);
        }
    }
}
