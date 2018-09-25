using Opportunity.AssLoader.Serializer;
using System.Collections.Generic;

namespace Opportunity.AssLoader.Text
{
    [TagDefination("b")]
    public sealed class FontWeightTag : Tag
    {
        [TagField(0)]
        public int? Value { get; set; }
    }

    [TagDefination("i")]
    public sealed class FontStyleTag : Tag
    {
        [TagField(0)]
        [BooleanSerialize(FalseString = "0", TrueString = "1")]
        public bool? IsItalic { get; set; }
    }

    [TagDefination("u")]
    public sealed class UnderlineTag : Tag
    {
        [TagField(0)]
        [BooleanSerialize(FalseString = "0", TrueString = "1")]
        public bool? Value { get; set; }
    }

    [TagDefination("s")]
    public sealed class StrikeOutTag : Tag
    {
        [TagField(0)]
        [BooleanSerialize(FalseString = "0", TrueString = "1")]
        public bool? Value { get; set; }
    }

    [TagDefination("t")]
    public sealed class TransformTag : Tag
    {
        [TagField(0, Priority = 1)]
        public int? TStart { get; set; }
        [TagField(1, Priority = 1)]
        public int? TEnd { get; set; }
        [TagField(2, Priority = 2)]
        public double Acceleration { get; set; } = 1;
        [TagField(3)]
        [TagSerialize]
        public IList<Tag> Style { get; set; } = new List<Tag>();
    }
}
