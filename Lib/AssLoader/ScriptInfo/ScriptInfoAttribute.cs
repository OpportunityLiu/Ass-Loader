using AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    /// <summary>
    /// Infomation of field of <see cref="ScriptInfoCollection"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ScriptInfoAttribute : Attribute
    {
        private readonly string fieldName;

        /// <summary>
        /// Create new instance of <see cref="ScriptInfoAttribute"/>.
        /// </summary>
        /// <param name="fieldName">name of the field in the ass file.</param>
        public ScriptInfoAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }

        /// <summary>
        /// Name of the field in the ass file.
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
        }

        /// <summary>
        /// The field is optional or not,
        /// if true, the field will not be serialized if its value equals null or <see cref="DefaultValue"/>.
        /// </summary>
        public bool IsOptional
        {
            get;
            set;
        }

        /// <summary>
        /// The default value of the field in <see cref="ScriptInfoCollection"/>,
        /// if the field equals null, this value will be used to serialize.
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

#if DEBUG
        /// <summary>
        /// Returns <see cref="FieldName"/>.
        /// </summary>
        /// <returns><see cref="FieldName"/> of this <see cref="ScriptInfoAttribute"/>.</returns>
        public override string ToString()
        {
            return fieldName;
        }
#endif
    }
}
