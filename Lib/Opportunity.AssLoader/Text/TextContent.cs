using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Text of <see cref="SubEvent"/>, support methods to modify the text.
    /// This class is immutable.
    /// </summary>
    public sealed partial class TextContent : IEquatable<TextContent>
    {
        private static readonly string[] sep = new[] { "\r\n", "\r", "\n" };

        private TextContent()
        {
        }

        /// <summary>
        /// Create new instance of <see cref="TextContent"/> with <paramref name="value"/> as its content.
        /// Line breaks in <paramref name="value"/> will be replaced by @"\N".
        /// </summary>
        /// <param name="value">The content of text.</param>
        public TextContent(string value)
        {
            if (string.IsNullOrEmpty(value))
                this.Value = "";
            else
            {
                var s = value.Trim().Split(sep, StringSplitOptions.None);
                this.Value = s.Length == 1 ? s[0] : string.Join(@"\N", s);
            }
            this.init();
        }

        private void init()
        {
            var stext = new List<string>();
            var start = 0;
            var b = false;
            for (var i = 0; i < this.Value.Length; i++)
            {
                switch (this.Value[i])
                {
                case '{':
                    if (!b)
                    {
                        b = true;
                        stext.Add(this.Value.Substring(start, i - start));
                        start = i + 1;
                    }
                    break;
                case '}':
                    if (b)
                    {
                        b = false;
                        stext.Add(this.Value.Substring(start, i - start));
                        start = i + 1;
                    }
                    break;
                }
            }
            if (b)
                stext[stext.Count - 1] += this.Value.Substring(start - 1);
            else
                stext.Add(this.Value.Substring(start));
            this.splitedTexts = stext.ToArray();
            this.Tags = new TagList(this.splitedTexts);
            this.Texts = new TextList(this.splitedTexts);
        }

        internal static TextContent Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Empty;
            var re = new TextContent()
            {
                Value = value
            };
            re.init();
            return re;
        }

        private string[] splitedTexts;

        /// <summary>
        /// Content of this <see cref="TextContent"/>.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the list of tags in this <see cref="TextContent"/>.
        /// </summary>
        public TagList Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the list of texts in this <see cref="TextContent"/>.
        /// </summary>
        public TextList Texts
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns <see cref="Value"/>, which is the string form of this <see cref="TextContent"/>.
        /// </summary>
        /// <returns><see cref="Value"/> of this <see cref="TextContent"/>.</returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Returns a new <see cref="TextContent"/> that removed all tags of this <see cref="TextContent"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="TextContent"/> that removed tags.</returns>
        public TextContent RemoveTags()
        {
            if (this.splitedTexts.Length == 1)
                return this;
            var builder = new StringBuilder(this.Value.Length);
            for (var i = 0; i < this.splitedTexts.Length; i += 2)
                builder.Append(this.splitedTexts[i]);
            return builder.ToString();
        }

        /// <summary>
        /// Returns a new <see cref="TextContent"/> that removed all texts of this <see cref="TextContent"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="TextContent"/> that removed texts.</returns>
        public TextContent RemoveTexts()
        {
            if (this.splitedTexts.Length == 1)
                return Empty;
            var builder = new StringBuilder(this.Value.Length);
            builder.Append('{');
            for (var i = 1; i < this.splitedTexts.Length; i += 2)
                builder.Append(this.splitedTexts[i]);
            builder.Append('}');
            return builder.ToString();
        }

        /// <summary>
        /// Returns a new <see cref="TextContent"/> that replaced the text of <paramref name="index"/> of this <see cref="TextContent"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="TextContent"/> that replaced text.</returns>
        /// <param name="index">The index of text to replace.</param>
        /// <param name="newText">The text to replace the old text.</param>
        /// <returns>A new instance of <see cref="TextContent"/> that replaced text.</returns>
        public TextContent ReplaceText(int index, string newText)
        {
            newText = newText ?? "";
            if (newText == this.Texts[index])
                return this;
            index = index * 2;
            var builder = new StringBuilder(this.Value.Length + newText.Length);
            for (var i = 0; i < this.splitedTexts.Length; i++)
            {
                if (i == index)
                    builder.Append(newText);
                else
                    builder.Append(this.splitedTexts[i]);
                if (++i >= this.splitedTexts.Length)
                    break;
                builder.Append('{');
                builder.Append(this.splitedTexts[i]);
                builder.Append('}');
            }
            return builder.ToString();
        }


        /// <summary>
        /// Returns a new <see cref="TextContent"/> that replaced the text of <paramref name="index"/> of this <see cref="TextContent"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="TextContent"/> that replaced text.</returns>
        /// <param name="index">The index of text to replace.</param>
        /// <param name="newTag">The text to replace the old text.</param>
        /// <returns>A new instance of <see cref="TextContent"/> that replaced text.</returns>
        public TextContent ReplaceTag(int index, string newTag)
        {
            newTag = newTag ?? "";
            if (newTag == this.Tags[index])
                return this;
            index = index * 2 + 1;
            var builder = new StringBuilder(this.Value.Length + newTag.Length);
            for (var i = 0; i < this.splitedTexts.Length; i++)
            {
                builder.Append(this.splitedTexts[i]);
                if (++i >= this.splitedTexts.Length)
                    break;
                builder.Append('{');
                if (i == index)
                    builder.Append(newTag);
                else
                    builder.Append(this.splitedTexts[i]);
                builder.Append('}');
            }
            return builder.ToString();
        }

        /// <summary>
        /// Convert a <see cref="TextContent"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The <see cref="TextContent"/> to convert.</param>
        /// <returns><see cref="Value"/> of <paramref name="value"/>.</returns>
        public static implicit operator string(TextContent value) => value?.Value;

        /// <summary>
        /// Convert a <see cref="string"/> to <see cref="TextContent"/>.
        /// Line breaks in <paramref name="value"/> will be replaced by @"\N".
        /// </summary>
        /// <param name="value">The content of text.</param>
        /// <returns>A new instance of <see cref="TextContent"/> made by <see cref="TextContent(string)"/>.</returns>
        public static implicit operator TextContent(string value) => value == null ? null : new TextContent(value);

        /// <summary>
        /// Returns whatever two <see cref="TextContent"/> are equal.
        /// </summary>
        /// <param name="left">The first <see cref="TextContent"/> to compare.</param>
        /// <param name="right">The second <see cref="TextContent"/> to compare.</param>
        /// <returns>True if the two <see cref="TextContent"/> are equal.</returns>
        public static bool operator ==(TextContent left, TextContent right)
        {
            return left?.Value == right?.Value;
        }

        /// <summary>
        /// Returns whatever two <see cref="TextContent"/> are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="TextContent"/> to compare.</param>
        /// <param name="right">The second <see cref="TextContent"/> to compare.</param>
        /// <returns>True if the two <see cref="TextContent"/> are not equal.</returns>
        public static bool operator !=(TextContent left, TextContent right)
        {
            return left?.Value != right?.Value;
        }

        #region IEquatable<TextContent> 成员

        /// <summary>
        /// Returns whatever two <see cref="TextContent"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="TextContent"/> to compare with this <see cref="TextContent"/>.</param>
        /// <returns>True if the two <see cref="TextContent"/> are equal.</returns>
        public bool Equals(TextContent other)
        {
            return other?.Value == this.Value;
        }

        #endregion

        /// <summary>
        /// Returns whatever two <see cref="TextContent"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="TextContent"/>.</param>
        /// <returns>True if the two <see cref="TextContent"/> are equal.</returns>
        public override bool Equals(object obj) => this.Equals(obj as TextContent);

        /// <summary>
        /// Returns the hash code of the <see cref="TextContent"/>.
        /// </summary>
        /// <returns>The hash code of the <see cref="TextContent"/>.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        /// Empty <see cref="TextContent"/>, whose <see cref="Value"/> is <see cref="string.Empty"/>.
        /// </summary>
        public static readonly TextContent Empty = string.Empty;
    }
}
