using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AssLoader.Serializer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class TimeSerializeAttribute : SerializeAttribute
    {
        public override string Serialize(object value)
        {
            var va = (DateTime)value;
            return (va.Ticks / 36000000000).ToString(FormatHelper.DefaultFormat) + va.ToString(":mm:ss.ff", FormatHelper.DefaultFormat);
        }

        public override object Deserialize(string value)
        {
            var num = value.Split(':');
            if(num.Length != 3)
                throw new FormatException("wrong time format");
            var h = long.Parse(num[0], FormatHelper.DefaultFormat);
            var m = long.Parse(num[1], FormatHelper.DefaultFormat);
            var tick = (long)(Math.Round(1E7 * double.Parse(num[2], FormatHelper.DefaultFormat)));
            return new DateTime(((h * 60 + m) * (60 * 10000000) + tick));
        }
    }
}
