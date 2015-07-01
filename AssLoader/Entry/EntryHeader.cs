using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    public sealed class EntryHeader : IEquatable<EntryHeader>, IEqualityComparer<EntryHeader>, IReadOnlyList<string>
    {
        public EntryHeader(string format)
        {
            ThrowHelper.ThrowIfNullOrEmpty(format, "format");
            data = new EntryData(format, int.MaxValue);
            if(data.Contains(string.Empty) || data.Distinct().Count() != data.Count)
                throw new InvalidOperationException("header can't contains string.Empty or repeated strings.");
        }

        public EntryHeader(IEnumerable<string> format)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            data = format.Distinct(StringComparer.OrdinalIgnoreCase).Select(s => s.Trim()).ToList();
        }

        public override string ToString()
        {
            return string.Format(FormatHelper.DefaultFormat, "Format: {0}", data.ToString());
        }

        private IReadOnlyList<string> data;

        #region IEquatable<FormatEntry> 成员

        public bool Equals(EntryHeader other)
        {
            return Equals(this, other);
        }

        #endregion

        #region IEqualityComparer<FormatEntry> 成员

        public bool Equals(EntryHeader x, EntryHeader y)
        {
            if(object.ReferenceEquals(x, y))
                return true;
            if(x == null || y == null)
                return false;
            if(x.data.Count != y.data.Count)
                return false;
            return x.data.Count == Enumerable.Join(x.data, y.data, s => s, s => s, (o, i) => 0, StringComparer.OrdinalIgnoreCase).Count();
        }

        public int GetHashCode(EntryHeader obj)
        {
            ThrowHelper.ThrowIfNull(obj, "obj");
            return obj.data.Aggregate(0, (o, n) => o ^ StringComparer.OrdinalIgnoreCase.GetHashCode(n));
        }

        #endregion

        public override bool Equals(object obj)
        {
            var value = obj as EntryHeader;
            if(value == null)
                return false;
            return Equals(value, this);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IReadOnlyList<string> 成员

        public string this[int index]
        {
            get
            {
                return data[index];
            }
        }

        #endregion

        #region IReadOnlyCollection<string> 成员

        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        #endregion

        #region IEnumerable<string> 成员

        public IEnumerator<string> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }
}
