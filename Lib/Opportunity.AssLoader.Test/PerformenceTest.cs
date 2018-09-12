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
        [FileTestMethod(Limit = 1)]
        public void Read(string file)
        {
            this.TestHelper.StartTimer();
            for (var i = 0; i < 0xff; i++)
                Subtitle.Parse<AssScriptInfo>(file);
            this.TestHelper.EndTimer();
        }

        [FileTestMethod(Limit = 1)]
        public void Write(Subtitle<AssScriptInfo> sub)
        {
            this.TestHelper.StartTimer();
            for (var i = 0; i < 0xff; i++)
                sub.Serialize();
            this.TestHelper.EndTimer();
        }
    }
}
