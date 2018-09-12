using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader
{
    internal static class EntryHeader
    {
        public static string[] Parse(string format) => Parse(format.AsSpan());

        public static string[] Parse(ReadOnlySpan<char> format)
        {
            if (format.IsEmpty)
                throw new ArgumentNullException(nameof(format));
            var list = new List<string>();
            foreach (var item in format.Split(','))
            {
                var str = item.Trim();
                if (str.IsEmpty)
                    throw new ArgumentException("Header can't contains string.Empty.", nameof(format));
                list.Add(str.ToString());
            }
            if (list.Distinct().Count() != list.Count)
                throw new FormatException("Header can't contains repeated strings.");
            return list.ToArray();
        }

        public static void Serialize(string[] format, TextWriter writer)
        {
            writer.Write("Format: ");
            for (var i = 0; i < format.Length; i++)
            {
                if (i != 0)
                    writer.Write(',');
                writer.Write(format[i]);
            }
        }
    }
}
