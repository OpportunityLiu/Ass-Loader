using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader.Serializer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class TextSerializeAttribute : SerializeAttribute
    {
        public override string Serialize(object value)
        {
            if(value == null)
                return string.Empty;
            return value.ToString();
        }

        public override object Deserialize(string value)
        {
            return TextContent.Parse(value);
        }
    }
}
