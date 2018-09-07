using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader.Serializer;
using System;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class TimeSerializerTest
    {
        [TestMethod]
        public void TimeSerializer()
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
                var str = t.Serialize(time);
                var des = t.Deserialize(str);
                Assert.AreEqual(time, des);
            }
        }
    }
}
