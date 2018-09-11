using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Entry, Opportunity.AssLoader.EntryFieldAttribute>;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Entry of "events" section.
    /// </summary>
    [DebuggerDisplay(@"{EntryName, nq}: {text.ToString(), nq}")]
    public sealed class SubEvent : Entry
    {
        internal static readonly Dictionary<string, FieldSerializeHelper> FieldInfo = FieldSerializeHelper.GetScriptInfoFields(typeof(SubEvent));

        /// <summary>
        /// Create new instance of <see cref="SubEvent"/>.
        /// </summary>
        public SubEvent()
        {
        }

        /// <summary>
        /// Make a copy of this <see cref="SubEvent"/>.
        /// </summary>
        /// <returns>A copy of this <see cref="SubEvent"/>.</returns>
        public SubEvent Clone()
        {
            return (SubEvent)this.MemberwiseClone();
        }

        /// <summary>
        /// Name of this <see cref="Entry"/>, will be "Comment" or "Dialogue".
        /// </summary>
        protected sealed override string EntryName => this.IsComment ? "Comment" : "Dialogue";

        /// <summary>
        /// Whether the <see cref="SubEvent"/> is comment or not.
        /// </summary>
        public bool IsComment { get; set; }

        #region Fields

        [EntryField("Layer")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int layer = 0;

        /// <summary>
        /// Layer for this <see cref="SubEvent"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <remarks>
        /// If you override positioning with an override tag so that two or more lines are displayed on top of each other, this field controls which one is drawn where; higher layer numbers are drawn on top of lower ones.
        /// </remarks>
        public int Layer
        {
            get => this.layer;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.layer = value;
            }
        }

        [TimeSerialize]
        [EntryField("Start")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TimeSpan startTime;

        /// <summary>
        /// Start time for this <see cref="SubEvent"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> late than <see cref="EndTime"/>.
        /// </exception>
        public TimeSpan StartTime
        {
            get => this.startTime;
            set
            {
                if (value > this.endTime)
                    throw new ArgumentOutOfRangeException(nameof(value), "StartTime must earlier than EndTime.");
                this.startTime = value;
            }
        }

        [TimeSerialize]
        [EntryField("End")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TimeSpan endTime;

        /// <summary>
        /// End time for this <see cref="SubEvent"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> earlier than <see cref="StartTime"/>.
        /// </exception>
        public TimeSpan EndTime
        {
            get => this.endTime;
            set
            {
                if (value < this.startTime)
                    throw new ArgumentOutOfRangeException(nameof(value), "StartTime must earlier than EndTime.");
                this.endTime = value;
            }
        }

        [EntryField("Style", DefaultValue = "*Default")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string style;

        /// <summary>
        /// <see cref="Style.Name"/> of the <see cref="AssLoader.Style"/> used for this <see cref="SubEvent"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string Style
        {
            get => this.style;
            set
            {
                if (!FormatHelper.FieldStringValueValid(ref value))
                    value = null;
                this.style = value;
            }
        }

        [EntryField("Name", Alias = "Actor")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string name;

        /// <summary>
        /// The actor speaking this line.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        /// <remarks>
        /// Has no actual effect on subtitle display but can be useful for editing purposes.
        /// </remarks>
        public string Name
        {
            get => this.name;
            set
            {
                if (!FormatHelper.FieldStringValueValid(ref value))
                    value = null;
                this.name = value;
            }
        }

        [EntryField("MarginL")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int marginL;

        /// <summary>
        /// Left margin of the <see cref="SubEvent"/>.
        /// 0 means use the margin specified in the <see cref="Style"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
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
        /// Right margin of the <see cref="SubEvent"/>.
        /// 0 means use the margin specified in the <see cref="Style"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
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
        /// Vetical margin of the <see cref="SubEvent"/>.
        /// 0 means use the margin specified in the <see cref="Style"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
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

        [EntryField("Effect")]
        [EffectSerialize]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Effects.Effect effect;

        /// <summary>
        /// Effect for this <see cref="SubEvent"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        /// <remarks>
        /// There are a few predefined effects which can be applied via this field,
        /// but renderer support for them is spotty and using override tags is nearly always a better idea.
        /// This is commonly used as a metadata field for automation scripts.
        /// </remarks>
        public Effects.Effect Effect
        {
            get => this.effect;
            set => this.effect = value;
        }

        [TextSerialize]
        [EntryField("Text")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TextContent text = null;

        /// <summary>
        /// Text of this <see cref="SubEvent"/>.
        /// </summary>
        public TextContent Text
        {
            get => this.text;
            set => this.text = value;
        }

        #endregion
    }
}
