using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Custom serializer for <see cref="Color"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ColorSerializeAttribute : SerializeAttribute
    {
        /// <summary>
        /// Convert <see cref="Color"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>
        public override string Serialize(object value) => value.ToString();

        /// <summary>
        /// Convert <see cref="ReadOnlySpan{T}"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of convertion.</returns>   
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid color string.</exception>
        public override object Deserialize(ReadOnlySpan<char> value) => Color.Parse(value);
    }
}
