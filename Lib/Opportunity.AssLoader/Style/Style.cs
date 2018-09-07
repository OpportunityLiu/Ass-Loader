using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opportunity.AssLoader.Serializer;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Entry of "styles" section.
    /// </summary>
    public class Style : Entry
    {
        /// <summary>
        /// Create new instance of <see cref="Style"/>.
        /// </summary>
        protected Style()
        {
        }

        /// <summary>
        /// Create new instance of <see cref="Style"/> with the name.
        /// </summary>
        /// <param name="name">Name of the <see cref="Style"/>.</param>
        public Style(string name)
        {
            this.Name = name;
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
            var n = this.Clone(() => new Style());
            n.Name = newName;
            return n;
        }

        /// <summary>
        /// Return a string form of this <see cref="Style"/> with its <see cref="Name"/>.
        /// </summary>
        /// <returns>A string form of this <see cref="Style"/>.</returns>
        public override string ToString()
        {
            return "Style: " + this.name;
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
            get => this.name;
            protected set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                {
                    this.Set(ref this.name, value);
                }
                else
                    throw new ArgumentNullException(nameof(value));
            }
        }

        [EntryField("Fontname")]
        private string fontName = "Arial";

        /// <summary>
        /// The font name as used by Windows. 
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or white space.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string FontName
        {
            get => this.fontName;
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                {
                    this.Set(ref this.fontName, value);
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
            get => this.fontSize;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.fontSize, value);
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
            get => this.primaryColor;
            set => this.Set(ref this.primaryColor, value);
        }

        [ColorSerialize]
        [EntryField("SecondaryColour")]
        private Color secondaryColor;

        /// <summary>
        /// Secondary fill color, used for karaoke effects 
        /// </summary>
        public Color SecondaryColor
        {
            get => this.secondaryColor;
            set => this.Set(ref this.secondaryColor, value);
        }

        [ColorSerialize]
        [EntryField("OutlineColour")]
        private Color outlineColor;

        /// <summary>
        /// The border color of the text.
        /// </summary>
        public Color OutlineColor
        {
            get => this.outlineColor;
            set => this.Set(ref this.outlineColor, value);
        }

        [ColorSerialize]
        [EntryField("BackColour")]
        private Color shadowColor;

        /// <summary>
        /// The color of the shadow, which is displayed under the main text and offset by the shadow width defined to the right.
        /// </summary>
        public Color ShadowColor
        {
            get => this.shadowColor;
            set => this.Set(ref this.shadowColor, value);
        }

        [EntryField("Bold")]
        private int bold;

        /// <summary>
        /// Defines whether text is bold (true) or not (false). 
        /// </summary>
        public bool Bold
        {
            get => this.bold == -1;
            set => this.Set(ref this.bold, value ? -1 : 0);
        }

        [EntryField("Italic")]
        private int italic;

        /// <summary>
        /// Defines whether text is italic (true) or not (false). 
        /// </summary>
        public bool Italic
        {
            get => this.italic == -1;
            set => this.Set(ref this.italic, value ? -1 : 0);
        }

        [EntryField("Underline")]
        private int underline;

        /// <summary>
        /// Defines whether text has an underline (true) or not (false). 
        /// </summary>
        public bool Underline
        {
            get => this.underline == -1;
            set => this.Set(ref this.underline, value ? -1 : 0);
        }

        [EntryField("Strikeout")]
        private int strikeout;

        /// <summary>
        /// Defines whether text has a strikeout (true) or not (false). 
        /// </summary>
        public bool Strikeout
        {
            get => this.strikeout == -1;
            set => this.Set(ref this.strikeout, value ? -1 : 0);
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
            get => this.scaleX;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.scaleX, value);
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
            get => this.scaleY;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.scaleY, value);
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
            get => this.spacing;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                this.Set(ref this.spacing, value);
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
            get => this.rotation;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                value = value % 360;
                if(value > 180)
                    value -= 360;
                else if(value < -180)
                    value += 360;
                this.Set(ref this.rotation, value);
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
            get => (BorderStyle)this.borderStyle;
            set
            {
                switch(value)
                {
                case BorderStyle.OutlineAndDropShadow:
                case BorderStyle.OpaqueBox:
                    this.Set(ref this.borderStyle, (int)value);
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
            get => this.outline;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.outline, value);
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
            get => this.shadow;
            set
            {
                if(ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.shadow, value);
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
            get => (AlignmentStyle)this.alignment;
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
                    this.Set(ref this.alignment, (int)value);
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
            get => this.marginL;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.marginL, value);
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
            get => this.marginR;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.marginR, value);
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
            get => this.marginV;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.marginV, value);
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
            get => this.encoding;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.encoding, value);
            }
        }

        #endregion Fields

        /// <summary>
        /// Name of this <see cref="Entry"/>, will be "Style".
        /// </summary>
        protected sealed override string EntryName => "Style";
    }
}
