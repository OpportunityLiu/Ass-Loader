using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Custom serializer for <see cref="bool"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BooleanSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Serialize result of true, default value is "True".
        /// </summary>
        public string TrueString { get; set; } = "True";

        /// <summary>
        /// Serialize result of false, default value is "False".
        /// </summary>
        public string FalseString { get; set; } = "False";

        /// <summary>
        /// Convert <see cref="bool"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override void Serialize(TextWriter writer, object value, ISerializeInfo serializeInfo)
        {
            if ((bool)value)
                writer.Write(this.TrueString);
            else
                writer.Write(this.FalseString);
        }

        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            if (value.Equals(this.FalseString.AsSpan(), StringComparison.OrdinalIgnoreCase))
                return false;
            else if (value.Equals(this.TrueString.AsSpan(), StringComparison.OrdinalIgnoreCase))
                return true;
            deserializeInfo.AddException(new FormatException($"Convert failed, the string to deserialize ({value.ToString()}) is neither `{FalseString}` nor `{TrueString}`"));
            return null;
        }
    }
}
