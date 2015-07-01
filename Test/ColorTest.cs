using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssLoader;

namespace Test
{
    [TestClass]
    public class ColorTest
    {
        [TestMethod]
        public void ParseRgba()
        {
            for(int i = 0; i < 0xffff; i++)
            {
                uint va = TestHelper.RandomReader.ReadUInt32();
                var str = "&H" + Convert.ToString(va, 16).PadLeft(8, '0');
                var c = Color.Parse(str);
                Assert.AreEqual(c.ToString(), str, true);
            }
        }
            
        [TestMethod]
        public void ParseRgb()
        {
            for(int i = 0; i < 0xffff; i++)
            {
                var va = TestHelper.Random.Next(0x1000000);
                var c = Color.Parse("&H" + Convert.ToString(va, 16).PadLeft(6, '0'));
                Assert.AreEqual(c.ToString(), "&H" + Convert.ToString(va, 16).PadLeft(8, '0'), true);
            }
        }
            
        [TestMethod]
        public void ChangeRgba()
        {
            Func<Color> raCo = () => Color.Parse("&H" + Convert.ToString(TestHelper.RandomReader.ReadUInt32(), 16).PadLeft(8, '0'));
            for(int i = 0; i < 256; i++)
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
            for(int i = 0; i < 256; i++)
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
            for(int i = 0; i < 256; i++)
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
            for(int i = 0; i < 256; i++)
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
            for(int i = 0; i < 256; i++)
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
            for(int i = 0; i < 0xFFFFF; i++)
            {
                var b = TestHelper.RandomReader.ReadBytes(4);
                var c = Color.FromArgb(b[0], b[1], b[2], b[3]);
                Assert.AreEqual(c.Red, b[1]);
                Assert.AreEqual(c.Green, b[2]);
                Assert.AreEqual(c.Blue, b[3]);
                Assert.AreEqual(c.Transparency + b[0], 255);
            }
        }
    }
}
