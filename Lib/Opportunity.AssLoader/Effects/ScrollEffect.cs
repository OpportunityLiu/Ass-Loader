using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Opportunity.AssLoader
{
    public static class Effects
    {
        internal static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public static IReadOnlyDictionary<string, Type> Registered { get; } = new ReadOnlyDictionary<string, Type>(Types);

        /// <summary>
        /// Register effects for parsing, effects found in serializing will be registered automatically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="effectName"></param>
        /// <exception cref="ArgumentNullException"><paramref name="effectName"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="ArgumentException"><paramref name="effectName"/> contains line breaks.</exception>
        public static void Register<T>(string effectName)
            where T : EffectBase, new()
        {
            if (!FormatHelper.FieldStringValueValid(ref effectName))
                throw new ArgumentNullException(nameof(effectName));
            Types[effectName] = typeof(T);
        }
    }

    public abstract class EffectBase
    {
    }

    internal class ScrollEffect
    {
    }
}
