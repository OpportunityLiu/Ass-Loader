using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader.Serializer;
using Newtonsoft.Json;
using System.IO;

namespace SubtitleEditor.Model
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class JsonSerializerAttribute : SerializeAttribute
    {
        private static JsonSerializer serializer = JsonSerializer.CreateDefault();

        public JsonSerializerAttribute(Type jsonObjectType)
        {
            ObjectType = jsonObjectType;
        }

        public Type ObjectType
        {
            get;
            private set;
        }

        public override string Serialize(object value)
        {
            using(var writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        public override object Deserialize(string value)
        {
            using(var reader=new StringReader(value))
            {
                return serializer.Deserialize(reader, ObjectType);
            }
        }
    }
}
