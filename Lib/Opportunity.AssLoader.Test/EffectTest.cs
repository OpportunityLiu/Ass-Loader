using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class EffectTest : TestBase
    {
        public class E : Effect
        {
            public override string Name => "TestName";
        }

        public class E2 : Effect
        {
            public override string Name => "TestName";
        }

        public class WN1 : Effect
        {
            public override string Name => "  ss  ";
        }

        public class WN2 : Effect
        {
            public override string Name => "1221;3";
        }

        public class WN3 : Effect
        {
            public override string Name => "1221,3";
        }

        [TestMethod]
        public void Register()
        {
            CollectionAssert.Contains(Effect.RegisteredNames.ToList(), "Scroll Up");
            var e = new E();
            CollectionAssert.Contains(Effect.RegisteredNames.ToList(), "TestName");
            Effect.Register<E>();
            Effect.Register<E>();
            new UnknownEffect("as");
            CollectionAssert.DoesNotContain(Effect.RegisteredNames.ToList(), "as");

            Assert.ThrowsException<ArgumentException>(() => new UnknownEffect("a;s"));
            Assert.ThrowsException<ArgumentException>(() => new UnknownEffect("  a,s"));
            Assert.ThrowsException<ArgumentException>(() => new UnknownEffect(" "));
            Assert.ThrowsException<ArgumentException>(() => new UnknownEffect(""));
            Assert.ThrowsException<InvalidOperationException>(() => Effect.Register<E2>());
            Assert.ThrowsException<InvalidOperationException>(() => new E2());
            Assert.ThrowsException<InvalidOperationException>(() => Effect.Register<WN1>());
            Assert.ThrowsException<InvalidOperationException>(() => new WN1());
            Assert.ThrowsException<InvalidOperationException>(() => Effect.Register<WN2>());
            Assert.ThrowsException<InvalidOperationException>(() => Effect.Register<WN3>());
        }

        public class ME : Effect
        {
            public override string Name => "Custom Effect";

            [EffectField(0)]
            public double F1;
            [EffectField(1)]
            public int F2;
            [EffectField(2)]
            public string F3 { get; set; }

            public string NotAField;
            public string NotAField2 { get; set; }
        }

        [TestMethod]
        public void Parse()
        {
            var e1 = (UnknownEffect)Effect.Parse(" a a a ; dad, as ; paa ", TestSerializeInfo.Empty);
            Assert.AreEqual("a a a", e1.Name);
            CollectionAssert.AreEqual(new[] { "dad, as", "paa" }, e1.Arguments.ToList());

            var e2 = (UnknownEffect)Effect.Parse("Custom Effect", TestSerializeInfo.Empty);
            Assert.AreEqual("Custom Effect", e2.Name);
            CollectionAssert.That.IsEmpty(e2.Arguments);

            Effect.Register<ME>();

            Assert.IsInstanceOfType(Effect.Parse("Custom Effect", TestSerializeInfo.Empty), typeof(ME));

            var e4 = (ME)Effect.Parse("Custom Effect; 12.7 ; 1223; asdf ;af", TestSerializeInfo.Empty);
            Assert.AreEqual(12.7, e4.F1);
            Assert.AreEqual(1223, e4.F2);
            Assert.AreEqual("asdf", e4.F3);
            Assert.IsNull(e4.NotAField);
            Assert.IsNull(e4.NotAField2);

            Assert.ThrowsException<FormatException>(() => Effect.Parse("Custom Effect; 12.7 ; 1223; asdf ;af", TestSerializeInfo.Throw));
            Assert.ThrowsException<FormatException>(() => Effect.Parse("aaa", TestSerializeInfo.Throw));

            var e5 = (ScrollUpEffect)Effect.Parse("Scroll up; 1; 2", TestSerializeInfo.Throw);
            Assert.AreEqual(1, e5.Y1);
            Assert.AreEqual(2, e5.Y2);
            Assert.AreEqual(0, e5.Delay);
            Assert.AreEqual(0, e5.FadeAwayMargin);
        }
    }
}
