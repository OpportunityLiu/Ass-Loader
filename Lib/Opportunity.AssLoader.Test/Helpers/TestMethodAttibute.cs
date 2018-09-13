using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            if (Limit < l && Limit != 0)
                l = Limit;
            var r = new TestResult[l];
            var paramGen = new Func<string, object>[testMethod.ParameterTypes.Length];
            for (var i = 0; i < paramGen.Length; i++)
            {
                var pinfo = testMethod.ParameterTypes[i];
                if (pinfo.ParameterType == typeof(string))
                    paramGen[i] = s => s;
                else if (pinfo.ParameterType.GetGenericTypeDefinition() == typeof(ParseResult<>)
                    || pinfo.ParameterType.GetGenericTypeDefinition() == typeof(Subtitle<>))
                {
                    var siType = pinfo.ParameterType.GetGenericArguments()[0];
                    var submd = typeof(Subtitle).GetMethod("Parse", 1, new[] { typeof(string) });
                    submd = submd.MakeGenericMethod(siType);
                    var del = (Func<string, IParseResult>)submd.CreateDelegate(typeof(Func<string, IParseResult>));
                    if (pinfo.ParameterType.GetGenericTypeDefinition() == typeof(ParseResult<>))
                        paramGen[i] = del;
                    else
                        paramGen[i] = s => del(s).Result;
                }
            }

            for (var i = 0; i < r.Length; i++)
            {
                var data = TestFiles[i];
                var param = new object[testMethod.ParameterTypes.Length];
                for (var j = 0; j < param.Length; j++)
                {
                    param[j] = paramGen[j](data);
                }

                r[i] = testMethod.Invoke(param);
                r[i].DisplayName = TestFileNames[i];
            }
            return r;
        }

        public int Limit { get; set; }
    }
}
