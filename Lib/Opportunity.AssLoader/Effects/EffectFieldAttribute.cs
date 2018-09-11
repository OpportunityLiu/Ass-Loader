using System;

namespace Opportunity.AssLoader.Effects
{
    /// <summary>
    /// Serialization infomation of field of <see cref="Effect"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EffectFieldAttribute : SerializableFieldAttribute
    {
        /// <summary>
        /// Create new instance of <see cref="EffectFieldAttribute"/>.
        /// </summary>
        /// <param name="order">Order of the field.</param>
        public EffectFieldAttribute(int order) : base(order.ToString())
        {
            this.Order = order;
        }

        /// <summary>
        /// Order of the field.
        /// </summary>
        public int Order { get; }
    }
}
