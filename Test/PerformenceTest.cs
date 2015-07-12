using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssLoader;
using System.IO;
using AssLoader.Collections;

namespace Test
{
    [TestClass]
    public class PerformenceTest
    {
        public PerformenceTest()
        {
            TestHelper.Init();
        }

        private TestHelper helper;

        private Subtitle<AssScriptInfo> sub;

        public TestContext TestContext
        {
            get
            {
                return helper.Context;
            }
            set
            {
                helper = new TestHelper(value);
                sub = Subtitle.Parse<AssScriptInfo>(helper.TestFile);
            }
        }


        [TestMethod]
        public void Read()
        {
            helper.StartTimer();
            for(int i = 0; i < 0xff; i++)
                Subtitle.Parse<AssScriptInfo>(helper.TestFile);
            helper.EndTimer();
        }

        [TestMethod]
        public void Write()
        {
            helper.StartTimer();
            for(int i = 0; i < 0xff; i++)
                sub.Serialize();
            helper.EndTimer();
        }
    }
}
