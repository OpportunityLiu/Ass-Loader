using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AssLoader
{
    /// <summary>
    /// Text of <see cref="SubEvent"/>, support methods to modify the text.
    /// </summary>
    public sealed class TextContent : IEquatable<TextContent>
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
            if(string.IsNullOrEmpty(value))
                this.Value = "";
            else
            {
                var s = value.Trim().Split(sep, StringSplitOptions.None);
                if(s.Length == 1)
                    this.Value = s[0];
                else
                    this.Value = string.Join(@"\N", s);
            }
            init();
        }

        private void init()
        {
            var stext = new List<string>();
            var start = 0;
            var b = false;
            for(int i = 0; i < Value.Length; i++)
            {
                switch(Value[i])
                {
                case '{':
                    if(!b)
                    {
                        b = true;
                        stext.Add(Value.Substring(start, i - start));
                        start = i + 1;
                    }
                    break;
                case '}':
                    if(b)
                    {
                        b = false;
                        stext.Add(Value.Substring(start, i - start));
                        start = i + 1;
                    }
                    break;
                }
            }
            if(b)
                stext[stext.Count - 1] += Value.Substring(start - 1);
            else
                stext.Add(Value.Substring(start));
            this.splitedTexts = stext.ToArray();
            this.Tags = new TagList(splitedTexts);
            this.Texts = new TextList(splitedTexts);
        }

        internal static TextContent Parse(string value)
        {
            if(string.IsNullOrEmpty(value))
                return Empty;
            var re = new TextContent()
            {
                Value = value
            };
            re.init();
            return re;
        }

        private string[] splitedTexts;

        public string Value
        {
            get;
            private set;
        }

        public struct TextList : IReadOnlyList<string>
        {
            private string[] texts;
            private int max;

            internal TextList(string[] texts)
            {
                this.texts = texts;
                this.max = texts.Length / 2 + 1;
            }

            #region IReadOnlyList<string> 成员

            public string this[int index]
            {
                get
                {
                    ThrowHelper.ThrowIfLessThanZeroOrOutOfRange(max, index, "index");
                    return texts[index * 2];
                }
            }

            #endregion

            #region IReadOnlyCollection<string> 成员

            public int Count => max;

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                for(int i = 0; i < texts.Length; i+=2)
                    yield return texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

            #endregion
        }

        public struct TagList : IReadOnlyList<string>
        {
            private string[] texts;
            private int max;

            internal TagList(string[] texts)
            {
                this.texts = texts;
                this.max = texts.Length / 2;
            }

            #region IReadOnlyList<string> 成员

            public string this[int index]
            {
                get
                {
                    ThrowHelper.ThrowIfLessThanZeroOrOutOfRange(max, index, "index");
                    return texts[index * 2 + 1];
                }
            }

            #endregion

            #region IReadOnlyCollection<string> 成员

            public int Count => max;

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                for(int i = 1; i < texts.Length; i+=2)
                    yield return texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

            #endregion
        }

        public TagList Tags
        {
            get;
            private set;
        }

        public TextList Texts
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Value;
        }

        public TextContent RemoveTags()
        {
            if(splitedTexts.Length == 1)
                return this;
            var builder = new StringBuilder(Value.Length);
            for(var i = 0; i < splitedTexts.Length; i += 2)
                builder.Append(splitedTexts[i]);
            return builder.ToString();
        }

        public TextContent RemoveTexts()
        {
            if(splitedTexts.Length == 1)
                return Empty;
            var builder = new StringBuilder(Value.Length);
            builder.Append('{');
            for(var i = 1; i < splitedTexts.Length; i += 2)
                builder.Append(splitedTexts[i]);
            builder.Append('}');
            return builder.ToString();
        }

        public TextContent ReplaceText(int index, string newText)
        {
            newText = newText ?? "";
            if(newText == Texts[index])
                return this;
            index = index * 2;
            var builder = new StringBuilder(Value.Length + newText.Length);
            for(int i = 0; i < splitedTexts.Length; i++)
            {
                if(i == index)
                    builder.Append(newText);
                else
                    builder.Append(splitedTexts[i]);
                if(++i >= splitedTexts.Length)
                    break;
                builder.Append('{');
                builder.Append(splitedTexts[i]);
                builder.Append('}');
            }
            return builder.ToString();
        }
        
        public TextContent ReplaceTag(int index, string newTag)
        {
            newTag = newTag ?? "";
            if(newTag == Tags[index])
                return this;
            index = index * 2 + 1;
            var builder = new StringBuilder(Value.Length + newTag.Length);
            for(int i = 0; i < splitedTexts.Length; i++)
            {
                builder.Append(splitedTexts[i]);
                if(++i >= splitedTexts.Length)
                    break;
                builder.Append('{');
                if(i == index)
                    builder.Append(newTag);
                else
                    builder.Append(splitedTexts[i]);
                builder.Append('}');
            }
            return builder.ToString();
        }

        /// <summary>
        /// Convert a <see cref="TextContent"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The <see cref="TextContent"/> to convert.</param>
        /// <returns><see cref="Value"/> of <paramref name="value"/>.</returns>
        public static implicit operator string (TextContent value) => value?.Value;

        /// <summary>
        /// Convert a <see cref="string"/> to <see cref="TextContent"/>.
        /// Line breaks in <paramref name="value"/> will be replaced by @"\N".
        /// </summary>
        /// <param name="value">The content of text.</param>
        /// <returns>A new instance of <see cref="TextContent"/> made by <see cref="TextContent(string)"/>.</returns>
        public static implicit operator TextContent(string value) => new TextContent(value);

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
        public override bool Equals(object obj) => Equals(obj as TextContent);

        /// <summary>
        /// Returns the hash code of the <see cref="TextContent"/>.
        /// </summary>
        /// <returns>The hash code of the <see cref="TextContent"/>.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Empty <see cref="TextContent"/>, whose <see cref="Value"/> is <see cref="string.Empty"/>.
        /// </summary>
        public static readonly TextContent Empty = "";
    }
}
