using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    public class SubEventTest : TestBase
    {

        [TestMethod]
        public void CommentAll()
        {
            foreach (var file in this.TestHelper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value).Result;
                foreach (var item in t.EventCollection)
                    item.IsComment = true;
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize()).Result;
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), false);
            }
        }

        [TestMethod]
        public void CommentAllParallel()
        {
            this.TestHelper.LoadTestFiles().AsParallel().Select(item =>
            {
                var r = item.Value;
                return Subtitle.Parse<AssScriptInfo>(r).Result;
            }).ForAll(t =>
            {
                t.EventCollection.AsParallel().ForAll(ev => ev.IsComment = true);
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize()).Result;
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), false);
            });
        }

        [TestMethod]
        public void DecommentAll()
        {
            foreach (var file in this.TestHelper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value).Result;
                foreach (var item in t.EventCollection)
                    item.IsComment = false;
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize()).Result;
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), true);
            }
        }

        [TestMethod]
        public void Clone()
        {
            var sub = Subtitle.Parse<AssScriptInfo>(this.TestHelper.TestFile).Result;
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
