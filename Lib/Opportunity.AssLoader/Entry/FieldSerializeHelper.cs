using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    internal sealed class FieldSerializeHelper
    {
        private object defaultValue;

        public GetValueDelegate GetValue { get; }

        public SetValueDelegate SetValue { get; }

        public DeserializeDelegate Deserialize { get; }

        public DeserializeDelegate DeserializeExact { get; }

        public SerializeDelegate Serialize { get; }

        public FieldSerializeHelper(FieldInfo info, EntryFieldAttribute fieldInfo, SerializeAttribute serializer)
        {
            this.defaultValue = fieldInfo.DefaultValue;
            this.GetValue = info.GetValue;
            this.SetValue = info.SetValue;
            if (serializer != null)
            {
                this.Serialize = serializeCustom(this, serializer.Serialize);
                this.Deserialize = deserializeCustom(this, serializer.Deserialize);
                this.DeserializeExact = deserializeCustomExact(this, serializer.Deserialize);
                return;
            }
            this.Serialize = serializeDefault(this, fieldInfo.Format);
            if (info.FieldType.GetTypeInfo().IsEnum)
            {
                this.Deserialize = deserializeEnum(this, info.FieldType);
                this.DeserializeExact = deserializeEnumExact(this, info.FieldType);
            }
            else
            {
                this.Deserialize = deserializeDefault(this, info.FieldType);
                this.DeserializeExact = deserializeDefaultExact(this, info.FieldType);
            }
        }

        private static DeserializeDelegate deserializeCustom(FieldSerializeHelper field, DeserializerDelegate deserializer)
        {
            return (obj, value) =>
            {
                try
                {
                    field.SetValue(obj, deserializer(value));
                }
                catch (FormatException)
                {
                    field.SetValue(obj, field.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeDefault(FieldSerializeHelper field, Type fieldType)
        {
            return (obj, value) =>
            {
                try
                {
                    field.SetValue(obj, Convert.ChangeType(value, fieldType, FormatHelper.DefaultFormat));
                }
                catch (FormatException)
                {
                    field.SetValue(obj, field.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeEnum(FieldSerializeHelper field, Type fieldType)
        {
            return (obj, value) =>
            {
                try
                {
                    field.SetValue(obj, Enum.Parse(fieldType, value, true));
                }
                catch (FormatException)
                {
                    field.SetValue(obj, field.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeCustomExact(FieldSerializeHelper field, DeserializerDelegate deserializer)
        {
            return (obj, value) => field.SetValue(obj, deserializer(value));
        }

        private static DeserializeDelegate deserializeDefaultExact(FieldSerializeHelper field, Type fieldType)
        {
            return (obj, value) => field.SetValue(obj, Convert.ChangeType(value, fieldType, FormatHelper.DefaultFormat));
        }

        private static DeserializeDelegate deserializeEnumExact(FieldSerializeHelper field, Type fieldType)
        {
            return (obj, value) => field.SetValue(obj, Enum.Parse(fieldType, value, true));
        }

        private static SerializeDelegate serializeCustom(FieldSerializeHelper field, SerializeDelegate serializer)
        {
            return obj => serializer(field.GetValue(obj) ?? field.defaultValue);
        }

        private static SerializeDelegate serializeDefault(FieldSerializeHelper field, string format)
        {
            var formatStr = $"{{0:{format}}}";
            return obj => string.Format(FormatHelper.DefaultFormat, formatStr, field.GetValue(obj) ?? field.defaultValue);
        }
    }
}
