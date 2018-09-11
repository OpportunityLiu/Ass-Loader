using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    public class SubEventTest : TestBase
    {
        [SubTestMethod]
        public void CommentAll(ParseResult<AssScriptInfo> file)
        {
            var t = file.Result;
            foreach (var item in t.EventCollection)
                item.IsComment = true;
            var x = Subtitle.Parse<AssScriptInfo>(t.Serialize()).Result;
            var c = from ev in x.EventCollection
                    select ev.IsComment;
            CollectionAssert.DoesNotContain(c.ToList(), false);
        }

        [SubTestMethod]
        public void DecommentAll(ParseResult<AssScriptInfo> file)
        {
            var t = file.Result;
            foreach (var item in t.EventCollection)
                item.IsComment = false;
            var x = Subtitle.Parse<AssScriptInfo>(t.Serialize()).Result;
            var c = from ev in x.EventCollection
                    select ev.IsComment;
            CollectionAssert.DoesNotContain(c.ToList(), true);
        }

        [SubTestMethod]
        public void Clone(ParseResult<AssScriptInfo> file)
        {
            var sub = file.Result;
            foreach (var subeve in sub.EventCollection)
            {
                var clone = subeve.Clone();
                Assert.AreEqual(subeve.StartTime, clone.StartTime);
                Assert.AreEqual(subeve.Style, clone.Style);
                Assert.AreEqual(subeve.Text, clone.Text);
                Assert.AreEqual(subeve.MarginV, clone.MarginV);
                Assert.AreEqual(subeve.IsComment, clone.IsComment);
                Assert.AreEqual(subeve.Effect, clone.Effect);
            }
        }
    }
}
