using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// A list of entry names for serializing and deserializing.
    /// </summary>
    [DebuggerDisplay(@"Format: {string.Join("","", data), nq}")]
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
            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException(nameof(format));
            this.Data = new EntryData(format, int.MaxValue).Fields;
            if (this.Data.Contains(string.Empty))
                throw new ArgumentException("Header can't contains string.Empty.", nameof(format));
            if (this.Data.Distinct().Count() != this.Data.Length)
                throw new FormatException("Header can't contains repeated strings.");
        }

        /// <summary>
        /// Create new instance of <see cref="EntryHeader"/> with strings of entry names.
        /// </summary>
        /// <param name="format">strings of entry names</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null.</exception>
        public EntryHeader(IEnumerable<string> format)
        {
            if (format is null)
                throw new ArgumentNullException(nameof(format));
            this.Data = format.Distinct(StringComparer.OrdinalIgnoreCase).Select(s => s.Trim()).ToArray();
        }

        internal void Serialize(TextWriter writer)
        {
            var f = this.Data;
            writer.Write("Format: ");
            for (var i = 0; i < f.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                writer.Write(f[i]);
            }
        }

        internal readonly string[] Data;

        /// <summary>
        /// Returns whatever two <see cref="EntryHeader"/> are equal, ignore differences of the order of entry names.
        /// </summary>
        /// <param name="other">The <see cref="EntryHeader"/> to compare with this <see cref="EntryHeader"/>.</param>
        /// <returns>True if the two <see cref="EntryHeader"/> are equal.</returns>
        public bool Equals(EntryHeader other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (this.Data.Length != other.Data.Length)
                return false;
            return this.Data.Length == Enumerable.Join(this.Data, other.Data, s => s, s => s, (o, i) => 0, StringComparer.OrdinalIgnoreCase).Count();
        }

        /// <summary>
        /// Returns whatever two <see cref="EntryHeader"/> are equal, ignore differences of the order of entry names.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="EntryHeader"/>.</param>
        /// <returns>True if the two <see cref="EntryHeader"/> are equal.</returns>
        public override bool Equals(object obj)
            => obj is EntryHeader other && Equals(other);

        /// <summary>
        /// Get hash code of this <see cref="EntryHeader"/>, ignore differences of the order of entry names.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => this.Data.Aggregate(0, (o, n) => o ^ StringComparer.OrdinalIgnoreCase.GetHashCode(n));

        /// <summary>
        /// Get the entry name with the given index.
        /// </summary>
        /// <param name="index">The index of entry names, starts from 0.</param>
        /// <returns>The entry name with the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> out of range.</exception>
        public string this[int index] => this.Data[index];

        /// <summary>
        /// Get the number of entry names of this <see cref="EntryHeader"/>.
        /// </summary>
        public int Count => this.Data.Length;

        /// <summary>
        /// Get the enumerator of entry names of this <see cref="EntryHeader"/>.
        /// </summary>
        /// <returns>The enumerator of entry names of this <see cref="EntryHeader"/>.</returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)this.Data).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.Data.GetEnumerator();
    }
}
