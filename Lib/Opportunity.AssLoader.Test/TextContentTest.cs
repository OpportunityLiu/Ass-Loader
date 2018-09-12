using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Text;
using System;
using System.Linq;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class TextContentTest : TestBase
    {
        [TestMethod]
        public void CreateTextContent()
        {
            var t = new TextContent("df{ghj");
            var tags = t.Tags.ToArray();
            var texts = t.Texts.ToArray();
            Assert.AreEqual(0, tags.Length);
            Assert.AreEqual(1, texts.Length);
        }
    }
}
