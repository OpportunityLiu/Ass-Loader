using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Provides custom methods to serialize and deserialize fields of ass file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class SerializeAttribute : Attribute
    {
        /// <summary>
        /// Convert .Net fields to ass file fields.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <param name="value">The value of .Net field.</param>
        /// <returns>The ass file field.</returns>
        public virtual void Serialize(TextWriter writer, object value, ISerializeInfo serializeInfo) => writer.Write(value);

        /// <summary>
        /// Convert ass file fields to .Net fields.
        /// </summary>
        /// <param name="value">The value of ass file field.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The .Net field.</returns>
        public abstract object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo);
    }

    /// <summary>
    /// Interface for deserializing.
    /// </summary>
    public interface IDeserializeInfo
    {
        /// <summary>
        /// Record errors during deserializing.
        /// </summary>
        /// <param name="ex">The exception need to be record.</param>
        void AddException(Exception ex);
    }

    /// <summary>
    /// Interface for serializing.
    /// </summary>
    public interface ISerializeInfo
    {

    }
}
