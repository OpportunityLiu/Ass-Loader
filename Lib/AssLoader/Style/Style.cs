using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader.Serializer;

namespace AssLoader
{
    /// <summary>
    /// Entry of "styles" section.
    /// </summary>
    public class Style : Entry
    {
        /// <summary>
        /// Create new instance of <see cref="Style"/>.
        /// </summary>
        public Style()
        {
        }

        /// <summary>
        /// Parse from <paramref name="fields"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <returns><see cref="Style"/> presents the <paramref name="fields"/>.</returns>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        public static Style Parse(EntryHeader format, string fields)
        {
            var re = new Style();
            re.Parse(fields, format);
            return re;
        }

        /// <summary>
        /// Parse exactly from <paramref name="fields"/>.
        /// </summary>
        /// <param name="fields">A <see cref="string"/> of fields that seperates with ','.</param>
        /// <param name="format">The <see cref="EntryHeader"/> presents its format.</param>
        /// <returns><see cref="Style"/> presents the <paramref name="fields"/>.</returns>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">Deserialize failed for some fields.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Fields of <see cref="Style"/> and fields of <paramref name="format"/> doesn't match
        /// </exception>
        public static Style ParseExact(EntryHeader format, string fields)
        {
            var re = new Style();
            re.ParseExact(fields, format);
            return re;
        }

        /// <summary>
        /// Make a copy with new <see cref="Name"/> of this <see cref="Style"/>.
        /// </summary>
        /// <param name="newName">New <see cref="Name"/> of <see cref="Style"/></param>
        /// <returns>A copy of this <see cref="Style"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newName"/> is not a valid <see cref="Name"/>.</exception>
        public Style Clone(string newName)
        {
            var n = Clone<Style>();
            n.Name = newName;
            return n;
        }

        /// <summary>
        /// Return a string form of this <see cref="Style"/> with its <see cref="Name"/>.
        /// </summary>
        /// <returns>A string form of this <see cref="Style"/>.</returns>
        public override string ToString()
        {
            return "Style: " + name;
        }

        #region Fields

        [EntryField("Name", DefaultValue = "Default")]
        private string name;

        /// <summary>
        /// The name of the Style. Case insensitive. ',' will be replaced by ';'.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or white space.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            protected set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                {
                    name = value;
                    RaisePropertyChanged();
                }
                else
                    throw new ArgumentNullException(nameof(value));
            }
        }

        [EntryField("Fontname")]
        private string fontName;

        /// <summary>
        /// The font name as used by Windows. 
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or white space.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                {
                    fontName = value;
                    RaisePropertyChanged();
                }
                else
                    throw new ArgumentNullException(nameof(value));
            }
        }

        [EntryField("Fontsize")]
        private double fontSize = 12;

        /// <summary>
        /// The font size in points.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                fontSize = value;
                RaisePropertyChanged();
            }
        }

        [ColorSerialize]
        [EntryField("PrimaryColour")]
        private Color primaryColor;

        /// <summary>
        /// The main fill color of the body of the text.
        /// </summary>
        public Color PrimaryColor
        {
            get
            {
                return primaryColor;
            }
            set
            {
                primaryColor = value;
                RaisePropertyChanged();
            }
        }

        [ColorSerialize]
        [EntryField("SecondaryColour")]
        private Color secondaryColor;

        /// <summary>
        /// Secondary fill color, used for karaoke effects 
        /// </summary>
        public Color SecondaryColor
        {
            get
            {
                return secondaryColor;
            }
            set
            {
                secondaryColor = value;
                RaisePropertyChanged();
            }
        }

        [ColorSerialize]
        [EntryField("OutlineColour")]
        private Color outlineColor;

        /// <summary>
        /// The border color of the text.
        /// </summary>
        public Color OutlineColor
        {
            get
            {
                return outlineColor;
            }
            set
            {
                outlineColor = value;
                RaisePropertyChanged();
            }
        }

        [ColorSerialize]
        [EntryField("BackColour")]
        private Color shadowColor;

        /// <summary>
        /// The color of the shadow, which is displayed under the main text and offset by the shadow width defined to the right.
        /// </summary>
        public Color ShadowColor
        {
            get
            {
                return shadowColor;
            }
            set
            {
                shadowColor = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Bold")]
        private int bold;

        /// <summary>
        /// Defines whether text is bold (true) or not (false). 
        /// </summary>
        public bool Bold
        {
            get
            {
                return bold == -1;
            }
            set
            {
                if(value)
                    bold = -1;
                else
                    bold = 0;
                RaisePropertyChanged();
            }
        }

        [EntryField("Italic")]
        private int italic;

        /// <summary>
        /// Defines whether text is italic (true) or not (false). 
        /// </summary>
        public bool Italic
        {
            get
            {
                return italic == -1;
            }
            set
            {
                if(value)
                    italic = -1;
                else
                    italic = 0;
                RaisePropertyChanged();
            }
        }

        [EntryField("Underline")]
        private int underline;

        /// <summary>
        /// Defines whether text has an underline (true) or not (false). 
        /// </summary>
        public bool Underline
        {
            get
            {
                return underline == -1;
            }
            set
            {
                if(value)
                    underline = -1;
                else
                    underline = 0;
                RaisePropertyChanged();
            }
        }

        [EntryField("Strikeout")]
        private int strikeout;

        /// <summary>
        /// Defines whether text has a strikeout (true) or not (false). 
        /// </summary>
        public bool Strikeout
        {
            get
            {
                return strikeout == -1;
            }
            set
            {
                if(value)
                    strikeout = -1;
                else
                    strikeout = 0;
                RaisePropertyChanged();
            }
        }

        [EntryField("ScaleX")]
        private double scaleX = 100;

        /// <summary>
        /// Text stretching in the horizontal direction in percent.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        public double ScaleX
        {
            get
            {
                return scaleX;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                scaleX = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("ScaleY")]
        private double scaleY = 100;

        /// <summary>
        /// Text stretching in the vertical direction in percent.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        public double ScaleY
        {
            get
            {
                return scaleY;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                scaleY = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Spacing")]
        private double spacing;

        /// <summary>
        /// Extra space between characters in pixels.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/></exception>
        public double Spacing
        {
            get
            {
                return spacing;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                spacing = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Angle")]
        private double rotation;

        /// <summary>
        /// The angle of the rotation in degrees.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/></exception>
        public double Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                rotation = value % 360;
                if(rotation > 180)
                    rotation -= 360;
                else if(rotation < -180)
                    rotation += 360;
                RaisePropertyChanged();
            }
        }

        [EntryField("BorderStyle")]
        private int borderStyle = (int)BorderStyle.OutlineAndDropShadow;

        /// <summary>
        /// The style of border.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not one of the valid value.
        /// </exception>
        public BorderStyle BorderStyle
        {
            get
            {
                return (BorderStyle)borderStyle;
            }
            set
            {
                switch(value)
                {
                case BorderStyle.OutlineAndDropShadow:
                case BorderStyle.OpaqueBox:
                    borderStyle = (int)value;
                    RaisePropertyChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        [EntryField("Outline")]
        private double outline;

        /// <summary>
        /// If <see cref="BorderStyle"/> is <see cref="BorderStyle.OutlineAndDropShadow"/>, this specifies the width of the outline around the text in pixels.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        public double Outline
        {
            get
            {
                return outline;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                outline = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Shadow")]
        private double shadow;

        /// <summary>
        /// The depth of the drop shadow behind the text, in pixels.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        public double Shadow
        {
            get
            {
                return shadow;
            }
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                shadow = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Alignment")]
        private int alignment = (int)AlignmentStyle.BottomCenter;

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not one of the valid value.
        /// </exception>
        public AlignmentStyle Alignment
        {
            get
            {
                return (AlignmentStyle)alignment;
            }
            set
            {
                switch(value)
                {
                case AlignmentStyle.BottomLeft:
                case AlignmentStyle.BottomCenter:
                case AlignmentStyle.BottomRight:
                case AlignmentStyle.MiddleLeft:
                case AlignmentStyle.MiddleCenter:
                case AlignmentStyle.MiddleRight:
                case AlignmentStyle.TopLeft:
                case AlignmentStyle.TopCenter:
                case AlignmentStyle.TopRight:
                    alignment = (int)value;
                    RaisePropertyChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        [EntryField("MarginL")]
        private int marginL;

        /// <summary>
        /// Left margin of the text.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int MarginL
        {
            get
            {
                return marginL;
            }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                marginL = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("MarginR")]
        private int marginR;

        /// <summary>
        /// Right margin of the text.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int MarginR
        {
            get
            {
                return marginR;
            }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                marginR = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("MarginV")]
        private int marginV;

        /// <summary>
        /// Vetical margin of the text.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int MarginV
        {
            get
            {
                return marginV;
            }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                marginV = value;
                RaisePropertyChanged();
            }
        }

        [EntryField("Encoding")]
        private int encoding = 1;

        /// <summary>
        /// Controls which codepage is used to map codepoints to glyphs.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        /// <remarks>
        /// It has nothing to do with the actual text encoding of the script.
        /// This is only meaningful on Windows using VSFilter, where it is used to get some old(particularly Japanese) fonts without proper Unicode mappings to render properly.
        /// On other systems and renderers, Freetype2 provides the proper mappings.
        /// If you didn't understand a word of the above, pretend this setting doesn't exist, as it is rarely important.
        /// </remarks>
        public int Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                encoding = value;
                RaisePropertyChanged();
            }
        }

        #endregion Fields

        /// <summary>
        /// Name of this <see cref="Entry"/>, will be "Style".
        /// </summary>
        protected sealed override string EntryName
        {
            get
            {
                return "Style";
            }
        }
    }
}
