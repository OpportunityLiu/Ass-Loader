using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader.Collections;
using System;
using System.IO;
using System.Threading.Tasks;
using static Opportunity.AssLoader.Subtitle;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class SubtitleParseTest
    {
        public SubtitleParseTest()
        {
            TestHelper.Init();
        }

        private TestHelper helper;

        public TestContext TestContext
        {
            get => this.helper.Context;
            set => this.helper = new TestHelper(value);
        }

        [TestMethod]
        public void ParseEmpty()
        {
            var t = Subtitle.Parse<AssScriptInfo>("");
        }

        [TestMethod]
        public void Parse()
        {
            foreach (var item in this.helper.LoadTestFiles())
            {
                try
                {
                    using (item.Value)
                    {
                        var file = Parse<AssScriptInfo>(item.Value);
                        var str = file.Serialize();
                        var file2 = ParseExact<AssScriptInfo>(new StringReader(str));
                        var str2 = file2.Serialize();
                        Assert.AreEqual(str, str2);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(item.Key, ex);
                }
            }
        }

        [TestMethod]
        public async Task ParseAsync()
        {
            foreach (var item in this.helper.LoadTestFiles())
            {
                try
                {
                    using (item.Value)
                    {
                        var file = await ParseAsync<AssScriptInfo>(item.Value);
                        var str = file.Serialize();
                        var file2 = await ParseExactAsync<AssScriptInfo>(new StringReader(str));
                        var str2 = file2.Serialize();
                        Assert.AreEqual(str, str2);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(item.Key, ex);
                }
            }
        }
    }
}
