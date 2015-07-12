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
                    PropertyChanging();
                }
                else
                    throw new ArgumentNullException("value");
            }
        }

        [EntryField("Fontname")]
        private string fontName;

        /// <summary>
        /// The font name as used by Windows. 
        /// </summary>
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
                    PropertyChanging();
                }
                else
                    throw new ArgumentNullException("value");
            }
        }

        [EntryField("Fontsize")]
        private double fontSize = 12;

        /// <summary>
        /// The font size in points.
        /// </summary>
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                fontSize = value;
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
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
                PropertyChanging();
            }
        }

        [EntryField("ScaleX")]
        private double scaleX = 100;

        /// <summary>
        /// Text stretching in the horizontal direction in percent.
        /// </summary>
        public double ScaleX
        {
            get
            {
                return scaleX;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                scaleX = value;
                PropertyChanging();
            }
        }

        [EntryField("ScaleY")]
        private double scaleY = 100;

        /// <summary>
        /// Text stretching in the vertical direction in percent.
        /// </summary>
        public double ScaleY
        {
            get
            {
                return scaleY;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                scaleY = value;
                PropertyChanging();
            }
        }

        [EntryField("Spacing")]
        private double spacing;

        /// <summary>
        /// Extra space between characters in pixels.
        /// </summary>
        public double Spacing
        {
            get
            {
                return spacing;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                spacing = value;
                PropertyChanging();
            }
        }

        [EntryField("Angle")]
        private double rotation;

        /// <summary>
        /// The angle of the rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value % 360;
                if(rotation > 180)
                    rotation -= 360;
                else if(rotation < -180)
                    rotation += 360;
                PropertyChanging();
            }
        }

        [EntryField("BorderStyle")]
        private int borderStyle = (int)BorderStyle.OutlineAndDropShadow;

        /// <summary>
        /// The style of border.
        /// </summary>
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
                    PropertyChanging();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        [EntryField("Outline")]
        private double outline;

        /// <summary>
        /// If <see cref="BorderStyle"/> is <see cref="BorderStyle.OutlineAndDropShadow"/>, this specifies the width of the outline around the text in pixels.
        /// </summary>
        public double Outline
        {
            get
            {
                return outline;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                outline = value;
                PropertyChanging();
            }
        }

        [EntryField("Shadow")]
        private double shadow;

        /// <summary>
        /// The depth of the drop shadow behind the text, in pixels.
        /// </summary>
        public double Shadow
        {
            get
            {
                return shadow;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                shadow = value;
                PropertyChanging();
            }
        }

        [EntryField("Alignment")]
        private int alignment = (int)AlignmentStyle.BottomCenter;

        /// <summary>
        /// The alignment of the text.
        /// </summary>
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
                    PropertyChanging();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        [EntryField("MarginL")]
        private int marginL;

        /// <summary>
        /// Left margin of the text.
        /// </summary>
        public int MarginL
        {
            get
            {
                return marginL;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                marginL = value;
                PropertyChanging();
            }
        }

        [EntryField("MarginR")]
        private int marginR;

        /// <summary>
        /// Right margin of the text.
        /// </summary>
        public int MarginR
        {
            get
            {
                return marginR;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                marginR = value;
                PropertyChanging();
            }
        }

        [EntryField("MarginV")]
        private int marginV;

        /// <summary>
        /// Vetical margin of the text.
        /// </summary>
        public int MarginV
        {
            get
            {
                return marginV;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                marginV = value;
                PropertyChanging();
            }
        }

        [EntryField("Encoding")]
        private int encoding = 1;

        /// <summary>
        /// Controls which codepage is used to map codepoints to glyphs.
        /// </summary>
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
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                encoding = value;
                PropertyChanging();
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
