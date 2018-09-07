using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Serializer
{
    /// <summary>
    /// Provides custom methods to serialize and deserialize fields of ass file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class SerializeAttribute : Attribute
    {
        /// <summary>
        /// Convert .Net fields to ass file fields.
        /// </summary>
        /// <param name="value">The value of .Net field.</param>
        /// <returns>The ass file field.</returns>
        public abstract string Serialize(object value);

        /// <summary>
        /// Convert ass file fields to .Net fields.
        /// </summary>
        /// <param name="value">The value of ass file field.</param>
        /// <returns>The .Net field.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> can't be deserialize.</exception>
        public abstract object Deserialize(string value);
    }
}
