using Opportunity.AssLoader.Effects;
using Opportunity.AssLoader.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Custom serializer for <see cref="TextContent"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class TextSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="TextContent"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>   
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
            => TextContent.Parse(value);
    }

    /// <summary>
    /// Custom serializer for <see cref="Color"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ColorSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>   
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            try
            {
                return Color.Parse(value);
            }
            catch (Exception ex)
            {
                deserializeInfo.AddException(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Custom serializer for <see cref="Effects.Effect"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class EffectSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Convert <see cref="Effect"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write result to.</param>
        /// <param name="serializeInfo">Helper interface for serializing.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override void Serialize(TextWriter writer, object value, ISerializeInfo serializeInfo)
        {
            ((Effect)value).Serialize(writer, serializeInfo);
        }

        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="Effect"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="deserializeInfo">Helper interface for deserializing.</param>
        /// <returns>The result of convertion.</returns>   
        public override object Deserialize(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            return value.IsWhiteSpace() ? null : Effect.Parse(value, deserializeInfo);
        }
    }
}
