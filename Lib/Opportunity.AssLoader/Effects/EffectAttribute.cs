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

    /// <summary>
    /// Serialization infomation of <see cref="Effect"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EffectDefinationAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of <see cref="EffectFieldAttribute"/>.
        /// </summary>
        /// <param name="name">Name of <see cref="Effect"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid effect name.</exception>
        public EffectDefinationAttribute(string name)
        {
            EffectNameCheck(ref name);
            this.Name = name;
        }

        internal static void EffectNameCheck(ref string name)
        {
            if (!FormatHelper.InlineDataValueValid(ref name))
                throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Name of <see cref="Effect"/>.
        /// </summary>
        public string Name { get; }
    }
}
