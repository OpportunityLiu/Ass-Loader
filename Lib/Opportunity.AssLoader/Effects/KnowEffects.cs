using System.Diagnostics;
using System.Text;

namespace Opportunity.AssLoader.Effects
{
    /// <summary>
    /// Effect that transforms the subtitle text.
    /// </summary>
    public abstract class TranslateEffect : Effect
    {
        /// <summary>
        /// When delay is greater than 0, moving one pixel will take (1000/delay) second. 
        /// Otherwise, the speed will be caculated by move distance and duration.
        /// </summary>
        [EffectField(20000)]
        public double Delay { get; set; }
        /// <summary>
        /// The margin to make the scrolling text at the sides transparent.
        /// </summary>
        [EffectField(40000)]
        public double FadeAwayMargin { get; set; }
    }

    /// <summary>
    /// Base class for <see cref="ScrollUpEffect"/> and <see cref="ScrollDownEffect"/>.
    /// </summary>
    public abstract class ScrollEffectBase : TranslateEffect
    {
        /// <summary>
        /// The <see cref="Y1"/> and <see cref="Y2"/> values define a vertical region on the screen in which the text will scroll.
        /// </summary>
        [EffectField(5000)]
        public double Y1 { get; set; }

        /// <summary>
        /// The <see cref="Y1"/> and <see cref="Y2"/> values define a vertical region on the screen in which the text will scroll.
        /// </summary>
        [EffectField(10000)]
        public double Y2 { get; set; }
    }

    /// <summary>
    /// Effect that the text will scroll up the screen. 
    /// </summary>
    public class ScrollUpEffect : ScrollEffectBase
    {
        internal const string NAME = "Scroll Up";
        /// <summary>
        /// Name of the effect.
        /// </summary>
        public override string Name => NAME;
    }

    /// <summary>
    /// Effect that the text will scroll down the screen. 
    /// </summary>
    public class ScrollDownEffect : ScrollEffectBase
    {
        internal const string NAME = "Scroll Down";
        /// <summary>
        /// Name of the effect.
        /// </summary>
        public override string Name => NAME;
    }

    /// <summary>
    /// Effect that text will be forced into a single line, regardless of length,
    /// and scrolled from horizontally accross the screen.
    /// </summary>
    public class BannerEffect : TranslateEffect
    {
        internal const string NAME = "Banner";
        /// <summary>
        /// Name of the effect.
        /// </summary>
        public override string Name => NAME;

        [EffectField(30000)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int l2r;
        /// <summary>
        /// The direction of banner's moving.
        /// </summary>
        public bool IsLeftToRight { get => this.l2r != 0; set => this.l2r = value ? 1 : 0; }
    }
}
