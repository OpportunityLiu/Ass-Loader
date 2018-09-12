using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader.Collections;
using Opportunity.AssLoader.Effects;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Opportunity.AssLoader.Subtitle;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class SubtitleParseTest : TestBase
    {
        [TestMethod]
        public void ParseEmpty()
        {
            var t = Parse<AssScriptInfo>("");
        }

        [FileTestMethod]
        public void Parse(string data)
        {
            var file = Parse<AssScriptInfo>(data);
            Assert.AreEqual(file.Result.ScriptInfo.UndefinedFields.Count + file.Result.Events.Count(e => e.Effect is UnknownEffect), file.Exceptions.Where(ex => ex.Message != "Unknown section [Aegisub Project Garbage] found.").Count());
            var str = file.Result.Serialize();
            var file2 = Parse<AssScriptInfo>(str);
            Assert.AreEqual(file2.Result.ScriptInfo.UndefinedFields.Count + file2.Result.Events.Count(e => e.Effect is UnknownEffect), file2.Exceptions.Count);
            var str2 = file2.Result.Serialize();
            Assert.AreEqual(str, str2);
        }
    }
}
