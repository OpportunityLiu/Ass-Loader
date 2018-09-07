using Newtonsoft.Json;
using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.Model
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class JsonSerializerAttribute : SerializeAttribute
    {
        private static JsonSerializer serializer = JsonSerializer.CreateDefault();

        public JsonSerializerAttribute(Type jsonObjectType)
        {
            this.ObjectType = jsonObjectType;
        }

        public Type ObjectType
        {
            get;
            private set;
        }

        public override string Serialize(object value)
        {
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        public override object Deserialize(string value)
        {
            using (var reader = new StringReader(value))
            {
                return serializer.Deserialize(reader, this.ObjectType);
            }
        }
    }
}
