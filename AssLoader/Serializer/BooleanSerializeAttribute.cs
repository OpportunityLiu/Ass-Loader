using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader.Serializer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class BooleanSerializeAttribute : SerializeAttribute
    {
        private string t = "True";

        public string TrueString
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
            }
        }

        private string f = "False";

        public string FalseString
        {
            get
            {
                return f;
            }
            set
            {
                f = value;
            }
        }

        public bool ThrowOnDeserializing
        {
            get;
            set;
        }

        public override string Serialize(object value)
        {
            if((bool)value)
                return t;
            else
                return f;
        }

        public override object Deserialize(string value)
        {
            if(string.Equals(value, t, StringComparison.OrdinalIgnoreCase))
                return true;
            else if(string.Equals(value, t, StringComparison.OrdinalIgnoreCase))
                return false;
            else if(ThrowOnDeserializing)
                throw new FormatException("Convert failed, the string to deserialize is :\n" + value);
            else
                return false;
        }
    }
}
