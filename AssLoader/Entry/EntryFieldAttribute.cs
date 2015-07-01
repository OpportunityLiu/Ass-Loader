using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EntryFieldAttribute : Attribute
    {
        public EntryFieldAttribute(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Alias
        {
            get;
            set;
        }

        private object defaultValue;

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

        private string format = string.Empty;

        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
            }
        }
    }
}

