using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerformanceAnalyzeHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //var f = System.IO.File.OpenText(@"C:\Users\Opportunity\Desktop\Upotte[02].ass");
            //var t = AssLoader.Subtitle.Parse<AssLoader.AssScriptInfo>(f);
            //f.Dispose();
            //foreach(var item in t.StyleDictionary)
            //{
            //    item.Underline = false;
            //}
            //var test = (from m in testc.GetType().GetMethods()
            //            where m.GetCustomAttribute(typeof(TestMethodAttribute)) != null
            //            select m.Invoke(testc, null)).ToArray();
            var c = new SubtitleParseTest() { TestContext = null };
            for(int i = 0; i < 0xff; i++)
            {
                c.Parse();
                c.ParseExact();
            }
        }

        static void Read(int times)
        {
            var count = 0;
            var testc = new PerformenceTest()
            {
                TestContext = null
            };
        Repeat:
            try
            {
                testc.Read();
            }
            catch(Exception)
            {
                if(++count < times)
                    goto Repeat;
            }
        }

        static void Write(int times)
        {
            var count = 0;
            var testc = new PerformenceTest()
            {
                TestContext = null
            };
        Repeat:
            try
            {
                testc.Write();
            }
            catch(Exception)
            {
                if(++count < times)
                    goto Repeat;
            }
        }
    }
}
