using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Opportunity.AssLoader;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class TextContentTest
    {
        [TestMethod]
        public void CreateTextContent()
        {
            var t =new TextContent( "df{ghj");
            var tags = t.Tags.ToArray();
            var texts = t.Texts.ToArray();
            Assert.AreEqual(0, tags.Length);
            Assert.AreEqual(1, texts.Length);
        }
    }
}
