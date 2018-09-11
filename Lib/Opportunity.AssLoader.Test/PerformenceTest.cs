using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class PerformenceTest : TestBase
    {
        [TestMethod]
        public void Read()
        {
            this.TestHelper.StartTimer();
            for (var i = 0; i < 0xff; i++)
                Subtitle.Parse<AssScriptInfo>(this.TestHelper.TestFile);
            this.TestHelper.EndTimer();
        }

        [TestMethod]
        public void Write()
        {
            var sub = Subtitle.Parse<AssScriptInfo>(this.TestHelper.TestFile).Result;
            this.TestHelper.StartTimer();
            for (var i = 0; i < 0xff; i++)
                sub.Serialize();
            this.TestHelper.EndTimer();
        }
    }
}
