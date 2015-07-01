using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader.Serializer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ColorSerializeAttribute : SerializeAttribute
    {
        public override string Serialize(object value)
        {
            return value.ToString();
        }

        public override object Deserialize(string value)
        {
            return Color.Parse(value);
        }
    }
}
