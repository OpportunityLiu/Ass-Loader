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
    /// Serialization infomation of a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public abstract class SerializableFieldAttribute : Attribute
    {
        internal SerializableFieldAttribute() { }

        /// <summary>
        /// The default value of the field,
        /// if the field equals <see langword="null"/>, this value will be used to serialize.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// The field is optional or not,
        /// if true, the field will not be serialized if its value equals <see langword="null"/> or <see cref="DefaultValue"/>.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Will be used as format string if a custom serializer is not provided.
        /// </summary>
        public string Format { get; set; } = "";
    }
}
