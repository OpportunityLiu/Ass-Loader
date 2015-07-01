using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssLoader;
using System.Linq;

namespace Test
{
    [TestClass]
    public class InitTest
    {
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

        private TestHelper helper;

        [TestMethod]
        public void InitA()
        {
            helper.StartTimer();
            TestHelper.Init();
            helper.EndTimer();
        }

        [TestMethod]
        public void InitB()
        {
            helper.StartTimer();
            TestHelper.Init();
            helper.EndTimer();
        }
    }
}
