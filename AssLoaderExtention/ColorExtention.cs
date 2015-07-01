using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIColor = Windows.UI.Color;
using System.ComponentModel;

namespace AssLoader
{
    public static class ColorExtention
    {
        public static UIColor ToUIColor(this Color value)
        {
            return UIColor.FromArgb((byte)(~value.Transparency), value.Red, value.Green, value.Blue);
        }

        public static Color ToAssColour(this UIColor value)
        {
            return Color.FromArgb(value.A, value.R, value.G, value.B);
        }
    }
}
