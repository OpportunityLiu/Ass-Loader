using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Entry of "styles" section.
    /// </summary>
    [DebuggerDisplay(@"Style: {name}")]
    public sealed class Style : Entry
    {
        internal static readonly Dictionary<string, FieldSerializeHelper> FieldInfo = FieldSerializeHelper.GetScriptInfoFields(typeof(Style));

        internal Style() { }

        /// <summary>
        /// Create new instance of <see cref="Style"/> with the name.
        /// </summary>
        /// <param name="name">Name of the <see cref="Style"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or white space.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> contains line breaks.</exception>
        public Style(string name)
        {
            if (!FormatHelper.FieldStringValueValid(ref name))
                throw new ArgumentNullException(nameof(name));
            this.name = name;
        }

        /// <summary>
        /// Make a copy with new <see cref="Name"/> of this <see cref="Style"/>.
        /// </summary>
        /// <param name="newName">New <see cref="Name"/> of <see cref="Style"/></param>
        /// <returns>A copy of this <see cref="Style"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newName"/> is null or white space.</exception>
        /// <exception cref="ArgumentException"><paramref name="newName"/> contains line breaks.</exception>
        public Style Clone(string newName)
        {
            if (!FormatHelper.FieldStringValueValid(ref newName))
                throw new ArgumentNullException(nameof(newName));
            var n = (Style)MemberwiseClone();
            n.name = newName;
            return n;
        }

        #region Fields

        [EntryField("Name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string name;

        /// <summary>
        /// The name of the Style.
        /// </summary>
        public string Name => this.name;

        [EntryField("Fontname")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (!FormatHelper.FieldStringValueValid(ref value))
                    throw new ArgumentNullException(nameof(value));
                this.fontName = value;
            }
        }

        [EntryField("Fontsize")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.fontSize = value;
            }
        }

        [ColorSerialize]
        [EntryField("PrimaryColour")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color primaryColor;

        /// <summary>
        /// The main fill color of the body of the text.
        /// </summary>
        public Color PrimaryColor
        {
            get => this.primaryColor;
            set => this.primaryColor = value;
        }

        [ColorSerialize]
        [EntryField("SecondaryColour")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color secondaryColor;

        /// <summary>
        /// Secondary fill color, used for karaoke effects 
        /// </summary>
        public Color SecondaryColor
        {
            get => this.secondaryColor;
            set => this.secondaryColor = value;
        }

        [ColorSerialize]
        [EntryField("OutlineColour")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color outlineColor;

        /// <summary>
        /// The border color of the text.
        /// </summary>
        public Color OutlineColor
        {
            get => this.outlineColor;
            set => this.outlineColor = value;
        }

        [ColorSerialize]
        [EntryField("BackColour")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color shadowColor;

        /// <summary>
        /// The color of the shadow, which is displayed under the main text and offset by the shadow width defined to the right.
        /// </summary>
        public Color ShadowColor
        {
            get => this.shadowColor;
            set => this.shadowColor = value;
        }

        [EntryField("Bold")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int bold;

        /// <summary>
        /// Defines whether text is bold (true) or not (false). 
        /// </summary>
        public bool Bold
        {
            get => this.bold == -1;
            set => this.bold = value ? -1 : 0;
        }

        [EntryField("Italic")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int italic;

        /// <summary>
        /// Defines whether text is italic (true) or not (false). 
        /// </summary>
        public bool Italic
        {
            get => this.italic == -1;
            set => this.italic = value ? -1 : 0;
        }

        [EntryField("Underline")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int underline;

        /// <summary>
        /// Defines whether text has an underline (true) or not (false). 
        /// </summary>
        public bool Underline
        {
            get => this.underline == -1;
            set => this.underline = value ? -1 : 0;
        }

        [EntryField("StrikeOut")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int strikeOut;

        /// <summary>
        /// Defines whether text has a strikeout (true) or not (false). 
        /// </summary>
        public bool StrikeOut
        {
            get => this.strikeOut == -1;
            set => this.strikeOut = value ? -1 : 0;
        }

        [EntryField("ScaleX")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.scaleX = value;
            }
        }

        [EntryField("ScaleY")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.scaleY = value;
            }
        }

        [EntryField("Spacing")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                this.spacing = value;
            }
        }

        [EntryField("Angle")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                value = value % 360;
                if (value > 180)
                    value -= 360;
                else if (value < -180)
                    value += 360;
                this.rotation = value;
            }
        }

        [EntryField("BorderStyle")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (!value.IsDefined())
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.borderStyle = (int)value;
            }
        }

        [EntryField("Outline")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.outline = value;
            }
        }

        [EntryField("Shadow")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.shadow = value;
            }
        }

        [EntryField("Alignment")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (!value.IsDefined())
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.alignment = (int)value;
            }
        }

        [EntryField("MarginL")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.marginL = value;
            }
        }

        [EntryField("MarginR")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.marginR = value;
            }
        }

        [EntryField("MarginV")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.marginV = value;
            }
        }

        [EntryField("Encoding")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Encoding
        {
            get => this.encoding;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.encoding = value;
            }
        }

        #endregion Fields

        /// <summary>
        /// Name of this <see cref="Entry"/>, will be "Style".
        /// </summary>
        protected sealed override string EntryName => "Style";
    }
}
