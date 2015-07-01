using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AssLoader.Serializer;
using System.ComponentModel;
using System.Threading;

namespace AssLoader
{
    public abstract class Entry : INotifyPropertyChanged
    {
        protected Entry()
        {
            var type = this.GetType();
            if(fieldInfoCache.ContainsKey(type))
                fieldInfo = fieldInfoCache[type];
            else
            {
                fieldInfo = new Dictionary<string, FieldSerializeHelper>(StringComparer.OrdinalIgnoreCase);
                do
                {
                    foreach(var fInfo in type.GetRuntimeFields())
                    {
                        var att = fInfo.GetCustomAttribute(typeof(EntryFieldAttribute)) as EntryFieldAttribute;
                        if(att == null)
                            continue;
                        var ser = (SerializeAttribute)fInfo.GetCustomAttribute<SerializeAttribute>();
                        var helper = new FieldSerializeHelper(fInfo, att, ser);
                        fieldInfo.Add(att.Name, helper);
                        if(!string.IsNullOrEmpty(att.Alias))
                            fieldInfo.Add(att.Alias, helper);
                    }
                } while((type = type.GetTypeInfo().BaseType) != typeof(Entry));
                fieldInfoCache.Add(this.GetType(), fieldInfo);
            }
        }

        private static Dictionary<Type, Dictionary<string, FieldSerializeHelper>> fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldSerializeHelper>>();

        private Dictionary<string, FieldSerializeHelper> fieldInfo;

        protected abstract string EntryName
        {
            get;
        }

        public string Serialize(EntryHeader format)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            var r = new EntryData(format.Select(key => fieldInfo[key].Serialize(this)).ToArray());
            return string.Format(FormatHelper.DefaultFormat, "{0}: {1}", EntryName, r.ToString());
        }

        protected void Parse(string fields, EntryHeader format)
        {
            var data = new EntryData(fields, format.Count);
            for(int i = 0; i < format.Count; i++)
            {
                FieldSerializeHelper target;
                if(!fieldInfo.TryGetValue(format[i], out target))
                    continue;
                target.Deserialize(this, data[i]);
            }
        }

        protected void ParseExact(string fields, EntryHeader format)
        {
            var data = new EntryData(fields, format.Count);
            for(int i = 0; i < format.Count; i++)
                fieldInfo[format[i]].Deserialize(this, data[i]);
        }

        protected T Clone<T>() where T : Entry, new()
        {
            var re = new T();
            foreach(var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void PropertyChanging([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            if(PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
