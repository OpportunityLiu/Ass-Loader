using System;
using System.Collections.Generic;
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
        /// Whether <see cref="FormatException"/> will be thrown if the string doesn't match <see cref="TrueString"/> or <see cref="FalseString"/>.
        /// </summary>
        public bool ThrowOnDeserializing { get; set; }

        /// <summary>
        /// Convert <see cref="bool"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override string Serialize(object value)
        {
            if ((bool)value)
                return this.TrueString;
            else
                return this.FalseString;
        }

        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> doesn't match <see cref="TrueString"/> or <see cref="FalseString"/> while <see cref="ThrowOnDeserializing"/> is true.</exception>
        public override object Deserialize(ReadOnlySpan<char> value)
        {
            if (value.Equals(this.FalseString.AsSpan(), StringComparison.OrdinalIgnoreCase))
                return false;
            else if (value.Equals(this.TrueString.AsSpan(), StringComparison.OrdinalIgnoreCase))
                return true;
            else if (this.ThrowOnDeserializing)
                throw new FormatException($"Convert failed, the string to deserialize is:\n{value.ToString()}");
            else
                return false;
        }
    }
}
