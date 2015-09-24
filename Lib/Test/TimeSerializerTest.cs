using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class TimeSerializerTest
    {
        [TestMethod]
        public void TimeSerializer()
        {
            var t = new AssLoader.Serializer.TimeSerializeAttribute();
            Action<long> test = value =>
            {
                var time = new TimeSpan(value * 100000L);
                var str = t.Serialize(time);
                var des = t.Deserialize(str);
                Assert.AreEqual(time, des);
            };
            for(int i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadByte());
            for(int i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadUInt16());
            for(int i = 0; i < 0xffff; i++)
                test(TestHelper.RandomReader.ReadUInt32());
        }
    }
}
