using Opportunity.AssLoader.Serializer;
using Opportunity.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    internal sealed class SerializeHelper<TObj, TFieldInfo>
        where TFieldInfo : SerializableFieldAttribute
    {
        private static readonly Dictionary<Type, Dictionary<string, SerializeHelper<TObj, TFieldInfo>>> fieldInfoCache
            = new Dictionary<Type, Dictionary<string, SerializeHelper<TObj, TFieldInfo>>>();

        public static Dictionary<string, SerializeHelper<TObj, TFieldInfo>> GetScriptInfoFields(Type containerType)
        {
            if (!fieldInfoCache.TryGetValue(containerType, out var fieldInfo))
            {
                var temp = new List<Dictionary<string, SerializeHelper<TObj, TFieldInfo>>>();
                var ti = containerType.GetTypeInfo();
                do
                {
                    var dict = new Dictionary<string, SerializeHelper<TObj, TFieldInfo>>(StringComparer.OrdinalIgnoreCase);
                    foreach (var fInfo in ti.DeclaredFields)
                    {
                        if (fInfo.IsStatic)
                            continue;
                        var att = fInfo.GetCustomAttribute<TFieldInfo>();
                        if (att is null)
                            continue;
                        var ser = fInfo.GetCustomAttribute<SerializeAttribute>();
                        var helper = new SerializeHelper<TObj, TFieldInfo>(fInfo, att, ser);
                        dict.Add(att.FieldName, helper);
                        if (!string.IsNullOrEmpty(att.Alias))
                            dict.Add(att.Alias, helper);
                    }
                    foreach (var pInfo in ti.DeclaredProperties)
                    {
                        if ((pInfo.GetMethod ?? pInfo.SetMethod).IsStatic)
                            continue;
                        var att = pInfo.GetCustomAttribute<TFieldInfo>();
                        if (att is null)
                            continue;
                        var ser = pInfo.GetCustomAttribute<SerializeAttribute>();
                        var helper = new SerializeHelper<TObj, TFieldInfo>(pInfo, att, ser);
                        dict.Add(att.FieldName, helper);
                        if (!string.IsNullOrEmpty(att.Alias))
                            dict.Add(att.Alias, helper);
                    }
                    temp.Add(dict);
                    containerType = ti.BaseType;
                    ti = containerType.GetTypeInfo();
                } while (containerType != typeof(object));
                fieldInfo = (from dict in temp.Reverse<Dictionary<string, SerializeHelper<TObj, TFieldInfo>>>()
                             from entry in dict
                             select entry).ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
                lock (fieldInfoCache)
                {
                    fieldInfoCache[containerType] = fieldInfo;
                }
            }
            return fieldInfo;
        }

        public TFieldInfo Info { get; }
        public SerializeAttribute Serializer { get; }
        public TypeInfo FieldType { get; }
        public TypeCode FieldTypeCode { get; }
        public bool FieldCanBeNull { get; }

        public GetValueDelegate GetValue { get; }
        public SetValueDelegate SetValue { get; }

        public void Deserialize(TObj obj, ReadOnlySpan<char> value)
        {
            try
            {
                DeserializeExact(obj, value);
            }
            catch (Exception)
            {
                SetValue(obj, Info.DefaultValue);
            }
        }

        public void DeserializeExact(TObj obj, ReadOnlySpan<char> value)
        {
            var va = default(object);

            if (Serializer is null)
            {
                if (FieldCanBeNull && value.IsEmpty)
                    va = null;
                else if (FieldType.IsEnum)
                    va = Enum.Parse(FieldType.AsType(), value.ToString(), true);
                else
                    va = Convert.ChangeType(value.ToString(), FieldType.AsType(), FormatHelper.DefaultFormat);
            }
            else
                va = Serializer.Deserialize(value.ToString());
            SetValue(obj, va);
        }

        public string Serialize(TObj obj)
        {
            var value = GetValue(obj);
            if (Info.IsOptional && (value is null || Equals(value, Info.DefaultValue)))
                return null;
            if (value is null)
                value = Info.DefaultValue;
            if (Serializer is null)
            {
                var f = value as IFormattable;
                if (Info.Format.IsNullOrWhiteSpace() || f is null)
                    return value.ToString();
                else
                    return f.ToString(Info.Format, FormatHelper.DefaultFormat);
            }
            else
                return Serializer.Serialize(value);
        }

        public SerializeHelper(MemberInfo info, TFieldInfo fieldInfo, SerializeAttribute serializer)
        {
            Info = fieldInfo;
            Serializer = serializer;
            if (info is FieldInfo fi)
            {
                if (fi.IsInitOnly)
                    throw new ArgumentException($"Do not support readonly fields.\nField name: `{fi.Name}`\nType name: `{fi.DeclaringType}`");
                this.GetValue = fi.GetValue;
                this.SetValue = fi.SetValue;
                this.FieldType = fi.FieldType.GetTypeInfo();
            }
            else if (info is PropertyInfo pi)
            {
                if (!(pi.CanRead && pi.CanWrite))
                    throw new ArgumentException($"Do not support readonly or writeonly properties.\nProperty name: `{pi.Name}`\nType name: `{pi.DeclaringType}`");
                this.GetValue = pi.GetValue;
                this.SetValue = pi.SetValue;
                this.FieldType = pi.PropertyType.GetTypeInfo();
            }
            else
                throw new ArgumentException($"Unsupproted member.\nMember name: `{info.Name}`\nType name: `{info.DeclaringType}`");

            FieldCanBeNull = TypeTraits.Of(FieldType.AsType()).CanBeNull;
            if (Nullable.GetUnderlyingType(FieldType.AsType()) is Type nuType)
                FieldType = nuType.GetTypeInfo();
            var t = FieldType.AsType();
            if (t == typeof(int))
                FieldTypeCode = TypeCode.Int32;
            else if (t == typeof(uint))
                FieldTypeCode = TypeCode.UInt32;
            else if (t == typeof(double))
                FieldTypeCode = TypeCode.Double;
            else if (t == typeof(float))
                FieldTypeCode = TypeCode.Single;
            else if (t == typeof(bool))
                FieldTypeCode = TypeCode.Boolean;
            else if (t == typeof(string))
                FieldTypeCode = TypeCode.String;
            else if (t == typeof(char))
                FieldTypeCode = TypeCode.Char;
            else if (t == typeof(byte))
                FieldTypeCode = TypeCode.Byte;
            else if (t == typeof(sbyte))
                FieldTypeCode = TypeCode.SByte;
            else if (t == typeof(long))
                FieldTypeCode = TypeCode.Int64;
            else if (t == typeof(ulong))
                FieldTypeCode = TypeCode.UInt64;
            else if (t == typeof(short))
                FieldTypeCode = TypeCode.Int16;
            else if (t == typeof(ushort))
                FieldTypeCode = TypeCode.UInt16;
            else if (t == typeof(decimal))
                FieldTypeCode = TypeCode.Decimal;
            else if (t == typeof(DateTime))
                FieldTypeCode = TypeCode.DateTime;
            else
                FieldTypeCode = TypeCode.Object;
        }
    }
}
