using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AssLoader.Subtitle;
using AssLoader.Collections;

namespace Test
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
            get
            {
                return helper.Context;
            }
            set
            {
                helper = new TestHelper(value);
            }
        }

        [TestMethod]
        public void Parse()
        {
            foreach(var item in helper.LoadTestFiles())
            {
                try
                {
                    var ignore = Parse<AssScriptInfo>(item.Value);
                }
                catch(Exception ex)
                {
                    throw new Exception(item.Key, ex);
                }
            }
        }

        [TestMethod]
        public void ParseExact()
        {
            foreach(var item in helper.LoadTestFiles())
            {
                try
                {
                    var ignore = ParseExact<AssScriptInfo>(item.Value);
                }
                catch(Exception ex) when (!ex.InnerException.Message.StartsWith("Unknown section"))
                {
                    throw new Exception(item.Key, ex);
                }
                catch(Exception) { }
            }
        }

        [TestMethod]
        public void ParseAsync()
        {
            foreach(var item in helper.LoadTestFiles())
            {
                var ignore = ParseAsync<AssScriptInfo>(item.Value).Result;
            }
        }

        [TestMethod]
        public void ParseExactAsync()
        {
            foreach(var item in helper.LoadTestFiles())
            {
                try
                {
                    var ignore = ParseExactAsync<AssScriptInfo>(item.Value).Result;
                }
                catch(AggregateException ex) when (!ex.InnerException.InnerException.Message.StartsWith("Unknown section"))
                {
                    throw new Exception(item.Key, ex.InnerException);
                }
                catch(AggregateException) { }
            }
        }
    }
}
