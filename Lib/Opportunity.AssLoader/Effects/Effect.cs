using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            EffectDefinationAttribute.EffectNameCheck(ref name);
        }

        internal UnknownEffect(string name, IList<string> args)
        {
            this.Name = name;
            this.Arguments = args;
        }

        /// <summary>
        /// Name of the effect.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Arguments of the effect.
        /// </summary>
        public IList<string> Arguments { get; }
    }

    /// <summary>
    /// Base class for all effects. All implemetation should have <see cref="EffectDefinationAttribute"/>.
    /// </summary>
    [DebuggerDisplay(@"[{Name,nq}]")]
    public abstract class Effect
    {
        static Effect()
        {
            Register<ScrollUpEffect>();
            Register<ScrollDownEffect>();
            Register<BannerEffect>();
        }


        internal static readonly Dictionary<string, SerializeDataStore> Names = new Dictionary<string, SerializeDataStore>(StringComparer.OrdinalIgnoreCase);

        internal static readonly Dictionary<Type, string> Types = new Dictionary<Type, string>();

        internal static SerializeDataStore Register(Type type)
        {
            EffectDefinationAttribute attr;
            try
            {
                attr = type.GetCustomAttribute<EffectDefinationAttribute>(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid EffectDefinationAttribute.", ex);
            }
            if (attr is null)
                throw new InvalidOperationException($"Effect must have EffectDefinationAttribute.");
            var name = attr.Name;
            if (Names.TryGetValue(name, out var oldvalue))
            {
                if (oldvalue.EffectType == type)
                    return oldvalue;
                else
                    throw new InvalidOperationException($"Effect with same name({name}) has been regeistered by another type({oldvalue.EffectType}).");
            }

            Types[type] = name;
            return Names[name] = new SerializeDataStore(type, name);
        }

        /// <summary>
        /// Register effects for parsing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : Effect, new()
        {
            var type = typeof(T);
            if (type == typeof(UnknownEffect))
                return;
            Register(type);
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
                    if (Names.TryGetValue(key, out var info))
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
        public static IReadOnlyCollection<string> RegisteredNames => Names.Keys;

        /// <summary>
        /// Create new instance of <see cref="Effect"/>.
        /// </summary>
        protected Effect()
        {
            var t = GetType();
            if (t == typeof(UnknownEffect))
                return;
            this.SerializeData = Types.TryGetValue(t, out var name) ? Names[name] : Register(t);
        }

        internal sealed class SerializeDataStore
        {
            public SerializeDataStore(Type effectType, string name)
            {
                this.Name = name;
                this.EffectType = effectType;
                this.FieldInfo = FieldSerializeHelper.GetScriptInfoFields(effectType).Values.OrderBy(f => f.Info.Order).ToArray();
            }

            public readonly string Name;

            public readonly Type EffectType;

            public readonly FieldSerializeHelper[] FieldInfo;
        }

        internal readonly SerializeDataStore SerializeData;

        internal void Serialize(TextWriter writer, ISerializeInfo serializeInfo)
        {
            writer.Write(this.SerializeData.Name);
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
