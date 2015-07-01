using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader.Serializer;

namespace AssLoader
{
    public class Style : Entry
    {
        public static Style Parse(EntryHeader format, string fields)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            ThrowHelper.ThrowIfNullOrEmpty(fields, "fields");
            var re = new Style();
            re.Parse(fields, format);
            return re;
        }

        public static Style ParseExact(EntryHeader format, string fields)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            ThrowHelper.ThrowIfNullOrEmpty(fields, "fields");
            var re = new Style();
            re.ParseExact(fields, format);
            return re;
        }

        public Style Clone(string newName)
        {
            var n = base.Clone<Style>();
            n.Name = newName;
            return n;
        }

        public override string ToString()
        {
            return "Style: " + name;
        }

        #region Fields

        [EntryField("Name", DefaultValue = "Default")]
        private string name;

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
        private string fontname;

        public string Fontname
        {
            get
            {
                return fontname;
            }
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                {
                    fontname = value;
                    PropertyChanging();
                }
                else
                    throw new ArgumentNullException("value");
            }
        }

        [EntryField("Fontsize")]
        private double fontsize = 12;

        public double Fontsize
        {
            get
            {
                return fontsize;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                fontsize = value;
                PropertyChanging();
            }
        }

        [ColorSerialize]
        [EntryField("PrimaryColour")]
        private Color primaryColor;

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
        private Color backColor;

        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                PropertyChanging();
            }
        }

        [EntryField("Bold")]
        private int bold;

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

        [EntryField("StrikeOut")]
        private int strikeOut;

        public bool StrikeOut
        {
            get
            {
                return strikeOut == -1;
            }
            set
            {
                if(value)
                    strikeOut = -1;
                else
                    strikeOut = 0;
                PropertyChanging();
            }
        }

        [EntryField("ScaleX")]
        private double scaleX = 100;

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
        private double angle;

        public double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value % 360;
                if(angle > 180)
                    angle -= 360;
                else if(angle < -180)
                    angle += 360;
                PropertyChanging();
            }
        }

        [EntryField("BorderStyle")]
        private int borderStyle = (int)AssLoader.BorderStyle.OutlineAndDropShadow;

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
        private int alignment = (int)AlignmentStyle.BottomCentered;

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
                case AlignmentStyle.BottomCentered:
                case AlignmentStyle.BottomRight:
                case AlignmentStyle.MiddleLeft:
                case AlignmentStyle.MiddleCentered:
                case AlignmentStyle.MiddleRight:
                case AlignmentStyle.TopLeft:
                case AlignmentStyle.TopCentered:
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

        protected sealed override string EntryName
        {
            get
            {
                return "Style";
            }
        }
    }
}
