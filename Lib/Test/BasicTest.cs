using System;
using System.IO;
using AssLoader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using AssLoader.Collections;

namespace Test
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
            get
            {
                return helper.Context;
            }
            set
            {
                helper = new TestHelper(value);
            }
        }

        [TestMethod]
        public void FormatHash()
        {
            var s=new string[]
            {
                "Name,Fontsize,Fontname,PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding",
                "Name, Fontsize,  Fontname,PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding",
                "Name, Fontsize  ,  Fontname,PrimaryColour\t, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding",
                "Name, Fontsize  ,  Fontname, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding, PrimaryColour\t\r\n",
                "Name, Fontsize  ,  Fontname,PrimaryColour\t, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, \nEncoding"
            };
            var f = new List<EntryHeader>();
            foreach(var item in s)
            {
                f.Add(new EntryHeader(item));
                f.Add(new EntryHeader(item.ToUpper()));
                f.Add(new EntryHeader(item.ToLower()));
            }
            var q = (from format in f
                     select format.GetHashCode()).Distinct();
            Assert.AreEqual(q.Count(), 1);
            var e = f.Distinct();
            Assert.AreEqual(e.Count(), 1);
        }

        [TestMethod]
        public void LoadAndSave()
        {
            Subtitle<AssScriptInfo> t;
            foreach(var item in helper.LoadTestFiles())
            {
                using(var r = item.Value)
                    t = Subtitle.Parse<AssScriptInfo>(r);
                using(var savefile = helper.SaveResult(item.Key))
                    t.Serialize(savefile);
            }
        }

        [TestMethod]
        public void LoadAndSaveParallel()
        {
            helper.LoadTestFiles().AsParallel().Select(item =>
            {
                using(var r = item.Value)
                    return new
                    {
                        Key = item.Key,
                        Value = Subtitle.Parse<AssScriptInfo>(r)
                    };
            }).ForAll(item =>
            {
                using(var savefile = helper.SaveResult(item.Key + "Parallel"))
                    item.Value.Serialize(savefile);
            });
        }

        [TestMethod]
        public void CommentAll()
        {
            foreach(var file in helper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value);
                foreach(var item in t.EventCollection)
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
            helper.LoadTestFiles().AsParallel().Select(item =>
            {
                using(var r = item.Value)
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
            foreach(var file in helper.LoadTestFiles())
            {
                var t = Subtitle.Parse<AssScriptInfo>(file.Value);
                foreach(var item in t.EventCollection)
                    item.IsComment = false;
                var x = Subtitle.Parse<AssScriptInfo>(t.Serialize());
                var c = from ev in x.EventCollection
                        select ev.IsComment;
                CollectionAssert.DoesNotContain(c.ToList(), true);
            }
        }

        [TestMethod]
        public void Parse()
        {
            var t = Subtitle.Parse<AssScriptInfo>("");
        }
    }
}
