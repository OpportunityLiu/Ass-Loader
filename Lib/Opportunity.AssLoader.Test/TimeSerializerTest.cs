using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader.Serializer;
using System;
using System.IO;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class TimeSerializerTest : TestBase
    {
        [TestMethod]
        public void Random()
        {
            var t = new TimeSerializeAttribute();
            for (var i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadByte());
            for (var i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadUInt16());
            for (var i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadUInt32());

            void test(long value)
            {
                var time = new TimeSpan(value * 100000L);
                var w = new StringWriter();
                t.Serialize(w, time, TestSerializeInfo.Throw);
                var des = (TimeSpan)t.Deserialize(w.ToString(), TestSerializeInfo.Throw);
                Assert.AreEqual(time, des);
            }
        }

        //  [DataRow(" 1223 : 1 : 300 ", 44031600000000)]
        [DataRow(" 1223 : 1 : 300. 1 ", 44031601000000)]
        //[DataRow(" 1223 : 1 : 300. 1000000000 ", 44031601000000)]
        //[DataRow(" 1223 : 1 : 300. 010 ", 44031600100000)]
        [DataTestMethod]
        public void Speical(string str, long ticks)
        {

            var t = new TimeSerializeAttribute();
            var time = new TimeSpan(ticks);
            var w = new StringWriter();
            t.Serialize(w, time, TestSerializeInfo.Throw);
            var des = (TimeSpan)t.Deserialize(str, TestSerializeInfo.Throw);
            Assert.AreEqual(time, des);
        }
    }
}
