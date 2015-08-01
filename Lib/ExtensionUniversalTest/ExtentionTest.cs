using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Reflection;
using AssLoader;

namespace UWPTest
{
    [TestClass]
    public class ExtentionTest
    {
        [TestMethod]
        public void ColorExtention()
        {
            var ra = new Random();
            Func<Color> raCo = () => Color.Parse("&H" + Convert.ToString((uint)ra.Next(int.MinValue, int.MaxValue), 16).PadLeft(8, '0'));
            for(int i = 0; i < 65536; i++)
            {
                var ca = raCo();
                var cu = ca.ToUIColor();
                Assert.AreEqual(ca, cu.ToAssColor());
                Assert.AreEqual(cu, ca.ToUIColor());
            }
        }
    }
}
