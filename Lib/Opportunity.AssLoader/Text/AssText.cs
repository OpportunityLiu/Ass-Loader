using ColorCode;
using ColorCode.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Opportunity.AssLoader.Text
{
    internal class AssText : ILanguage
    {
        public AssText()
        {

        }
        public string Id => "asstext";
        public string FirstLinePattern => null;
        public string Name => "Ass Text";
        public IList<LanguageRule> Rules { get; } = new List<LanguageRule>
        {
            new LanguageRule(@"(\\n|\\N|\\h)", new Dictionary<int,string>
            {
                [1]= ScopeName.ControlKeyword
            }),
            new LanguageRule(@"(?six)
({)(?:\s*
  (?:
    ([^\\}]+)
  |
    (
      (\\)([0-9]*[a-z]+)\s*
      (?:
        ((\() \s*([^\\\(\)}]*)\s* (\)))
        |
        ([^\\\(\)}]*)
      )\s*
    )
  )
\s*)*(})", new Dictionary<int,string>
            {
                [1]= ScopeName.Brackets,
                [2]=ScopeName.Comment,
                [4]=ScopeName.Operator,
                [5]=ScopeName.Keyword,
                [8]=ScopeName.Number,
                [10]=ScopeName.Number,
                [11]=ScopeName.Brackets,
            })
        };
        public string CssClassName => "ass-text";

        public bool HasAlias(string lang) => false;
    }
}
