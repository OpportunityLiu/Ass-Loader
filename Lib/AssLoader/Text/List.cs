using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AssLoader
{
    public sealed partial class TextContent : IEquatable<TextContent>
    {
        /// <summary>
        /// The list of texts of a <see cref="TextContent"/>.
        /// </summary>
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

            /// <summary>
            /// Get the text of <paramref name="index"/> of the <see cref="TextContent"/>.
            /// </summary>
            /// <param name="index">The index to find.</param>
            /// <returns>The text of <paramref name="index"/> of the <see cref="TextContent"/>.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
            public string this[int index]
            {
                get
                {
                    if(ThrowHelper.IsLessThanZeroOrOutOfRange(this.max, index))
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return this.texts[index * 2];
                }
            }

            #endregion

            #region IReadOnlyCollection<string> 成员

            /// <summary>
            /// Count of texts in the <see cref="TextContent"/>.
            /// </summary>
            public int Count => this.max;

            #endregion

            #region IEnumerable<string> 成员

            /// <summary>
            /// Get a enumerator of texts of the <see cref="TextContent"/>.
            /// </summary>
            /// <returns>A enumerator of texts of the <see cref="TextContent"/>.</returns>
            public IEnumerator<string> GetEnumerator()
            {
                for(var i = 0; i < this.texts.Length; i += 2)
                    yield return this.texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
                => this.GetEnumerator();

            #endregion
        }

        /// <summary>
        /// The list of tags of a <see cref="TextContent"/>.
        /// </summary>
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

            /// <summary>
            /// Get the tag of <paramref name="index"/> of the <see cref="TextContent"/>.
            /// </summary>
            /// <param name="index">The index to find.</param>
            /// <returns>The tag of <paramref name="index"/> of the <see cref="TextContent"/>.</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
            public string this[int index]
            {
                get
                {
                    if(ThrowHelper.IsLessThanZeroOrOutOfRange(this.max, index))
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return this.texts[index * 2 + 1];
                }
            }

            #endregion

            #region IReadOnlyCollection<string> 成员

            /// <summary>
            /// Count of tags in the <see cref="TextContent"/>.
            /// </summary>
            public int Count => this.max;

            #endregion

            #region IEnumerable<string> 成员

            /// <summary>
            /// Get a enumerator of tags of the <see cref="TextContent"/>.
            /// </summary>
            /// <returns>A enumerator of tags of the <see cref="TextContent"/>.</returns>
            public IEnumerator<string> GetEnumerator()
            {
                for(var i = 1; i < this.texts.Length; i += 2)
                    yield return this.texts[i];
            }

            #endregion

            #region IEnumerable 成员

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
                => this.GetEnumerator();

            #endregion
        }
    }
}