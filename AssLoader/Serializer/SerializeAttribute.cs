using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader.Serializer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class SerializeAttribute : Attribute
    {
        public abstract string Serialize(object value);

        public abstract object Deserialize(string value);
    }
}
