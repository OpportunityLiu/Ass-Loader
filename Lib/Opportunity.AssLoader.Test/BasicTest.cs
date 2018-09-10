using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class BasicTest
    {
        public BasicTest()
        {
            TestHelper.Init();
        }

        private TestHelper helper;

        public TestContext TestContext
        {
            get => this.helper.Context;
            set => this.helper = new TestHelper(value);
        }

        [TestMethod]
        public void LoadAndSave()
        {
            Subtitle<AssScriptInfo> t;
            foreach (var item in this.helper.LoadTestFiles())
            {
                using (var r = item.Value)
                    t = Subtitle.Parse<AssScriptInfo>(r);
                using (var savefile = this.helper.SaveResult(item.Key))
                    t.Serialize(savefile);
            }
        }

        [TestMethod]
        public void LoadAndSaveParallel()
        {
            this.helper.LoadTestFiles().AsParallel().Select(item =>
            {
                using (var r = item.Value)
                    return new
                    {
                        Key = item.Key,
                        Value = Subtitle.Parse<AssScriptInfo>(r)
                    };
            }).ForAll(item =>
            {
                using (var savefile = this.helper.SaveResult(item.Key))
                    item.Value.Serialize(savefile);
            });
        }

        [TestMethod]
        public void CommentAll()
        {
            foreach (var file in this.helper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value);
                foreach (var item in t.EventCollection)
                    item.IsComment = true;
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize());
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), false);
            }
        }

        [TestMethod]
        public void CommentAllParallel()
        {
            this.helper.LoadTestFiles().AsParallel().Select(item =>
            {
                using (var r = item.Value)
                    return Subtitle.Parse<AssScriptInfo>(r);
            }).ForAll(t =>
            {
                t.EventCollection.AsParallel().ForAll(ev => ev.IsComment = true);
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize());
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), false);
            });
        }

        [TestMethod]
        public void DecommentAll()
        {
            foreach (var file in this.helper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value);
                foreach (var item in t.EventCollection)
                    item.IsComment = false;
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize());
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), true);
            }
        }

        [TestMethod]
        public void Clone()
        {
            var sub = Subtitle.Parse<AssScriptInfo>(this.helper.TestFile);
            foreach (var style in sub.StyleSet)
            {
                var clone = style.Clone("Default");
                Assert.AreEqual("Default", clone.Name);
                Assert.AreEqual(style.Outline, clone.Outline);
                Assert.AreEqual(style.OutlineColor, clone.OutlineColor);
                Assert.AreEqual(style.FontName, clone.FontName);
                Assert.AreEqual(style.FontSize, clone.FontSize);
                Assert.AreEqual(style.Italic, clone.Italic);
            }
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
