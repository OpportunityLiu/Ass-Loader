using System;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class CollectionTest
    {
        public CollectionTest()
        {
            TestHelper.Init();
        }

        private TestHelper helper;

        public TestContext TestContext
        {
            get => helper.Context;
            set => helper = new TestHelper(value);
        }

        [TestMethod]
        public void CreateStyleSet()
        {
            var c = new StyleSet();
            var f1 = new Style("1") { FontName = "a" };
            var f2 = new Style("2") { FontName = "ab" };
            var f3 = new Style("3") { FontName = "abb" };
            var f4 = new Style("4") { FontName = "abbb" };
            var f5 = new Style("5") { FontName = "abbbb" };
            var f6 = new Style("6") { FontName = "abbbbb" };
            var f7 = new Style("7") { FontName = "abbbbbb" };
            var f8 = new Style("8") { FontName = "abbbbbbb" };
            c.Add(f1);
            c.Add(f2);
            c.Add(f3);
            c.Add(f4);
            c.Add(f5);
            c.Add(f6);
            c.Add(f7);
            c.Add(f8);
        }
    }
}
