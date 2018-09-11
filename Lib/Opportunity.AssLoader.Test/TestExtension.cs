using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    internal static class TestExtension
    {
        public static void IsEmpty<T>(this CollectionAssert t, IEnumerable<T> enumerable)
        {
            Assert.IsNotNull(enumerable);
            var c = enumerable.Count();
            if (c != 0)
                throw new AssertFailedException($"The collection contains {c} items, is not empty.");
        }
        public static void IsNotEmpty<T>(this CollectionAssert t, IEnumerable<T> enumerable)
        {
            Assert.IsNotNull(enumerable);
            var c = enumerable.Count();
            if (c == 0)
                throw new AssertFailedException($"The collection is empty.");
        }
    }
}
