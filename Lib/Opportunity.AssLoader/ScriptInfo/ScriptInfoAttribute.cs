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
    /// Serialization infomation of field of <see cref="ScriptInfoCollection"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    [DebuggerDisplay(@"[{FieldName,nq}] = {DefaultValue}")]
    public sealed class ScriptInfoAttribute : SerializableFieldAttribute
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
    }
}
