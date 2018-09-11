using Opportunity.AssLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    internal class Program
    {
        private enum a
        {
            a
        }
        private static int Foo<TEnum>(TEnum value)
            where TEnum : struct, IConvertible  // C# does not allow enum constraint
        {
            return value.ToInt32(null);
        }
        private static void Main(string[] args)
        {
            var f = File.ReadAllText(@"..\..\..\TestFiles\Upotte[02].ass");
            for (var i = 0; i < 0xffff; i++)
            {
                var t = Subtitle.Parse<AssScriptInfo>(f);
                t.Result.Serialize();
            }
        }
    }
}
