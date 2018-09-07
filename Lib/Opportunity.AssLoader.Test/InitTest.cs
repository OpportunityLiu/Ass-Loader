using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using System.Linq;

namespace Test
{
    [TestClass]
    public class InitTest
    {
        public TestContext TestContext
        {
            get => helper.Context;
            set => helper = new TestHelper(value);
        }

        private TestHelper helper;

        [TestMethod]
        public void InitA()
        {
            this.helper.StartTimer();
            TestHelper.Init();
            this.helper.EndTimer();
        }

        [TestMethod]
        public void InitB()
        {
            this.helper.StartTimer();
            TestHelper.Init();
            this.helper.EndTimer();
        }
    }
}
