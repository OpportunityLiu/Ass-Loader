using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader.Serializer;

namespace AssLoader
{
    public class SubEvent : Entry
    {
        public static SubEvent Parse(EntryHeader format, bool isComment, string fields)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            ThrowHelper.ThrowIfNullOrEmpty(fields, "fields");
            var re = new SubEvent();
            re.Parse(fields, format);
            re.IsComment = isComment;
            return re;
        }

        public static SubEvent ParseExact(EntryHeader format, bool isComment, string fields)
        {
            ThrowHelper.ThrowIfNull(format, "format");
            ThrowHelper.ThrowIfNullOrEmpty(fields, "fields");
            var re = new SubEvent();
            re.IsComment = isComment;
            re.ParseExact(fields, format);
            return re;
        }

        public SubEvent Clone()
        {
            var re = base.Clone<SubEvent>();
            return re;
        }

        public override string ToString()
        {
            return EntryName + ": " + text.ToString();
        }

        protected sealed override string EntryName
        {
            get
            {
                if(IsComment)
                    return "Comment";
                else
                    return "Dialogue";
            }
        }

        private bool iscomment;

        public bool IsComment
        {
            get
            {
                return iscomment;
            }
            set
            {
                iscomment = value;
                PropertyChanging();
                PropertyChanging("EntryName");
            }
        }

        [EntryField("Layer")]
        private int layer = 0;

        public int Layer
        {
            get
            {
                return layer;
            }
            set
            {
                ThrowHelper.ThrowIfLessThanZero(value, "value");
                layer = value;
                PropertyChanging();
            }
        }

        [TimeSerialize]
        [EntryField("Start")]
        private DateTime startTime;

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(value.Kind != DateTimeKind.Unspecified)
                    throw new ArgumentException("Specified datetime isn't needed.");
                if(value >= endTime)
                    startTime = endTime;
                else
                    startTime = value;
                PropertyChanging();
            }
        }

        [TimeSerialize]
        [EntryField("End")]
        private DateTime endTime;

        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(value.Kind != DateTimeKind.Unspecified)
                    throw new ArgumentException("Specified datetime isn't needed.");
                if(value <= startTime)
                    endTime = startTime;
                else
                    endTime = value;
                PropertyChanging();
            }
        }

        [EntryField("Style", DefaultValue = "*Default")]
        private string style;

        public string Style
        {
            get
            {
                return style;
            }
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                    style = value;
                else
                    style = null;
                PropertyChanging();
            }
        }

        [EntryField("Name", Alias = "Actor", DefaultValue = "")]
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                    name = value;
                else
                    name = null;
                PropertyChanging();
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

        [EntryField("Effect", DefaultValue = "")]
        private string effect;

        public string Effect
        {
            get
            {
                return effect;
            }
            set
            {
                if(FormatHelper.FieldStringValueValid(ref value))
                    effect = value;
                else
                    effect = null;
                PropertyChanging();
            }
        }

        [TextSerialize]
        [EntryField("Text")]
        private TextContent text = null;

        public TextContent Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                PropertyChanging();
            }
        }
    }
}
