using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using System;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class StyleTest : TestBase
    {
        [TestMethod]
        public void StyleSet()
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
            Assert.AreSame(c[0], f1);
            Assert.AreSame(c[1], f2);
            Assert.AreSame(c[5], f6);
            Assert.AreSame(c[7], f8);
            Assert.AreEqual(8, c.Count);

            c[0] = f1.Clone("1");
            Assert.AreNotSame(c[0], f1);
            Assert.AreEqual(8, c.Count);

            c[0] = f1.Clone("2");
            Assert.AreNotSame(c[0], f1);
            Assert.AreSame(c[1], f3);
            Assert.AreEqual(7, c.Count);

            c[0] = f1.Clone("111");
            Assert.AreNotSame(c[0], f1);
            Assert.AreSame(c[1], f3);
            Assert.AreEqual(7, c.Count);

            c.Insert(1, f2);
            Assert.AreSame(c[1], f2);
            Assert.AreEqual(8, c.Count);

            c.Insert(4, f2);
            Assert.AreSame(c[1], f2);
            Assert.AreEqual(8, c.Count);
        }

        [TestMethod]
        public void Clone()
        {
            var sub = Subtitle.Parse<AssScriptInfo>(this.TestHelper.TestFile).Result;
            foreach (var style in sub.StyleSet)
            {
                var clone = style.Clone("Default");
                Assert.AreEqual("Default", clone.Name);
                Assert.AreEqual(style.Outline, clone.Outline);
                Assert.AreEqual(style.OutlineColor, clone.OutlineColor);
                Assert.AreEqual(style.FontName, clone.FontName);
                Assert.AreEqual(style.FontSize, clone.FontSize);
                Assert.AreEqual(style.Italic, clone.Italic);
            }
        }
    }
}
