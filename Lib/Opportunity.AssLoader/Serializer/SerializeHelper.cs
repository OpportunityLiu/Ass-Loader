using Opportunity.AssLoader.Serializer;
using Opportunity.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    [DebuggerDisplay(@"{((global::System.Reflection.MemberInfo)GetValue.Target).Name,nq}")]
    internal sealed class SerializeHelper<TObj, TFieldInfo>
        where TFieldInfo : SerializableFieldAttribute
    {
        public static Dictionary<string, SerializeHelper<TObj, TFieldInfo>> GetScriptInfoFields(Type containerType)
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
            return (from dict in temp.Reverse<Dictionary<string, SerializeHelper<TObj, TFieldInfo>>>()
                    from entry in dict
                    select entry).ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
        }

        public TFieldInfo Info { get; }
        public SerializeAttribute Serializer { get; }
        public TypeInfo FieldType { get; }
        public bool FieldCanBeNull { get; }

        public GetValueDelegate GetValue { get; }
        public SetValueDelegate SetValue { get; }

        public void Deserialize(ReadOnlySpan<char> value, TObj obj, IDeserializeInfo deserializeInfo)
        {
            var error = default(Exception);
            try
            {
                var va = default(object);

                if (Serializer is null)
                {
                    if (FieldCanBeNull && value.IsEmpty)
                        va = null;
                    else if (FieldType.IsEnum)
                        va = Enum.Parse(FieldType.AsType(), value.ToString(), true);
                    else if (FieldType == typeof(int) || FieldType == typeof(long) || FieldType == typeof(short) || FieldType == typeof(byte))
                        va = Convert.ChangeType(double.Parse(value.ToString(), FormatHelper.DefaultFormat), FieldType.AsType(), FormatHelper.DefaultFormat);
                    else
                        va = Convert.ChangeType(value.ToString(), FieldType.AsType(), FormatHelper.DefaultFormat);
                }
                else
                    va = Serializer.Deserialize(value, deserializeInfo);
                SetValue(obj, va);
            }
            catch(TargetInvocationException ex)
            {
                error = ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                error = ex;
            }
            if (error != null)
            {
                deserializeInfo.AddException(error);
                SetValue(obj, Info.DefaultValue);
            }
        }

        public void Serialize(TextWriter writer, TObj obj, ISerializeInfo serializeInfo)
        {
            var value = GetValue(obj);
            if (value is null || Equals(value, Info.DefaultValue))
                return;
            if (value is null)
                value = Info.DefaultValue;
            if (Serializer is null)
            {
                var f = value as IFormattable;
                if (Info.Format.IsNullOrWhiteSpace() || f is null)
                    writer.Write(value);
                else
                    writer.Write(f.ToString(Info.Format, FormatHelper.DefaultFormat));
            }
            else
                Serializer.Serialize(writer, value, serializeInfo);
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
        }
    }
}
