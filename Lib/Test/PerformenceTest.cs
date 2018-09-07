using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using System.IO;
using Opportunity.AssLoader.Collections;

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
            get => helper.Context;
            set
            {
                this.helper = new TestHelper(value);
                this.sub = Subtitle.Parse<AssScriptInfo>(this.helper.TestFile);
            }
        }


        [TestMethod]
        public void Read()
        {
            this.helper.StartTimer();
            for(var i = 0; i < 0xff; i++)
                Subtitle.Parse<AssScriptInfo>(this.helper.TestFile);
            this.helper.EndTimer();
        }

        [TestMethod]
        public void Write()
        {
            this.helper.StartTimer();
            for(var i = 0; i < 0xff; i++)
                this.sub.Serialize();
            this.helper.EndTimer();
        }
    }
}
