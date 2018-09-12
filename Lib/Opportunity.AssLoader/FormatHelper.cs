using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    internal static class FormatHelper
    {
        private static readonly char[] retChar = new char[] { '\n', '\r' };

        /// <summary>
        /// Trim input string, returns <see langword="false"/> if it's <see langword="null"/> or whitespace, throw if contains crlf.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public static bool SingleLineStringValueValid(ref string value)
        {
            if (value is null)
                return false;
            value = value.Trim();
            if (value.Length == 0)
                return false;
            if (value.IndexOfAny(retChar) != -1)
                throw new ArgumentException("value must be single line.", nameof(value));
            return true;
        }

        /// <summary>
        /// Trim input string and replace ',' with ';', returns <see langword="false"/> if it's <see langword="null"/> or whitespace, throw if contains crlf.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public static bool FieldStringValueValid(ref string value)
        {
            if (SingleLineStringValueValid(ref value))
            {
                value = value.Replace(',', ';');
                return true;
            }
            else
                return false;
        }

        public static IFormatProvider DefaultFormat { get; } = System.Globalization.CultureInfo.InvariantCulture;

        private static readonly char[] split = new char[] { ':' };

        public static bool TryPraseLine(out ReadOnlySpan<char> key, out ReadOnlySpan<char> value, ReadOnlySpan<char> rawString)
        {
            var ind = rawString.IndexOf(':');
            if (ind == -1)
            {
                key = value = null;
                return false;
            }
            key = rawString.Slice(0, ind).TrimEnd();
            value = rawString.Slice(ind + 1).TrimStart(null);
            return true;
        }

        public readonly ref struct SplitEnumerable
        {
            private readonly ReadOnlySpan<char> data;
            private readonly char splitter;
            private readonly int maxCount;


            public SplitEnumerable(ReadOnlySpan<char> data, char splitter, int maxCount)
            {
                this.data = data;
                this.splitter = splitter;
                this.maxCount = maxCount;
            }

            public ref struct SplitEnumerator
            {
                private readonly SplitEnumerable data;

                private ReadOnlySpan<char> remain, current;

                private int state;

                public ReadOnlySpan<char> Current => this.state >= 0 ? this.current : throw new InvalidOperationException();

                public bool MoveNext()
                {
                    if (this.state == -1)
                        this.state = 1;
                    if (this.state == 0)
                    {
                        this.state = int.MinValue;
                        return false;
                    }

                    if (this.data.maxCount >= 0 && this.data.maxCount == this.state)
                    {
                        this.state = 0;
                        this.current = this.remain;
                        return true;
                    }

                    var brindex = this.remain.IndexOf(this.data.splitter);
                    if (brindex < 0)
                    {
                        this.state = 0;
                        this.current = this.remain;
                        return true;
                    }

                    this.current = this.remain.Slice(0, brindex);
                    this.remain = this.remain.Slice(brindex + 1);
                    this.state++;
                    return true;
                }

                public SplitEnumerator(in SplitEnumerable splitEnumerable)
                {
                    this.data = splitEnumerable;
                    this.state = -1;
                    this.remain = splitEnumerable.data;
                    this.current = default;
                }
            }

            public SplitEnumerator GetEnumerator() => new SplitEnumerator(in this);
        }

        public static SplitEnumerable Split(this ReadOnlySpan<char> data, char splitter)
            => new SplitEnumerable(data, splitter, -1);

        public static SplitEnumerable Split(this ReadOnlySpan<char> data, char splitter, int maxCount)
            => new SplitEnumerable(data, splitter, maxCount);
    }
}
