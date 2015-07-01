using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ScriptInfoAttribute : Attribute
    {
        private readonly string infoName;

        public ScriptInfoAttribute(string infoName)
        {
            this.infoName = infoName;
        }

        public string InfoName
        {
            get
            {
                return infoName;
            }
        }

        public bool IsOptional
        {
            get;
            set;
        }

        public object DefaultValue
        {
            get;
            set;
        }

        private string format = "";

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

        public override string ToString()
        {
            return infoName;
        }
    }
}
