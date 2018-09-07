using System;
using UIColor = Windows.UI.Color;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Converters between <see cref="UIColor"/> and <see cref="Color"/>.
    /// </summary>
    public static class ColorExtention
    {
        /// <summary>
        /// Convert a <see cref="Color"/> to <see cref="UIColor"/>.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to convert.</param>
        /// <returns>A <see cref="UIColor"/> which presents the same value as <paramref name="value"/>.</returns>
        public static UIColor ToUIColor(this Color value)
        {
            return UIColor.FromArgb(value.Alpha, value.Red, value.Green, value.Blue);
        }

        /// <summary>
        /// Convert a <see cref="UIColor"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The <see cref="UIColor"/> to convert.</param>
        /// <returns>A <see cref="Color"/> which presents the same value as <paramref name="value"/>.</returns>
        public static Color ToAssColor(this UIColor value)
        {
            return Color.FromArgb(value.A, value.R, value.G, value.B);
        }
    }
}
