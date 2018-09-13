using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Presents a certain color with red, green, blue and transparency channel.
    /// </summary>
    public readonly struct Color : IEquatable<Color>
    {
        internal static Color FromUInt32(uint color) => new Color(color);
        private Color(uint data) => this.data = data;

        /// <summary>
        /// Modify the <see cref="Color"/> with given values of channels.
        /// </summary>
        /// <param name="transparency"><see cref="Transparency"/> of the <see cref="Color"/>, <see langword="null"/> for current value.</param>
        /// <param name="red"><see cref="Red"/> of the <see cref="Color"/>, <see langword="null"/> for current value.</param>
        /// <param name="green"><see cref="Green"/> of the <see cref="Color"/>, <see langword="null"/> for current value.</param>
        /// <param name="blue"><see cref="Blue"/> of the <see cref="Color"/>, <see langword="null"/> for current value.</param>
        /// <returns>The <see cref="Color"/> with given values of channels modified.</returns>
        public Color With(byte? red = default, byte? green = default, byte? blue = default, byte? transparency = default)
        {
            unchecked
            {
                var data = this.data;
                if (red.HasValue)
                {
                    data &= rFliter;
                    data |= ((uint)red << rOffset);
                }
                if (green.HasValue)
                {
                    data &= gFliter;
                    data |= ((uint)green << gOffset);
                }
                if (blue.HasValue)
                {
                    data &= bFliter;
                    data |= ((uint)blue << bOffset);
                }
                if (transparency.HasValue)
                {
                    data &= tFliter;
                    data |= ((uint)transparency << tOffset);
                }
                return new Color(data);
            }
        }

        /// <summary>
        /// Red channel of the <see cref="Color"/>.
        /// </summary>
        public byte Red => unchecked((byte)((this.data & rMask) >> rOffset));

        /// <summary>
        /// Green channel of the <see cref="Color"/>.
        /// </summary>
        public byte Green => unchecked((byte)((this.data & gMask) >> gOffset));

        /// <summary>
        /// Blue channel of the <see cref="Color"/>.
        /// </summary>
        public byte Blue => unchecked((byte)((this.data & bMask) >> bOffset));

        /// <summary>
        /// Transparency channel of the <see cref="Color"/>, 255 for fully-transparent and 0 for non-transparent.
        /// </summary>
        public byte Transparency => unchecked((byte)((this.data & tMask) >> tOffset));

        /// <summary>
        /// Alpha channel of the <see cref="Color"/>, 0 for fully-transparent and 255 for non-transparent.
        /// </summary>
        /// <seealso cref="Transparency"/>
        public byte Alpha => (byte)~this.Transparency;

        private const uint tFliter = 0x00FFFFFF;
        private const uint bFliter = 0xFF00FFFF;
        private const uint gFliter = 0xFFFF00FF;
        private const uint rFliter = 0xFFFFFF00;

        private const uint tMask = 0xFF000000;
        private const uint bMask = 0x00FF0000;
        private const uint gMask = 0x0000FF00;
        private const uint rMask = 0x000000FF;

        private const int tOffset = 24;
        private const int bOffset = 16;
        private const int gOffset = 8;
        private const int rOffset = 0;

        private readonly uint data;

        /// <summary>
        /// Returns the string form of the <see cref="Color"/> in the ass file.
        /// </summary>
        /// <returns>The string form of the <see cref="Color"/> in the ass file, which is a <see cref="string"/> started with "&amp;H" and followed an 8-digit hex number.</returns>
        public override string ToString()
        {
            return string.Format(FormatHelper.DefaultFormat, "&H{0:X8}", this.data);
        }

        /// <summary>
        /// Returns the <see cref="Color"/> of the string form.
        /// </summary>
        /// <param name="value">A <see cref="string"/> presents a <see cref="Color"/>.</param>
        /// <returns>The <see cref="Color"/> of the string form.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or <see cref="string.Empty"/>.</exception>
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid color string.</exception>
        public static Color Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            return Parse(value.AsSpan());
        }

        /// <summary>
        /// Returns the <see cref="Color"/> of the string form.
        /// </summary>
        /// <param name="value">A <see cref="string"/> presents a <see cref="Color"/>.</param>
        /// <returns>The <see cref="Color"/> of the string form.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or <see cref="string.Empty"/>.</exception>
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid color string.</exception>
        public static Color Parse(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                throw new ArgumentNullException(nameof(value));
            try
            {
                var b = 0;
                var l = value.Length;
                if (value.StartsWith("&H".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    b += 2;
                    l -= 2;
                }
                if (value.EndsWith("&".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    l -= 1;
                }
                return FromUInt32(Convert.ToUInt32(value.Slice(b, l).ToString(), 16));
            }
            catch (Exception ex)
            {
                throw new FormatException($"\"{value.ToString()}\" is not a valid color string.", ex);
            }
        }

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>True if the two <see cref="Color"/> are equal.</returns>
        public bool Equals(Color other) => this == other;

        /// <summary>
        /// Returns the hash code of this <see cref="Color"/>.
        /// </summary>
        /// <returns>The hash code of this <see cref="Color"/>.</returns>
        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Color"/> to compare.</param>
        /// <param name="right">The second <see cref="Color"/> to compare.</param>
        /// <returns>True if the two <see cref="Color"/> are equal.</returns>
        public static bool operator ==(Color left, Color right) => left.data == right.data;

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Color"/> to compare.</param>
        /// <param name="right">The second <see cref="Color"/> to compare.</param>
        /// <returns>True if the two <see cref="Color"/> are not equal.</returns>
        public static bool operator !=(Color left, Color right) => left.data != right.data;

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>True if the two <see cref="Color"/> are equal.</returns>
        public override bool Equals(object obj) => obj is Color c && Equals(c);

        /// <summary>
        /// Get a <see cref="Color"/> with given values of channels.
        /// </summary>
        /// <param name="alpha"><see cref="Alpha"/> of the <see cref="Color"/>.</param>
        /// <param name="red"><see cref="Red"/> of the <see cref="Color"/>.</param>
        /// <param name="green"><see cref="Green"/> of the <see cref="Color"/>.</param>
        /// <param name="blue"><see cref="Blue"/> of the <see cref="Color"/>.</param>
        /// <returns>The <see cref="Color"/> with given values of channels.</returns>
        public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
        {
            unchecked
            {
                return new Color(((uint)(~alpha) << tOffset) | ((uint)red << rOffset) | ((uint)green << gOffset) | ((uint)blue << bOffset));
            }
        }
    }
}
