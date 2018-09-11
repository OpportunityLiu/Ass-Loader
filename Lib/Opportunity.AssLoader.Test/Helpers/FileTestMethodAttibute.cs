using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    public class FileTestMethodAttribute : TestMethodAttribute
    {
        public static readonly string[] TestFileNames = new DirectoryInfo("../../../../TestFiles/").GetFiles().Select(f => f.Name).ToArray();

        public static readonly string[] TestFiles = TestFileNames.Select(n => File.ReadAllText("../../../../TestFiles/" + n)).ToArray();

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var l = TestFiles.Length;
            if (Limit < l)
                l = Limit;
            var r = new TestResult[l];
            for (var i = 0; i < r.Length; i++)
            {
                r[i] = testMethod.Invoke(new object[] { TestFiles[i] });
                r[i].DisplayName = TestFileNames[i];
            }
            return r;
        }

        public int Limit { get; set; }
    }

    public class SubTestMethodAttribute : FileTestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var l = TestFiles.Length;
            if (Limit < l)
                l = Limit;
            var r = new TestResult[l];
            for (var i = 0; i < r.Length; i++)
            {
                var submd = typeof(Subtitle).GetMethod("Parse", 1, new[] { typeof(string) });
                submd = submd.MakeGenericMethod(ScriptInfoType);
                var sub = submd.Invoke(null, new[] { TestFiles[i] });
                r[i] = testMethod.Invoke(new object[] { sub });
                r[i].DisplayName = TestFileNames[i];
            }
            return r;
        }

        public Type ScriptInfoType { get; set; } = typeof(AssScriptInfo);
    }
}
