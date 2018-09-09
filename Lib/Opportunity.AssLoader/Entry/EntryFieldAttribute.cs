using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Serialization infomation of field of <see cref="Entry"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    [DebuggerDisplay(@"[{Name,nq}({Alias,nq})] = {DefaultValue}")]
    public sealed class EntryFieldAttribute : SerializableFieldAttribute
    {
        /// <summary>
        /// Create new instance of <see cref="EntryFieldAttribute"/>.
        /// </summary>
        /// <param name="fieldName">name of the field in the ass file.</param>
        public EntryFieldAttribute(string fieldName) : base(fieldName) { }

        /// <summary>
        /// Alias of the field in the ass file, which is defined at the "Format" line.
        /// </summary>
        public string Alias { get; set; }
    }
}

