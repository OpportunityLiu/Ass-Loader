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
    public struct Color : IEquatable<Color>
    {
        internal static Color FromUInt32(uint color)
        {
            return new Color
            {
                data = color
            };
        }

        /// <summary>
        /// Red channel of the <see cref="Color"/>.
        /// </summary>
        public byte Red
        {
            get
            {
                unchecked
                {
                    return (byte)((this.data & rMask) >> rOffset);
                }
            }
            set
            {
                unchecked
                {
                    this.data &= rFliter;
                    this.data |= ((uint)value << rOffset);
                }
            }
        }

        /// <summary>
        /// Green channel of the <see cref="Color"/>.
        /// </summary>
        public byte Green
        {
            get
            {
                unchecked
                {
                    return (byte)((this.data & gMask) >> gOffset);
                }
            }
            set
            {
                unchecked
                {
                    this.data &= gFliter;
                    this.data |= ((uint)value << gOffset);
                }
            }
        }

        /// <summary>
        /// Blue channel of the <see cref="Color"/>.
        /// </summary>
        public byte Blue
        {
            get
            {
                unchecked
                {
                    return (byte)((this.data & bMask) >> bOffset);
                }
            }
            set
            {
                unchecked
                {
                    this.data &= bFliter;
                    this.data |= ((uint)value << bOffset);
                }
            }
        }

        /// <summary>
        /// Transparency channel of the <see cref="Color"/>, 255 for fully-transparent and 0 for non-transparent.
        /// </summary>
        public byte Transparency
        {
            get
            {
                unchecked
                {
                    return (byte)((this.data & tMask) >> tOffset);
                }
            }
            set
            {
                unchecked
                {
                    this.data &= tFliter;
                    this.data |= ((uint)value << tOffset);
                }
            }
        }

        /// <summary>
        /// Alpha channel of the <see cref="Color"/>, 0 for fully-transparent and 255 for non-transparent.
        /// </summary>
        /// <seealso cref="Transparency"/>
        public byte Alpha
        {
            get => (byte)~this.Transparency;
            set => this.Transparency = (byte)~value;
        }

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

        private uint data;

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
            try
            {
                if (value.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
                    return FromUInt32(Convert.ToUInt32(value.Substring(2), 16));
            }
            catch (Exception ex)
            {
                throw new FormatException($"\"{value}\" is not a valid color string.", ex);
            }
            throw new FormatException($"\"{value}\" is not a valid color string.");
        }

        #region IEquatable<Color> 成员

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>True if the two <see cref="Color"/> are equal.</returns>
        public bool Equals(Color other)
        {
            return this == other;
        }

        #endregion

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
        public static bool operator ==(Color left, Color right)
        {
            return left.data == right.data;
        }

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Color"/> to compare.</param>
        /// <param name="right">The second <see cref="Color"/> to compare.</param>
        /// <returns>True if the two <see cref="Color"/> are not equal.</returns>
        public static bool operator !=(Color left, Color right)
        {
            return left.data != right.data;
        }

        /// <summary>
        /// Returns whatever two <see cref="Color"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this <see cref="Color"/>.</param>
        /// <returns>True if the two <see cref="Color"/> are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Color c)
                return Equals(c);
            return false;
        }

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
                return new Color()
                {
                    data = (((uint)(~alpha) << tOffset) | ((uint)red << rOffset) | ((uint)green << gOffset) | ((uint)blue << bOffset))
                };
            }
        }
    }
}
