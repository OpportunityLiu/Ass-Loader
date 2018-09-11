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

        [TestMethod]
        public void Parse()
        {
            foreach (var item in this.TestHelper.LoadTestFiles())
            {
                try
                {
                    var file = Parse<AssScriptInfo>(item.Value).Result;
                    var str = file.Serialize();
                    var file2 = Parse<AssScriptInfo>(str);
                    Assert.AreEqual(file2.Result.ScriptInfo.UndefinedFields.Count + file2.Result.EventCollection.Count(e => e.Effect is UnknownEffect), file2.Exceptions.Count);
                    var str2 = file2.Result.Serialize();
                    Assert.AreEqual(str, str2);
                }
                catch (Exception ex)
                {
                    throw new Exception(item.Key, ex);
                }
            }
        }
    }
}
