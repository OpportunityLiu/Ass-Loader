using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;

namespace Test
{
    [TestClass]
    public class ColorTest
    {
        [TestMethod]
        public void ParseRgba()
        {
            for(var i = 0; i < 0xffff; i++)
            {
                var va = TestHelper.RandomReader.ReadUInt32();
                var str = "&H" + Convert.ToString(va, 16).PadLeft(8, '0');
                var c = Color.Parse(str);
                Assert.AreEqual(str, c.ToString(), true);
            }
        }

        [TestMethod]
        public void ParseRgb()
        {
            for(var i = 0; i < 0xffff; i++)
            {
                var va = TestHelper.Random.Next(0x1000000);
                var c = Color.Parse("&H" + Convert.ToString(va, 16).PadLeft(6, '0'));
                Assert.AreEqual("&H" + Convert.ToString(va, 16).PadLeft(8, '0'), c.ToString(), true);
            }
        }

        [TestMethod]
        public void ChangeRgba()
        {
            Func<Color> raCo = () => Color.Parse("&H" + Convert.ToString(TestHelper.RandomReader.ReadUInt32(), 16).PadLeft(8, '0'));
            for(var i = 0; i < 256; i++)
            {
                var va = (byte)i;
                var c = raCo();
                byte r = c.Red, g = c.Green, b = c.Blue, t = c.Transparency;
                c.Red = va;
                Assert.AreEqual(va, c.Red);
                Assert.AreEqual(g, c.Green);
                Assert.AreEqual(b, c.Blue);
                Assert.AreEqual(t, c.Transparency);
                Assert.AreEqual(255, c.Transparency + c.Alpha);
            }
            for(var i = 0; i < 256; i++)
            {
                var va = (byte)i;
                var c = raCo();
                byte r = c.Red, g = c.Green, b = c.Blue, t = c.Transparency;
                c.Green = va;
                Assert.AreEqual(r, c.Red);
                Assert.AreEqual(va, c.Green);
                Assert.AreEqual(b, c.Blue);
                Assert.AreEqual(t, c.Transparency);
                Assert.AreEqual(255, c.Transparency + c.Alpha);
            }
            for(var i = 0; i < 256; i++)
            {
                var va = (byte)i;
                var c = raCo();
                byte r = c.Red, g = c.Green, b = c.Blue, t = c.Transparency;
                c.Blue = va;
                Assert.AreEqual(r, c.Red);
                Assert.AreEqual(g, c.Green);
                Assert.AreEqual(va, c.Blue);
                Assert.AreEqual(t, c.Transparency);
                Assert.AreEqual(255, c.Transparency + c.Alpha);
            }
            for(var i = 0; i < 256; i++)
            {
                var va = (byte)i;
                var c = raCo();
                byte r = c.Red, g = c.Green, b = c.Blue, t = c.Transparency;
                c.Transparency = va;
                Assert.AreEqual(r, c.Red);
                Assert.AreEqual(g, c.Green);
                Assert.AreEqual(b, c.Blue);
                Assert.AreEqual(va, c.Transparency);
                Assert.AreEqual(255, c.Transparency + c.Alpha);
            }
            for(var i = 0; i < 256; i++)
            {
                var va = (byte)i;
                var c = raCo();
                byte r = c.Red, g = c.Green, b = c.Blue, t = c.Transparency;
                c.Alpha = va;
                Assert.AreEqual(r, c.Red);
                Assert.AreEqual(g, c.Green);
                Assert.AreEqual(b, c.Blue);
                Assert.AreEqual(va, c.Alpha);
                Assert.AreEqual(255, c.Transparency + c.Alpha);
            }
        }

        [TestMethod]
        public void FromArgb()
        {
            for(var i = 0; i < 0xFFFFF; i++)
            {
                var b = TestHelper.RandomReader.ReadBytes(4);
                var c = Color.FromArgb(b[0], b[1], b[2], b[3]);
                Assert.AreEqual(b[1], c.Red);
                Assert.AreEqual(b[2], c.Green);
                Assert.AreEqual(b[3], c.Blue);
                Assert.AreEqual(255, c.Transparency + b[0]);
            }
        }
    }
}
