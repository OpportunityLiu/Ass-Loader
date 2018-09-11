using System;
using System.IO;
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
        /// Convert <see cref="Enum"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override void Serialize(TextWriter writer, object value, ISerializeInfo serializeInfo)
        {
            var c = (IConvertible)value;
            writer.Write(c.ToDouble(FormatHelper.DefaultFormat).ToString(FormatHelper.DefaultFormat));
        }

        /// <summary>
        /// Convert number to <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>
        /// <exception cref="FormatException">Failed to parse the string.</exception>
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            try
            {
                return Enum.Parse(EnumType, value.ToString());
            }
            catch (Exception ex)
            {
                deserializeInfo.AddException(ex);
                return null;
            }
        }
    }
}
