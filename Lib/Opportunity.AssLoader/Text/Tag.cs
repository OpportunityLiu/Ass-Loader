using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FieldSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.Text.Tag, Opportunity.AssLoader.Text.TagFieldAttribute>;

namespace Opportunity.AssLoader.Text
{
    internal ref struct TagParser
    {
        private readonly int length;
        private ReadOnlySpan<char> data;
        private readonly List<Tag> result;
        private readonly IDeserializeInfo di;

        public TagParser(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo) : this()
        {
            this.length = value.Length;
            this.data = value;
            this.di = deserializeInfo;
            this.result = new List<Tag>();
        }

        private void addTag(Tag tag)
        {
            this.result.Add(tag);
        }

        private void addComment(ReadOnlySpan<char> comment)
        {
            addTag(new CommentTag { Content = comment.TrimEnd().ToString() });
        }

        private void skipWhiteSpace()
        {
            this.data = this.data.TrimStart();
        }

        private void parse()
        {
            while (true)
            {
                skipWhiteSpace();
                if (this.data.IsEmpty)
                    break;
                var start = this.data.IndexOf('\\');
                if (start != 0)
                {
                    if (start < 0)
                    {
                        var data = this.data;
                        this.data = default;
                        addComment(data);
                        return;
                    }
                    else
                    {
                        var data = this.data.Slice(0, start);
                        this.data = this.data.Slice(start + 1);
                        addComment(data);
                    }
                }
                else
                    this.data = this.data.Slice(1);
                skipWhiteSpace();
                addTag();
            }
        }

        private void addTag()
        {
            var i = 0;
            var br = 0;
            var hasbr = false;
            Span<int> commapos = stackalloc int[20];
            var commacnt = -1;
            var tagname = default(ReadOnlySpan<char>);
            var lb = -1;
            var rb = -1;
            for (; i < this.data.Length; i++)
            {
                if (this.data[i] == '(')
                {
                    commacnt = 0;
                    hasbr = true;
                    br++;
                    if (br == 1)
                    {
                        tagname = this.data.Slice(0, i).TrimEnd();
                        lb = i;
                    }
                }
                else if (this.data[i] == ')')
                {
                    if (br == 1)
                        rb = i;
                    br--;
                }
                else if (this.data[i] == '\\' && br <= 0)
                    break;
                else if (this.data[i] == ',' && br == 1)
                {
                    if (commacnt == 20)
                        this.di.AddException(new FormatException($"Too mant arguments."));
                    else
                    {
                        commapos[commacnt] = i;
                        commacnt++;
                    }
                }
                if (hasbr && br == 0)
                {
                    i++;
                    break;
                }

                if (br < 0)
                    break;
            }
            if (i == 0)
            {
                this.di.AddException(new FormatException($"Missing tag name after `\\`."));
                return;
            }
            var tag = this.data.Slice(0, i);
            this.data = this.data.Slice(i);
            if (hasbr)
            {
                if (br > 0)
                {
                    this.di.AddException(new FormatException($"Missing `)` in tag `{tag.ToString()}`."));
                    Span<char> tag2 = stackalloc char[tag.Length + br];
                    tag2.Fill(')');
                    tag.CopyTo(tag2);
                    tag = tag2.ToString().AsSpan();
                    rb = tag.Length - 1;
                }
            }
            else
            {
                var hasname = false;
                foreach (var item in Tag.Names.Keys)
                {
                    if (!tag.StartsWith(item.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var param = tag.Slice(item.Length);
                    tagname = tag.Slice(0, item.Length);
                    if (item.Length != tag.Length)
                    {
                        lb = item.Length - 1;
                        rb = tag.Length;
                        commacnt = 0;
                    }
                    hasname = true;
                    break;
                }
                if (!hasname)
                {
                    var state = 0;
                    var j = 0;
                    for (; j < tag.Length; j++)
                    {
                        var ch = tag[j];
                        switch (state)
                        {
                        case 0:
                            if (ch >= '0' && ch <= '9')
                                break;
                            else if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                            {
                                state = 1;
                                break;
                            }
                            else
                            {
                                state = 2;
                                break;
                            }
                        case 1:
                            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                                break;
                            else
                            {
                                state = 2;
                                break;
                            }
                        }
                        if (state == 2)
                            break;
                    }
                    tagname = tag.Slice(0, j);
                    if (j != tag.Length)
                    {
                        lb = tagname.Length - 1;
                        rb = tag.Length;
                        commacnt = 0;
                    }
                }
            }


            {
                var tagins = default(Tag);
                var tagnamestr = tagname.ToString();
                if (!Tag.Names.TryGetValue(tagnamestr, out var fi))
                {
                    var fields = new List<string>(commacnt + 1);
                    for (var k = 0; k <= commacnt; k++)
                    {
                        var s = k == 0 ? lb : commapos[k - 1];
                        s++;
                        var p = k == commacnt ? rb : commapos[k];
                        fields.Add(tag.Slice(s, p - s).Trim().ToString());
                    }
                    tagins = new UnknownTag(tagnamestr, fields);
                }
                else
                {
                    tagins = (Tag)Activator.CreateInstance(fi.TagType);
                    var fields = fi.FieldInfo[commacnt + 1];
                    for (var k = 0; k <= commacnt; k++)
                    {
                        var s = k == 0 ? lb : commapos[k - 1];
                        s++;
                        var p = k == commacnt ? rb : commapos[k];
                        fields[k].Deserialize(tag.Slice(s, p - s).Trim(), tagins, this.di);
                    }
                }
                addTag(tagins);
            }
        }


        public static List<Tag> Parse(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            if (value.IsWhiteSpace())
                return new List<Tag>();
            var p = new TagParser(value, deserializeInfo);
            p.parse();
            return p.result;
        }
    }

    /// <summary>
    /// Tag of ass text.
    /// </summary>
    [DebuggerDisplay(@"[{SerializeData.Name,nq}]")]
    public abstract class Tag
    {
        static Tag()
        {
            Register<FontStyleTag>();
            Register<FontWeightTag>();
            Register<UnderlineTag>();
            Register<StrikeOutTag>();
            Register<TransformTag>();
        }

        internal static readonly Dictionary<string, SerializeDataStore> Names = new Dictionary<string, SerializeDataStore>(StringComparer.OrdinalIgnoreCase)
        {
            //[ScrollUpEffect.NAME] = new SerializeDataStore(typeof(ScrollUpEffect), ScrollUpEffect.NAME),
            //[ScrollDownEffect.NAME] = new SerializeDataStore(typeof(ScrollDownEffect), ScrollDownEffect.NAME),
            //[BannerEffect.NAME] = new SerializeDataStore(typeof(BannerEffect), BannerEffect.NAME),
        };

        internal static readonly Dictionary<Type, string> Types = new Dictionary<Type, string>()
        {
            //[typeof(ScrollUpEffect)] = ScrollUpEffect.NAME,
            //[typeof(ScrollDownEffect)] = ScrollDownEffect.NAME,
            //[typeof(BannerEffect)] = BannerEffect.NAME,
        };

        internal static SerializeDataStore Register(Type type)
        {
            TagDefinationAttribute attr;
            try
            {
                attr = type.GetCustomAttribute<TagDefinationAttribute>(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid TagDefinationAttribute.", ex);
            }
            if (attr is null)
                throw new InvalidOperationException($"Tag must have TagDefinationAttribute.");
            var name = attr.Name;
            if (Names.TryGetValue(name, out var oldvalue))
            {
                if (oldvalue.TagType == type)
                    return oldvalue;
                else
                    throw new InvalidOperationException($"Tag with same name({name}) has been regeistered by another type({oldvalue.TagType}).");
            }

            Types[type] = name;
            return Names[name] = new SerializeDataStore(type, name);
        }

        /// <summary>
        /// Register effects for parsing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : Tag, new()
        {
            var type = typeof(T);
            if (type == typeof(UnknownTag) || type == typeof(CommentTag))
                return;
            Register(type);
        }

        /// <summary>
        /// A read only collection of registered effect names.
        /// </summary>
        public static IReadOnlyCollection<string> RegisteredNames => Names.Keys;

        /// <summary>
        /// Create new instance of <see cref="Tag"/>.
        /// </summary>
        protected Tag()
        {
            var t = GetType();
            if (t == typeof(UnknownTag) || t == typeof(CommentTag))
                return;
            this.SerializeData = Types.TryGetValue(t, out var name) ? Names[name] : Register(t);
        }

        internal sealed class SerializeDataStore
        {
            public SerializeDataStore(Type tagType, string name)
            {
                this.Name = name;
                this.TagType = tagType;
                var fi = FieldSerializeHelper.GetScriptInfoFields(tagType).Values.Distinct().ToArray();
                var gfi = fi.GroupBy(f => f.Info.Priority).ToDictionary(g => g.Key, g => g);
                var po = gfi.Keys.OrderBy(k => k).ToArray();
                this.FieldInfo = new FieldSerializeHelper[fi.Length + 1][];
                this.FieldInfo[0] = Array.Empty<FieldSerializeHelper>();
                for (var i = 0; i < 1 << po.Length; i++)
                {
                    var lfi = new List<FieldSerializeHelper>();
                    for (var j = po.Length - 1; j >= 0; j--)
                    {
                        if ((i & (1 << j)) != 0)
                        {
                            lfi.AddRange(gfi[po[po.Length - j - 1]]);
                        }
                    }
                    this.FieldInfo[lfi.Count] = lfi.OrderBy(f => f.Info.Order).ToArray();
                }
            }

            public readonly string Name;

            public readonly Type TagType;

            public readonly FieldSerializeHelper[][] FieldInfo;
        }

        internal readonly SerializeDataStore SerializeData;

        internal void Serialize(TextWriter writer, ISerializeInfo serializeInfo)
        {
            writer.Write(this.SerializeData.Name);
            //SerializeParams(writer, serializeInfo);
            //var f = this.SerializeData?.FieldInfo;
            //if (f is null)
            //{
            //    var d = ((UnknownTag)this).Arguments;
            //    foreach (var item in d)
            //    {
            //        writer.Write(';');
            //        var v = item;
            //        FormatHelper.FieldStringValueValid(ref v);
            //        writer.Write(v);
            //    }
            //}
            //else
            //{
            //    for (var i = 0; i < f.Length; i++)
            //    {
            //        writer.Write(';');
            //        f[i].Serialize(writer, this, serializeInfo);
            //    }
            //}
        }
    }

    /// <summary>
    /// Tag that is not registered via <see cref="Tag.Register{T}()"/>.
    /// </summary>
    [DebuggerDisplay(@"[{Name,nq}]")]
    public sealed class UnknownTag : Tag
    {
        /// <summary>
        /// Create new instance of <see cref="UnknownTag"/>.
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid tag name.</exception>
        public UnknownTag(string name)
            : this(name, new List<string>())
        {
            TagDefinationAttribute.TagNameCheck(ref name);
        }

        internal UnknownTag(string name, IList<string> args)
        {
            this.Name = name;
            this.Arguments = args;
        }

        /// <summary>
        /// Name of the tag.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Arguments of the tag.
        /// </summary>
        public IList<string> Arguments { get; }
    }

    /// <summary>
    /// Inline comments in braces.
    /// </summary>
    [DebuggerDisplay(@"\{{Content,nq}\}")]
    public sealed class CommentTag : Tag
    {
        private static readonly Regex commentRegex = new Regex(@"^[^\\}]*$", RegexOptions.Compiled | RegexOptions.Singleline);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string content;

        /// <summary>
        /// Content of comments.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value" /> contains invalid chars.</exception>
        public string Content
        {
            get => this.content;
            set
            {
                if (value is null)
                {
                    this.content = "";
                    return;
                }
                value = value.Trim();
                if (!commentRegex.IsMatch(value))
                    throw new ArgumentException("value contains invalid chars.");
                this.content = value;
            }
        }
    }
}
