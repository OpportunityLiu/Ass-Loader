using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AssLoader.Serializer;
using System.ComponentModel;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AssLoader
{
    /// <summary>
    /// Entry of ass file.
    /// </summary>
    public abstract class Entry : INotifyPropertyChanged
    {
        /// <summary>
        /// Create new instance of <see cref="Entry"/>.
        /// </summary>
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

        /// <summary>
        /// Name of this <see cref="Entry"/>.
        /// </summary>
        protected abstract string EntryName
        {
            get;
        }

        /// <summary>
        /// Returns the ass form of this <see cref="Entry"/>, with the given <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <returns>The <see cref="string"/> presents this <see cref="Entry"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null.</exception>
        public string Serialize(EntryHeader format)
        {
            if(format == null)
                throw new ArgumentNullException(nameof(format));
            var r = new EntryData(format.Select(key => fieldInfo[key].Serialize(this)).ToArray());
            return string.Format(FormatHelper.DefaultFormat, "{0}: {1}", EntryName, r.ToString());
        }

        /// <summary>
        /// Parse from <paramref name="fields"/>, deserialize and save to this <see cref="Entry"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        protected void Parse(string fields, EntryHeader format)
        {
            if(format == null)
                throw new ArgumentNullException(nameof(format));
            if(string.IsNullOrEmpty(fields))
                throw new ArgumentNullException(nameof(fields));
            var data = new EntryData(fields, format.Count);
            for(int i = 0; i < format.Count; i++)
            {
                FieldSerializeHelper target;
                if(!fieldInfo.TryGetValue(format[i], out target))
                    continue;
                target.Deserialize(this, data[i]);
            }
        }

        /// <summary>
        /// Parse exactly from <paramref name="fields"/>, deserialize and save to this <see cref="Entry"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Fields of <see cref="Entry"/> and fields of <paramref name="format"/> doesn't match
        /// </exception>
        protected void ParseExact(string fields, EntryHeader format)
        {
            if(format == null)
                throw new ArgumentNullException(nameof(format));
            if(string.IsNullOrEmpty(fields))
                throw new ArgumentNullException(nameof(fields));
            var data = new EntryData(fields, format.Count);
            for(int i = 0; i < format.Count; i++)
                fieldInfo[format[i]].Deserialize(this, data[i]);
        }

        /// <summary>
        /// Make a copy of this <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A subclass of <see cref="Entry"/>.</typeparam>
        /// <returns>A copy of this <typeparamref name="T"/>.</returns>
        protected T Clone<T>() where T : Entry, new()
        {
            var re = new T();
            foreach(var item in fieldInfo.Values)
                item.SetValue(re, item.GetValue(this));
            return re;
        }

        #region INotifyPropertyChanged 成员
        /// <summary>
        /// Occurs when the property changes.
        /// </summary>
        /// <remarks>
        /// The event handler receives an argument of type
        /// <seealso cref="PropertyChangedEventHandler" />
        /// containing data related to this event.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the event <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changing property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
