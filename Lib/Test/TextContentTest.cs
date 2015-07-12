using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AssLoader;

namespace Test
{
    [TestClass]
    public class TextContentTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            TextContent t =new TextContent( "df{ghj");
            var tags = t.Tags.ToArray();
            var texts = t.Texts.ToArray();
        }
    }
}
