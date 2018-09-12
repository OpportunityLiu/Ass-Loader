using System;

namespace Opportunity.AssLoader.Text
{
    /// <summary>
    /// Serialization infomation of field of <see cref="Tag"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TagFieldAttribute : SerializableFieldAttribute
    {
        /// <summary>
        /// Create new instance of <see cref="TagFieldAttribute"/>.
        /// </summary>
        /// <param name="order">Order of the field.</param>
        public TagFieldAttribute(int order) : base(order.ToString())
        {
            this.Order = order;
        }

        /// <summary>
        /// Order of the field, value less than zero means cound backwards.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// An index to help handling optional parameters.
        /// TagFieldAttribute with same <see cref="SelctionGroup"/> will be regarded as an overload.
        /// </summary>
        public int SelctionGroup { get; set; }
    }
}
