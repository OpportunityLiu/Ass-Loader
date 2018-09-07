﻿using System;
using System.Linq;
using Opportunity.AssLoader.Serializer;

namespace Opportunity.AssLoader.Collections
{
    /// <summary>
    /// Container of the "script info" section which provides access to common properties of script info.
    /// </summary>
    public class AssScriptInfo : ScriptInfoCollection
    {
        /// <summary>
        /// Create a new instance of <see cref="AssScriptInfo"/> with default values of properties.
        /// </summary>
        public AssScriptInfo()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="AssScriptInfo"/> with given properties.
        /// </summary>
        /// <param name="originalScript">The original author(s) of the script.</param>
        /// <param name="title">This is a description of the script.</param>
        public AssScriptInfo(string title,string originalScript)
        {
            this.Title = title;
            this.OriginalScript = originalScript;
        }

        [ScriptInfo("ScriptType", DefaultValue = "v4.00+")]
        private string scriptType = "v4.00+";

        /// <summary>
        /// This is the SSA script fields version.
        /// </summary>
        public string ScriptType => this.scriptType;

        [ScriptInfo("Title", DefaultValue = "untitled")]
        private string title;

        /// <summary>
        /// This is a description of the script.
        /// If the original author(s) did not provide this information then "untitled" is automatically substituted.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string Title
        {
            get => this.title;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.title, value);
            }
        }

        [ScriptInfo("Original Script", DefaultValue = "unknown")]
        private string originalScript;

        /// <summary>
        /// The original author(s) of the script.
        /// If the original author(s) did not provide this information then "unknown" is automatically substituted.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string OriginalScript
        {
            get => this.originalScript;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.originalScript, value);
            }
        }

        [ScriptInfo("Original Translation", IsOptional = true)]
        private string originalTranslation;

        /// <summary>
        /// (optional) The original translator of the dialogue. 
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string OriginalTranslation
        {
            get => this.originalTranslation;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.originalTranslation, value);
            }
        }

        [ScriptInfo("Original Editing", IsOptional = true)]
        private string originalEditing;

        /// <summary>
        /// (optional) The original script editor(s), 
        /// typically whoever took the raw translation and turned it into idiomatic english and reworded for readability. 
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string OriginalEditing
        {
            get => this.originalEditing;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.originalEditing, value);
            }
        }

        [ScriptInfo("Original Timing", IsOptional = true)]
        private string originalTiming;

        /// <summary>
        /// (optional) Whoever timed the original script.
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string OriginalTiming
        {
            get => this.originalTiming;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.originalTiming, value);
            }
        }

        [ScriptInfo("Synch Point", IsOptional = true)]
        private string synchPoint;

        /// <summary>
        /// (optional) Description of where in the video the script should begin playback.
        /// This entry does not appear if no information was entered by the author.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string SynchPoint
        {
            get => this.synchPoint;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.synchPoint, value);
            }
        }

        [ScriptInfo("Script Updated By", IsOptional = true)]
        private string scriptUpdatedBy;

        /// <summary>
        /// (optional) Names of any other subtitling groups who edited the original script.
        /// This entry does not appear if subsequent editors did not enter the information.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string ScriptUpdatedBy
        {
            get => this.scriptUpdatedBy;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.scriptUpdatedBy, value);
            }
        }

        [ScriptInfo("Update Details", IsOptional = true)]
        private string updateDetails;

        /// <summary>
        /// The details of any updates to the original script - made by other subtitling groups.
        /// This entry does not appear if subsequent editors did not enter any information.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> contains line breaks.</exception>
        public string UpdateDetails
        {
            get => this.updateDetails;
            set
            {
                if(!FormatHelper.SingleLineStringValueValid(ref value))
                    value = null;
                this.Set(ref this.updateDetails, value);
            }
        }

        [ScriptInfo("Collisions", DefaultValue = CollisionStyle.Normal, IsOptional = true)]
        private CollisionStyle collisions;

        /// <summary>
        /// This determines how subtitles are moved, when automatically preventing onscreen collisions.
        /// </summary>
        public CollisionStyle Collisions
        {
            get => this.collisions;
            set => this.Set(ref this.collisions, value);
        }


        [ScriptInfo("PlayResY", DefaultValue = 360)]
        private int playResY = 360;

        /// <summary>
        /// This is the height of the screen used by the script's author(s) when playing the script. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <seealso cref="PlayResX"/>
        public int PlayResY
        {
            get => this.playResY;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is less than 0.");
                this.Set(ref this.playResY, value);
            }
        }

        [ScriptInfo("PlayResX", DefaultValue = 640)]
        private int playResX = 640;

        /// <summary>
        /// This is the width of the screen used by the script's author(s) when playing the script.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 0.
        /// </exception>
        /// <seealso cref="PlayResY"/>
        public int PlayResX
        {
            get => this.playResX;
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is less than 0.");
                this.Set(ref this.playResX, value);
            }
        }

        [ScriptInfo("PlayDepth", IsOptional = true)]
        private int? playDepth;

        /// <summary>
        /// (optional) This is the color depth used by the script's author(s) when playing the script.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not greater than 0.
        /// </exception>
        public int? PlayDepth
        {
            get => this.playDepth;
            set
            {
                if(value.HasValue)
                {
                    if(value.Value <= 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than 0.");
                }
                this.Set(ref this.playDepth, value);
            }
        }

        [ScriptInfo("Timer", DefaultValue = 100d, IsOptional = true, Format = "F4")]
        private double? timer;

        /// <summary>
        /// (optional) This is the Timer Speed for the script, as percentage.
        /// </summary>
        /// <example> 
        /// "100.0000" is exactly 100%. It has four digits following the decimal point.
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not greater than 0.
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
        public double? Timer
        {
            get => this.timer;
            set
            {
                if(value.HasValue)
                {
                    if(ThrowHelper.IsInvalidDouble(value.Value))
                        throw new ArgumentException("value should be a valid number", nameof(value));
                    if(value.Value <= 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than 0.");
                }
                this.Set(ref this.timer, value);
            }
        }

        [ScriptInfo("WrapStyle", DefaultValue = 0)]
        private int wrapStyle = 0;

        /// <summary>
        /// Defines the default wrapping style. 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not one of the valid value.
        /// </exception>
        public WrapStyle WrapStyle
        {
            get => (WrapStyle)this.wrapStyle;
            set
            {
                var num = (int)value;
                if(ThrowHelper.IsLessThanZeroOrOutOfRange(4, num))
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.Set(ref this.wrapStyle, num);
            }
        }

        [BooleanSerialize(TrueString = "Yes", FalseString = "No", ThrowOnDeserializing = false)]
        [ScriptInfo("ScaledBorderAndShadow")]
        private bool scaledBorderAndShadow;

        /// <summary>
        /// if true, script resolution is used for <see cref="Style.Outline"/> and <see cref="Style.Shadow"/>, if false, video resolution is used. 
        /// </summary>
        public bool ScaledBorderAndShadow
        {
            get => this.scaledBorderAndShadow;
            set => this.Set(ref this.scaledBorderAndShadow, value);
        }
    }
}
