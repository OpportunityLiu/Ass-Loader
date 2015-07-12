using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{    /// <summary>
     /// Infomation of field of <see cref="Entry"/>.
     /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EntryFieldAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of <see cref="EntryFieldAttribute"/>.
        /// </summary>
        /// <param name="name">name of the field in the ass file.</param>
        public EntryFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the field in the ass file.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Alias of the field in the ass file.
        /// </summary>
        public string Alias
        {
            get;
            set;
        }

        /// <summary>
        /// The default value of the field in <see cref="Entry"/>.
        /// If the field equals null, this value will be used to serialize.
        /// </summary>
        public object DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Will be used as format string if a custom serializer is not provided.
        /// </summary>
        public string Format
        {
            get;
            set;
        }
        = "";
    }
}

