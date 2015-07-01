using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AssLoader.Serializer;

namespace AssLoader
{
    internal sealed class FieldSerializeHelper
    {
        private object defaultValue;

        public GetValueDelegate GetValue
        {
            private set;
            get;
        }

        public SetValueDelegate SetValue
        {
            private set;
            get;
        }

        public DeserializeDelegate Deserialize
        {
            get;
            private set;
        }

        public SerializeDelegate Serialize
        {
            get;
            private set;
        }

        public FieldSerializeHelper(FieldInfo info, EntryFieldAttribute fieldInfo, SerializeAttribute serializer)
        {
            this.defaultValue = fieldInfo.DefaultValue;
            this.GetValue = info.GetValue;
            this.SetValue = info.SetValue;
            if(serializer != null)
            {
                this.Serialize = serialize(this, serializer.Serialize);
                this.Deserialize = deserialize(this, serializer.Deserialize);
            }
            else
            {
                this.Serialize = serialize(this, "{0:" + fieldInfo.Format + "}");
                this.Deserialize = deserialize(this, info.FieldType);
            }
        }

        private static DeserializeDelegate deserialize(FieldSerializeHelper field, DeserializerDelegate deserializer)
        {
            return (obj,value) =>
            {
                try
                {
                    field.SetValue(obj, deserializer(value) ?? field.defaultValue);
                }
                catch(FormatException)
                {
                    field.SetValue(obj, field.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserialize(FieldSerializeHelper field, Type fieldType)
        {
            return (obj, value) =>
            {
                try
                {
                    field.SetValue(obj, Convert.ChangeType(value, fieldType, FormatHelper.DefaultFormat));
                }
                catch(FormatException)
                {
                    field.SetValue(obj, field.defaultValue);
                }
            };
        }

        private static SerializeDelegate serialize(FieldSerializeHelper field, SerializeDelegate serializer)
        {
            return obj => serializer(field.GetValue(obj) ?? field.defaultValue);
        }

        private static SerializeDelegate serialize(FieldSerializeHelper field,string format)
        {
            return obj => string.Format(FormatHelper.DefaultFormat, format, field.GetValue(obj) ?? field.defaultValue);
        }
    }
}
