﻿using Opportunity.AssLoader.Serializer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Opportunity.AssLoader
{
    /// <summary>
    /// Container of the "script info" section which provides access to common properties of script info.
    /// </summary>
    public class AssScriptInfo : ScriptInfoCollection
    {
        /// <summary>
        /// Create a new instance of <see cref="AssScriptInfo"/> with default values of properties.
        /// </summary>
        public AssScriptInfo() { }

        /// <summary>
        /// Create a new instance of <see cref="AssScriptInfo"/> with given properties.
        /// </summary>
        /// <param name="originalScript">The original author(s) of the script.</param>
        /// <param name="title">This is a description of the script.</param>
        public AssScriptInfo(string title, string originalScript)
        {
            this.Title = title;
            this.OriginalScript = originalScript;
        }

        /// <summary>
        /// This is the SSA script fields version.
        /// </summary>
        [ScriptInfo("ScriptType")]
        public string ScriptType
        {
            get => "v4.00+";
            private set
            {
                if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value.Trim(), "v4.00+", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("value MUST be \"v4.00+\" for AssScriptInfo.");
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string title;

        /// <summary>
        /// This is a description of the script.
        /// If the original author(s) did not provide this information then "untitled" is automatically substituted.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Title")]
        public string Title
        {
            get => this.title;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.title = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string originalScript;

        /// <summary>
        /// The original author(s) of the script.
        /// If the original author(s) did not provide this information then "unknown" is automatically substituted.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Original Script")]
        public string OriginalScript
        {
            get => this.originalScript;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.originalScript = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string originalTranslation;

        /// <summary>
        /// (optional) The original translator of the dialogue. 
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Original Translation")]
        public string OriginalTranslation
        {
            get => this.originalTranslation;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.originalTranslation = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string originalEditing;

        /// <summary>
        /// (optional) The original script editor(s), 
        /// typically whoever took the raw translation and turned it into idiomatic english and reworded for readability. 
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Original Editing")]
        public string OriginalEditing
        {
            get => this.originalEditing;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.originalEditing = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string originalTiming;

        /// <summary>
        /// (optional) Whoever timed the original script.
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Original Timing")]
        public string OriginalTiming
        {
            get => this.originalTiming;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.originalTiming = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string scriptUpdatedBy;

        /// <summary>
        /// (optional) Names of any other subtitling groups who edited the original script.
        /// This entry does not appear if subsequent editors did not enter the information.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Script Updated By")]
        public string ScriptUpdatedBy
        {
            get => this.scriptUpdatedBy;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.scriptUpdatedBy = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string updateDetails;

        /// <summary>
        /// The details of any updates to the original script - made by other subtitling groups.
        /// This entry does not appear if subsequent editors did not enter any information.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        [ScriptInfo("Update Details")]
        public string UpdateDetails
        {
            get => this.updateDetails;
            set
            {
                if (!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.updateDetails = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CollisionStyle collisions;

        /// <summary>
        /// This determines how subtitles are moved, when automatically preventing onscreen collisions.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not one of the valid value.
        /// </exception>
        [ScriptInfo("Collisions", DefaultValue = CollisionStyle.Normal)]
        public CollisionStyle Collisions
        {
            get => this.collisions;
            set
            {
                if (!value.IsDefined())
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.collisions = value;
            }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int playResY = 360;

        /// <summary>
        /// This is the height of the screen used by the script's author(s) when playing the script. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <seealso cref="PlayResX"/>
        [ScriptInfo("PlayResY")]
        public int PlayResY
        {
            get => this.playResY;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is less than 0.");
                this.playResY = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int playResX = 640;

        /// <summary>
        /// This is the width of the screen used by the script's author(s) when playing the script.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <seealso cref="PlayResY"/>
        [ScriptInfo("PlayResX")]
        public int PlayResX
        {
            get => this.playResX;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is less than 0.");
                this.playResX = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int wrapStyle = 0;

        /// <summary>
        /// Defines the default wrapping style. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not one of the valid value.
        /// </exception>
        [ScriptInfo("WrapStyle")]
        [EnumNumberSerialize(typeof(WrapStyle))]
        public WrapStyle WrapStyle
        {
            get => (WrapStyle)this.wrapStyle;
            set
            {
                if (!value.IsDefined())
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.wrapStyle = (int)value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool scaledBorderAndShadow;

        /// <summary>
        /// if true, script resolution is used for <see cref="Style.Outline"/> and <see cref="Style.Shadow"/>, if false, video resolution is used. 
        /// </summary>
        [BooleanSerialize(TrueString = "Yes", FalseString = "No")]
        [ScriptInfo("ScaledBorderAndShadow")]
        public bool ScaledBorderAndShadow
        {
            get => this.scaledBorderAndShadow;
            set => this.scaledBorderAndShadow = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? playDepth;

        /// <summary>
        /// (optional) This is the color depth used by the script's author(s) when playing the script.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not greater than 0.
        /// </exception>
        [ScriptInfo("PlayDepth")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Obsolete(Consts.OUTDATED)]
        public int? PlayDepth
        {
            get => this.playDepth;
            set
            {
                if (value.HasValue && value.Value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than 0.");
                this.playDepth = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double? synchPoint;

        /// <summary>
        /// (optional) Description of where in the video the script should begin playback.
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        [ScriptInfo("Synch Point")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Obsolete(Consts.OUTDATED)]
        public double? SynchPoint
        {
            get => this.synchPoint;
            set
            {
                if (value != null && ThrowHelper.IsInvalidDouble(value.Value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                this.synchPoint = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double timer = 100d;

        /// <summary>
        /// (optional) This is the Timer Speed for the script, as percentage.
        /// </summary>
        /// <example> 
        /// "100.0000" is exactly 100%. It has four digits following the decimal point.
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is too small or large.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is <see cref="double.IsNaN(double)"/> or <see cref="double.IsInfinity(double)"/>
        /// </exception>
        /// <remarks> 
        /// The timer speed is alpha time multiplier applied to SSA's clock to stretch or compress the duration of alpha script.
        /// A speed greater than 100% will reduce the overall duration, and means that subtitles will progressively appear sooner and sooner.
        /// A speed less than 100% will increase the overall duration of the script means subtitles will progressively appear later and later 
        /// (like alpha positive ramp time).
        /// The stretching or compressing only occurs during script playback 
        /// - this value does not change the actual timings for each event listed in the script.
        /// 
        /// Check the SSA user guide if you want to know why "Timer Speed" is more powerful than "Ramp Time", 
        /// even though they both achieve the same result.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [ScriptInfo("Timer", Format = "F4")]
        [Obsolete(Consts.OUTDATED)]
        public double Timer
        {
            get => this.timer;
            set
            {
                if (ThrowHelper.IsInvalidDouble(value))
                    throw new ArgumentException("value should be a valid number", nameof(value));
                if (value <= 0.0001)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than 0.0001.");
                if (value >= 1000)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be less than 1000.");
                this.timer = value;
            }
        }
    }
}
