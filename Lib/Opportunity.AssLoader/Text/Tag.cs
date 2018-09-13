﻿using Opportunity.AssLoader.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Opportunity.AssLoader.Text
{
    internal ref struct TagParser
    {
        private ReadOnlySpan<char> data;
        private List<Tag> result;
        private readonly IDeserializeInfo di;

        public TagParser(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo) : this()
        {
            this.data = value;
            this.di = deserializeInfo;
        }

        private void addTag(Tag tag)
        {
            if (this.result is null)
                this.result = new List<Tag>();
            this.result.Add(tag);
        }

        private void addTag(ReadOnlySpan<char> tagname)
        {
            Console.WriteLine($"Tag {tagname.ToString()}");
        }

        private void addComment(ReadOnlySpan<char> comment)
        {
            Console.WriteLine($"Comment {comment.ToString()}");
        }

        private void addTag(ReadOnlySpan<char> tagname, ReadOnlySpan<char> param)
        {
            Console.WriteLine($"Tag {tagname.ToString()}, param: {param.ToString()}");
        }

        private void addAdvTag(ReadOnlySpan<char> tagname, ReadOnlySpan<char> param)
        {
            Span<int> commas = stackalloc int[param.Length];
            commas.Fill(-1);
            var p = 0;
            var br = 0;
            var pc = 0;
            for (var i = 0; i < param.Length; i++)
            {
                if (param[i] == '(')
                    br++;
                else if (param[i] == ')')
                    br--;
                if (br == 0 && param[i] == ',')
                    commas[p++] = i;
            }
            Console.WriteLine($"AdvTag {tagname.ToString()}, param: ");
            foreach (var index in commas)
            {
                if (index < 0)
                    break;
                var par = param.Slice(0, index).Trim();
                param = param.Slice(index + 1);
                Console.WriteLine($"[{pc++}]: {par.ToString()}");
            }
            Console.WriteLine($"[{pc}]: {param.ToString()}");
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
                        addComment(this.data);
                        return;
                    }
                    else
                        addComment(this.data.Slice(0, start).TrimEnd());
                }
                this.data = this.data.Slice(start + 1);
                skipWhiteSpace();
                var tname = parseTagname();
                if (tname.IsEmpty)
                {
                    this.di.AddException(new FormatException($"Missing tag name after `\\`."));
                    continue;
                }
                if (this.data.IsEmpty || this.data[0] == '\\')
                {
                    addTag(tname);
                    continue;
                }

                if (this.data[0] != '(')
                {
                    var param = parseParam();
                    addTag(tname, param);
                    continue;
                }
                else
                {
                    var end = 1;
                    var br = 1;
                    for (; end < this.data.Length && br > 0; end++)
                    {
                        if (this.data[end] == '(')
                            br++;
                        else if (this.data[end] == ')')
                            br--;
                    }
                    var param = this.data.Slice(1, end - 2);
                    this.data = this.data.Slice(end);
                    if (br != 0)
                    {
                        this.di.AddException(new FormatException($"Missing `)` in tag `{tname.ToString()}`."));
                        Span<char> param2 = stackalloc char[param.Length + br];
                        param2.Fill(')');
                        param.CopyTo(param2);
                        param = param2.ToString().AsSpan();
                    }
                    addAdvTag(tname, param);
                    continue;
                }
            }
        }

        private ReadOnlySpan<char> parseParam()
        {
            skipWhiteSpace();
            var end = this.data.IndexOf('\\');
            if (end < 0)
            {
                var c2 = this.data;
                this.data = default;
                return c2;
            }
            var c = this.data.Slice(0, end).TrimEnd();
            this.data = this.data.Slice(end);
            return c;
        }

        private ReadOnlySpan<char> parseTagname()
        {
            var data = this.data;
            var i = 0;
            for (; i < this.data.Length; i++)
            {
                if ('0' <= this.data[i] && this.data[i] <= '9')
                    continue;
                break;
            }
            for (; i < this.data.Length; i++)
            {
                if (('a' <= this.data[i] && this.data[i] <= 'z') || ('A' <= this.data[i] && this.data[i] <= 'Z'))
                    continue;
                break;
            }
            var r = this.data.Slice(0, i);
            this.data = this.data.Slice(i);
            skipWhiteSpace();
            return r;
        }

        public static Tag[] Parse(ReadOnlySpan<char> value, IDeserializeInfo deserializeInfo)
        {
            if (value.IsWhiteSpace())
                return Array.Empty<Tag>();
            var p = new TagParser(value, deserializeInfo);
            p.parse();
            if (p.result is null)
                return Array.Empty<Tag>();
            return p.result.ToArray();
        }
    }

    public abstract class Tag
    {
        public static void Register<T>()
        {
        }
    }

    /// <summary>
    /// Tag that is not registered via <see cref="Tag.Register{T}()"/>.
    /// </summary>
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
