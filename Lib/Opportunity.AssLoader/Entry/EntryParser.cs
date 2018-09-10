using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    internal static class EntryParser
    {
        public static string[] ParseHeader(ReadOnlySpan<char> format)
        {
            if (format.IsEmpty)
                throw new ArgumentNullException(nameof(format));
            var data = Parse(format, int.MaxValue);
            if (data.Contains(string.Empty))
                throw new ArgumentException("Header can't contains string.Empty.", nameof(format));
            if (data.Distinct().Count() != data.Length)
                throw new FormatException("Header can't contains repeated strings.");
            return data;
        }

        internal static void Serialize(string[] format, TextWriter writer)
        {
            writer.Write("Format: ");
            for (var i = 0; i < format.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                writer.Write(format[i]);
            }
        }

        private static readonly char[] splitChar = new char[] { ',' };

        internal static string[] Parse(ReadOnlySpan<char> fields, int count)
        {
            var sfields = fields.ToString().Split(splitChar, count);
            for (var i = 0; i < sfields.Length; i++)
                sfields[i] = sfields[i].Trim();
            return sfields;
        }
    }
}
