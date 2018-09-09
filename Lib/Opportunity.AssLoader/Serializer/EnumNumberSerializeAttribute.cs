using System;
using System.Linq;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Custom serializer for <see cref="Enum"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EnumNumberSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Create new instance of <see cref="EnumNumberSerializeAttribute"/>.
        /// </summary>
        /// <param name="enumType">Type of enum.</param>
        public EnumNumberSerializeAttribute(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
            Enum.GetValues(enumType).Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// Type of enum.
        /// </summary>
        public Type EnumType { get; }

        /// <summary>
        /// Whether <see cref="FormatException"/> will be thrown if the enum value doesn't defined.
        /// </summary>
        public bool ThrowOnDeserializing { get; set; }

        /// <summary>
        /// Convert <see cref="Enum"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override string Serialize(object value)
        {
            var c = (IConvertible)value;
            return c.ToDouble(FormatHelper.DefaultFormat).ToString(FormatHelper.DefaultFormat);
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        /// <exception cref="FormatException">Failed to parse the string.</exception>
        public override object Deserialize(string value)
        {
            try
            {
                return Enum.Parse(EnumType, value);
            }
            catch
            {
                if (ThrowOnDeserializing)
                    throw;
            }
            return Enum.GetValues(EnumType).Cast<object>().FirstOrDefault();
        }
    }
}
