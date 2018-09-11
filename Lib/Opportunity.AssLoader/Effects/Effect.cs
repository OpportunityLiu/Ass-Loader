using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Effects.Effect, Opportunity.AssLoader.Effects.EffectFieldAttribute>;

namespace Opportunity.AssLoader.Effects
{
    /// <summary>
    /// Effect that is not registered via <see cref="Effect.Register{T}()"/>.
    /// </summary>
    public sealed class UnknownEffect : Effect
    {
        /// <summary>
        /// Create new instance of <see cref="UnknownEffect"/>.
        /// </summary>
        /// <param name="name">Name of the effect.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid effect name.</exception>
        public UnknownEffect(string name)
            : this(name, new List<string>())
        {
            try
            {
                EffectNameCheck(name, typeof(UnknownEffect));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid name.", ex);
            }
        }

        internal UnknownEffect(string name, IList<string> args)
        {
            this.name = name;
            this.Arguments = args;
        }

        private readonly string name;
        /// <summary>
        /// Name of the effect.
        /// </summary>
        public override string Name => this.name;

        /// <summary>
        /// Arguments of the effect.
        /// </summary>
        public IList<string> Arguments { get; }
    }

    /// <summary>
    /// Base class for all effects.
    /// </summary>
    [DebuggerDisplay(@"[{Name,nq}]")]
    public abstract class Effect
    {
        internal static readonly Dictionary<string, SerializeDataStore> Types = new Dictionary<string, SerializeDataStore>(StringComparer.OrdinalIgnoreCase)
        {
            [ScrollUpEffect.NAME] = SerializeDataStore.Create<ScrollUpEffect>(),
            [ScrollDownEffect.NAME] = SerializeDataStore.Create<ScrollDownEffect>(),
            [BannerEffect.NAME] = SerializeDataStore.Create<BannerEffect>(),
        };

        /// <summary>
        /// Register effects for parsing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static void Register<T>()
            where T : Effect, new()
        {
            if (typeof(T) == typeof(UnknownEffect))
                return;
            try
            {
                var test = new T();
                var name = test.Name;
                if (Types.TryGetValue(name, out var oldvalue) && test.GetType() != oldvalue.EffectType)
                    throw new InvalidOperationException($"Effect with same name({name}) has been regeistered by another type({oldvalue.EffectType}).");
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        internal static Effect Parse(ReadOnlySpan<char> data, IDeserializeInfo deserializeInfo)
        {
            var index = -1;
            var fi = default(SerializeDataStore);
            var eff = default(Effect);
            var fields = default(List<string>);
            foreach (var item in data.Split(';'))
            {
                var field = item.Trim();
                if (eff is null)
                {
                    var key = field.ToString();
                    if (Types.TryGetValue(key, out var info))
                    {
                        eff = (Effect)Activator.CreateInstance(info.EffectType);
                        fi = info;
                    }
                    else
                    {
                        fields = new List<string>();
                        eff = new UnknownEffect(key, fields);
                        deserializeInfo.AddException(new FormatException($"Unknown effect name `{key}` of Effect."));
                    }
                }
                else if (fi is null)
                {
                    fields.Add(field.ToString());
                }
                else
                {
                    if (index >= fi.FieldInfo.Length)
                    {
                        deserializeInfo.AddException(new FormatException($"Too many fields for effect {fi.EffectType.FullName}."));
                        break;
                    }
                    fi.FieldInfo[index].Deserialize(field, eff, deserializeInfo);
                }
                index++;
            }
            return eff;
        }

        /// <summary>
        /// A read only collection of registered effect names.
        /// </summary>
        public static IReadOnlyCollection<string> RegisteredNames => Types.Keys;

        /// <summary>
        /// Name of the effect.
        /// </summary>
        /// <remarks>
        /// Must always returns a same value for same type.
        /// </remarks>
        public abstract string Name { get; }

        internal static void EffectNameCheck(string effectName, Type effectType)
        {
            var name = effectName;
            if (!FormatHelper.FieldStringValueValid(ref effectName) || effectName != name || effectName.IndexOf(';') >= 0)
                throw new InvalidOperationException($"Invalid Name({name}) provided by the effect class {effectType.FullName}.");
        }

        /// <summary>
        /// Create new instance of <see cref="Effect"/>.
        /// </summary>
        protected Effect()
        {
            var t = GetType();
            if (t == typeof(UnknownEffect))
                return;
            var effectName = Name;
            if (!Types.TryGetValue(effectName, out this.SerializeData))
            {
                EffectNameCheck(effectName, t);
                Types[effectName] = this.SerializeData = new SerializeDataStore(t);
            }
            if (t != this.SerializeData.EffectType)
                throw new InvalidOperationException($"Effect with same name({effectName}) has been regeistered by another type({this.SerializeData.EffectType}).");
        }

        internal sealed class SerializeDataStore
        {
            public static SerializeDataStore Create<T>()
                where T : Effect, new()
            {
                return new SerializeDataStore(typeof(T));
            }

            public SerializeDataStore(Type effectType)
            {
                this.EffectType = effectType;
                this.FieldInfo = FieldSerializeHelper.GetScriptInfoFields(effectType).Values.OrderBy(f => f.Info.Order).ToArray();
            }

            public readonly Type EffectType;

            public readonly FieldSerializeHelper[] FieldInfo;
        }

        internal readonly SerializeDataStore SerializeData;

        internal void Serialize(TextWriter writer, ISerializeInfo serializeInfo)
        {
            writer.Write(this.Name);
            var f = this.SerializeData?.FieldInfo;
            if (f is null)
            {
                var d = ((UnknownEffect)this).Arguments;
                foreach (var item in d)
                {
                    writer.Write(';');
                    var v = item;
                    FormatHelper.FieldStringValueValid(ref v);
                    writer.Write(v);
                }
            }
            else
            {
                for (var i = 0; i < f.Length; i++)
                {
                    writer.Write(';');
                    f[i].Serialize(writer, this, serializeInfo);
                }
            }
        }
    }
}
