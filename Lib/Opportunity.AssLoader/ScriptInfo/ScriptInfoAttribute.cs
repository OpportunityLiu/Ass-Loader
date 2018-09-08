using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Infomation of field of <see cref="ScriptInfoCollection"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    [DebuggerDisplay(@"[{FieldName,nq}]")]
    public sealed class ScriptInfoAttribute : Attribute
    {

        /// <summary>
        /// Create new instance of <see cref="ScriptInfoAttribute"/>.
        /// </summary>
        /// <param name="fieldName">name of the field in the ass file.</param>
        public ScriptInfoAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Name of the field in the ass file.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// The field is optional or not,
        /// if true, the field will not be serialized if its value equals null or <see cref="DefaultValue"/>.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// The default value of the field in <see cref="ScriptInfoCollection"/>,
        /// if the field equals null, this value will be used to serialize.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Will be used as format string if a custom serializer is not provided.
        /// </summary>
        public string Format { get; set; } = "";
    }
}
