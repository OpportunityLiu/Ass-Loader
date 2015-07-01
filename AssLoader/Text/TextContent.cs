using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AssLoader
{
    public sealed class TextContent : IEquatable<TextContent>
    {
        private static readonly string[] sep = new[] { "\r\n", "\r", "\n" };

        private TextContent()
        {
        }

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
            this.Tags = new TagsList(splitedTexts);
            this.Texts = new TextsList(splitedTexts);
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

        public struct TextsList : IReadOnlyList<string>
        {
            private string[] texts;
            private int max;

            internal TextsList(string[] texts)
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

            public int Count
            {
                get
                {
                    return max;
                }
            }

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                for(int i = 0; i < texts.Length; i+=2)
                    yield return texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        public struct TagsList : IReadOnlyList<string>
        {
            private string[] texts;
            private int max;

            internal TagsList(string[] texts)
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

            public int Count
            {
                get
                {
                    return max;
                }
            }

            #endregion

            #region IEnumerable<string> 成员

            public IEnumerator<string> GetEnumerator()
            {
                for(int i = 1; i < texts.Length; i+=2)
                    yield return texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        public TagsList Tags
        {
            get;
            private set;
        }

        public TextsList Texts
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

        public static implicit operator string(TextContent value)
        {
            return value.Value;
        }

        public static implicit operator TextContent(string value)
        {
            return new TextContent(value);
        }

        public static bool operator ==(TextContent a, TextContent b)
        {
            if(a != null)
                return a.Equals(b);
            else
                return b == null;
        }

        public static bool operator !=(TextContent a, TextContent b)
        {
            return !(a == b);
        }

        #region IEquatable<TextContent> 成员

        public bool Equals(TextContent other)
        {
            if(other == null)
                return false;
            return other.Value == this.Value;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as TextContent);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static readonly TextContent Empty = "";
    }
}
