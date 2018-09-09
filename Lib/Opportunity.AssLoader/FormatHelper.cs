using System;
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

        public static bool TryPraseLine(out string key, out string value, string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.IndexOf(':') == -1)
            {
                key = value = null;
                return false;
            }
            var s = rawString.Split(split, 2);
            key = s[0].TrimEnd(null);
            value = s[1].TrimStart(null);
            return true;
        }
    }
}
