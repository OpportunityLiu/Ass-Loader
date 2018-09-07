using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// A list of entry names for serializing and deserializing.
    /// </summary>
    public sealed class EntryHeader : IEquatable<EntryHeader>, IReadOnlyList<string>
    {
        /// <summary>
        /// Create new instance of <see cref="EntryHeader"/> with a <see cref="string"/> of entry names.
        /// </summary>
        /// <param name="format">A <see cref="string"/> of entry names that seperates with ','.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="format"/> contains entry name that is <see cref="string.Empty"/>.</exception>
        /// <exception cref="FormatException"><paramref name="format"/> contains repeated entry names.</exception>
        public EntryHeader(string format)
        {
            if(string.IsNullOrEmpty(format))
                throw new ArgumentNullException(nameof(format));
            this.data = new EntryData(format, int.MaxValue);
            if(this.data.Contains(string.Empty))
                throw new ArgumentException("Header can't contains string.Empty.", nameof(format));
            if(this.data.Distinct().Count() != this.data.Count)
                throw new FormatException("Header can't contains repeated strings.");
        }

        /// <summary>
        /// Create new instance of <see cref="EntryHeader"/> with strings of entry names.
        /// </summary>
        /// <param name="format">strings of entry names</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null.</exception>
        public EntryHeader(IEnumerable<string> format)
        {
            if(format == null)
                throw new ArgumentNullException(nameof(format));
            this.data = format.Distinct(StringComparer.OrdinalIgnoreCase).Select(s => s.Trim()).ToList();
        }

        /// <summary>
        /// Returns the ass format of this <see cref="EntryHeader"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> presents the ass format of this <see cref="EntryHeader"/>.</returns>
        public override string ToString()
        {
            return string.Format(FormatHelper.DefaultFormat, "Format: {0}", this.data.ToString());
        }

        private IReadOnlyList<string> data;

        #region IEquatable<FormatEntry> 成员

        /// <summary>
        /// Returns whatever two <see cref="EntryHeader"/> are equal, ignore differences of the order of entry names.
        /// </summary>
        /// <param name="other">The <see cref="EntryHeader"/> to compare with this <see cref="EntryHeader"/>.</param>
        /// <returns>True if the two <see cref="EntryHeader"/> are equal.</returns>
        public bool Equals(EntryHeader other)
        {
            if(other == null)
                return false;
            if(ReferenceEquals(this, other))
                return true;
            if(this.data.Count != other.data.Count)
                return false;
            return this.data.Count == Enumerable.Join(this.data, other.data, s => s, s => s, (o, i) => 0, StringComparer.OrdinalIgnoreCase).Count();
        }

        #endregion

        /// <summary>
        /// Returns whatever two <see cref="EntryHeader"/> are equal, ignore differences of the order of entry names.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="EntryHeader"/>.</param>
        /// <returns>True if the two <see cref="EntryHeader"/> are equal.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EntryHeader);
        }

        /// <summary>
        /// Get hash code of this <see cref="EntryHeader"/>, ignore differences of the order of entry names.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.data.Aggregate(0, (o, n) => o ^ StringComparer.OrdinalIgnoreCase.GetHashCode(n));
        }

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        #endregion

        #region IReadOnlyList<string> 成员

        /// <summary>
        /// Get the entry name with the given index.
        /// </summary>
        /// <param name="index">The index of entry names, starts from 0.</param>
        /// <returns>The entry name with the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> out of range.</exception>
        public string this[int index] => this.data[index];

        #endregion

        #region IReadOnlyCollection<string> 成员

        /// <summary>
        /// Get the number of entry names of this <see cref="EntryHeader"/>.
        /// </summary>
        public int Count => this.data.Count;

        #endregion

        #region IEnumerable<string> 成员

        /// <summary>
        /// Get the enumerator of entry names of this <see cref="EntryHeader"/>.
        /// </summary>
        /// <returns>The enumerator of entry names of this <see cref="EntryHeader"/>.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        #endregion
    }
}
