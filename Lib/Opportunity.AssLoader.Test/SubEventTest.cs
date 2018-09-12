using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    public class SubEventTest : TestBase
    {
        [FileTestMethod]
        public void CommentAll(Subtitle<AssScriptInfo> sub)
        {
            foreach (var item in sub.Events)
                item.IsComment = true;
            var x = Subtitle.Parse<AssScriptInfo>(sub.Serialize()).Result;
            var c = from ev in x.Events
                    select ev.IsComment;
            CollectionAssert.DoesNotContain(c.ToList(), false);
        }

        [FileTestMethod]
        public void DecommentAll(Subtitle<AssScriptInfo> sub)
        {
            foreach (var item in sub.Events)
                item.IsComment = false;
            var x = Subtitle.Parse<AssScriptInfo>(sub.Serialize()).Result;
            var c = from ev in x.Events
                    select ev.IsComment;
            CollectionAssert.DoesNotContain(c.ToList(), true);
        }

        [FileTestMethod]
        public void Clone(Subtitle<AssScriptInfo> sub)
        {
            foreach (var subeve in sub.Events)
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
