using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssLoader.Serializer;

namespace AssLoader
{
    internal sealed class ScriptInfoSerializeHelper
    {
        private string format;

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

        public DeserializeDelegate DeserializeExact
        {
            get;
            private set;
        }

        public SerializeDelegate Serialize
        {
            get;
            private set;
        }

        public ScriptInfoSerializeHelper(FieldInfo info, ScriptInfoAttribute fieldInfo, SerializeAttribute serializer)
        {
            this.GetValue = info.GetValue;
            this.SetValue = info.SetValue;
            this.defaultValue = fieldInfo.DefaultValue;
            if(serializer != null)
            {
                //custom
                this.Deserialize = deserializeCustom(this, serializer.Deserialize);
                this.Deserialize = deserializeCustomExact(this, serializer.Deserialize);
                if(fieldInfo.IsOptional)
                    this.Serialize = serializeOptional(this, serializer.Serialize);
                else
                    this.Serialize = serialize(this, serializer.Serialize);
                this.format = fieldInfo.FieldName + ": {0}";
                return;
            }
            //enum, nullable and others
            if(fieldInfo.IsOptional)
                this.Serialize = serializeOptional(this);
            else
                this.Serialize = serialize(this);
            var fieldType = info.FieldType;
            this.format = fieldInfo.FieldName + ": {0:" + fieldInfo.Format + "}";
            Type nullableInner;
            if((nullableInner = Nullable.GetUnderlyingType(fieldType)) != null)
            {
                //nullable
                if(this.defaultValue.GetType() == nullableInner)
                    this.defaultValue = Activator.CreateInstance(fieldType, new[] { this.defaultValue });
                this.Deserialize = deserializeNullable(this, fieldType, nullableInner);
                this.Deserialize = deserializeNullableExact(this, fieldType, nullableInner);
                return;
            }
            if(fieldType.GetTypeInfo().IsEnum)
            {
                //enum
                this.Deserialize = deserializeEnum(this, fieldType);
                this.Deserialize = deserializeEnumExact(this, fieldType);
                return;
            }
            //default
            this.Deserialize = deserializeDefault(this, fieldType);
            this.Deserialize = deserializeDefaultExact(this, fieldType);
        }

        private static DeserializeDelegate deserializeDefault(ScriptInfoSerializeHelper target, Type fieldType)
        {
            return (obj, value) =>
            {
                try
                {
                    target.SetValue(obj, Convert.ChangeType(value, fieldType, FormatHelper.DefaultFormat));
                }
                catch(FormatException)
                {
                    target.SetValue(obj, target.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeCustom(ScriptInfoSerializeHelper target, DeserializerDelegate deserializer)
        {
            return (obj, value) =>
            {
                try
                {
                    target.SetValue(obj, deserializer(value));
                }
                catch(FormatException)
                {
                    target.SetValue(obj, target.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeEnum(ScriptInfoSerializeHelper target, Type fieldType)
        {
            return (obj, value) =>
            {
                try
                {
                    target.SetValue(obj, Enum.Parse(fieldType, value, true));
                }
                catch(FormatException)
                {
                    target.SetValue(obj, target.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeNullable(ScriptInfoSerializeHelper target, Type fieldType, Type innerType)
        {
            return (obj, value) =>
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(value))
                        target.SetValue(obj, target.defaultValue);
                    else
                    {
                        var innerValue = Convert.ChangeType(value, innerType, FormatHelper.DefaultFormat);
                        var nullable = Activator.CreateInstance(fieldType, new object[] { innerValue });
                        target.SetValue(obj, nullable);
                    }
                }
                catch(FormatException)
                {
                    target.SetValue(obj, target.defaultValue);
                }
            };
        }

        private static DeserializeDelegate deserializeDefaultExact(ScriptInfoSerializeHelper target, Type fieldType)
        {
            return (obj, value) => target.SetValue(obj, Convert.ChangeType(value, fieldType, FormatHelper.DefaultFormat));
        }

        private static DeserializeDelegate deserializeCustomExact(ScriptInfoSerializeHelper target, DeserializerDelegate deserializer)
        {
            return (obj, value) => target.SetValue(obj, deserializer(value));
        }

        private static DeserializeDelegate deserializeEnumExact(ScriptInfoSerializeHelper target, Type fieldType)
        {
            return (obj, value) => target.SetValue(obj, Enum.Parse(fieldType, value, true));
        }

        private static DeserializeDelegate deserializeNullableExact(ScriptInfoSerializeHelper target, Type fieldType, Type innerType)
        {
            return (obj, value) =>
            {
                if(string.IsNullOrWhiteSpace(value))
                    target.SetValue(obj, target.defaultValue);
                else
                {
                    var innerValue = Convert.ChangeType(value, innerType, FormatHelper.DefaultFormat);
                    var nullable = Activator.CreateInstance(fieldType, new object[] { innerValue });
                    target.SetValue(obj, nullable);
                }
            };
        }

        private static SerializeDelegate serialize(ScriptInfoSerializeHelper target)
        {
            return obj =>
            {
                var value = target.GetValue(obj) ?? target.defaultValue;
                return string.Format(FormatHelper.DefaultFormat, target.format, value);
            };
        }

        private static SerializeDelegate serialize(ScriptInfoSerializeHelper target, SerializerDelegate serializer)
        {
            return obj =>
            {
                var value = target.GetValue(obj) ?? target.defaultValue;
                return string.Format(FormatHelper.DefaultFormat, target.format, serializer(value));
            };
        }

        private static SerializeDelegate serializeOptional(ScriptInfoSerializeHelper target)
        {
            return obj =>
            {
                var value = target.GetValue(obj);
                if(value == null || value == target.defaultValue)
                    return null;
                return string.Format(FormatHelper.DefaultFormat, target.format, value);
            };
        }

        private static SerializeDelegate serializeOptional(ScriptInfoSerializeHelper target, SerializerDelegate serializer)
        {
            return obj =>
            {
                var value = target.GetValue(obj);
                if(value == null || value == target.defaultValue)
                    return null;
                return string.Format(FormatHelper.DefaultFormat, target.format, serializer(value));
            };
        }
    }
}
