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
        private static void Main(string[] args)
        {
            var f = File.ReadAllText(@"..\..\..\TestFiles\Upotte[02].ass");
            for (var i = 0; i < 0xffff; i++)
            {
                var t = Subtitle.ParseExact<AssScriptInfo>(f);
            }
        }
    }
}
