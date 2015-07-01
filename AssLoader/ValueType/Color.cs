using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    public struct Color : IEquatable<Color>, IEqualityComparer<Color>
    {
        internal static Color FromUInt32(uint color)
        {
            return new Color
            {
                data = color
            };
        }

        public byte Red
        {
            get
            {
                unchecked
                {
                    return (byte)((data & rMask) >> rOffset);
                }
            }
            set
            {
                unchecked
                {
                    data &= rFliter;
                    data |= ((uint)value << rOffset);
                }
            }
        }

        public byte Green
        {
            get
            {
                unchecked
                {
                    return (byte)((data & gMask) >> gOffset);
                }
            }
            set
            {
                unchecked
                {
                    data &= gFliter;
                    data |= ((uint)value << gOffset);
                }
            }
        }

        public byte Blue
        {
            get
            {
                unchecked
                {
                    return (byte)((data & bMask) >> bOffset);
                }
            }
            set
            {
                unchecked
                {
                    data &= bFliter;
                    data |= ((uint)value << bOffset);
                }
            }
        }

        public byte Transparency
        {
            get
            {
                unchecked
                {
                    return (byte)((data & tMask) >> tOffset);
                }
            }
            set
            {
                unchecked
                {
                    data &= tFliter;
                    data |= ((uint)value << tOffset);
                }
            }
        }

        public byte Alpha
        {
            get
            {
                return (byte)~Transparency;
            }
            set
            {
                Transparency = (byte)~value;
            }
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

        public override string ToString()
        {
            return string.Format(FormatHelper.DefaultFormat, "&H{0:X8}", data);
        }

        public static Color Parse(string value)
        {
            ThrowHelper.ThrowIfNullOrEmpty(value, "value");
            try
            {
                if(value.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
                    return Color.FromUInt32(Convert.ToUInt32(value.Substring(2), 16));
            }
            catch(Exception ex)
            {
                throw new FormatException(value + " is not a valid color string.", ex);
            }
            throw new FormatException(value + " is not a valid color string.");
        }

        #region IEquatable<Colour> 成员

        public bool Equals(Color other)
        {
            return this == other;
        }

        #endregion

        #region IEqualityComparer<Colour> 成员

        public bool Equals(Color x, Color y)
        {
            return x == y;
        }

        public int GetHashCode(Color obj)
        {
            return data.GetHashCode();
        }

        #endregion

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public static bool operator ==(Color first, Color second)
        {
            return first.data == second.data;
        }

        public static bool operator !=(Color first, Color second)
        {
            return first.data != second.data;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is Color))
                return false;
            return (Color)obj == this;
        }

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
