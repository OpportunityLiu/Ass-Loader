using System;
using AssLoader;
using AssLoader.Collections;
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
        public void TestMethod1()
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
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var s = Subtitle.Parse<AssScriptInfo>(helper.TestFile);
        }
    }
}
