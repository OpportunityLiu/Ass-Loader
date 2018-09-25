using System;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// Order of the field, value less than zero means count backwards.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// An index to help handling optional parameters.
        /// <see cref="TagFieldAttribute"/> with smaller <see cref="Priority"/> will be handled first.
        /// </summary>
        public int Priority { get; set; }
    }

    /// <summary>
    /// Serialization infomation of <see cref="Tag"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TagDefinationAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of <see cref="TagDefinationAttribute"/>.
        /// </summary>
        /// <param name="name">Name of <see cref="Tag"/>.</param>
        /// <param name="aliases">Aliases of <see cref="Tag"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> or <paramref name="aliases"/> is not a valid tag name.</exception>
        public TagDefinationAttribute(string name, params string[] aliases)
        {
            TagNameCheck(ref name);
            aliases = aliases ?? Array.Empty<string>();
            for (var i = 0; i < aliases.Length; i++)
            {
                TagNameCheck(ref aliases[i]);
            }
            this.Name = name;
            this.Aliases = aliases;
        }

        private static readonly Regex nameRegex = new Regex(@"^[0-9]*[A-Za-z]*$", RegexOptions.Compiled | RegexOptions.Singleline);

        internal static void TagNameCheck(ref string name)
        {
            try
            {
                if (!FormatHelper.InlineDataValueValid(ref name))
                    throw new ArgumentNullException(nameof(name));
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Name or alias `{name}` is invalid.", ex);
            }
            if (!nameRegex.IsMatch(name))
                throw new ArgumentException($"Name or alias `{name}` is invalid.");
        }

        /// <summary>
        /// Name of <see cref="Tag"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Aliases of <see cref="Tag"/>.
        /// </summary>
        public string[] Aliases { get; }
    }
}
