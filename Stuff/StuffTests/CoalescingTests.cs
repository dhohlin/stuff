using System;
using NUnit.Framework;
using Stuff;

namespace StuffTests
{
    [TestFixture]
    internal class CoalescingTests
    {
        [Test]
        public void TestCoalescingGuid() => testCoalescing(Guid.Empty, Guid.NewGuid());

        [Test]
        public void TestCoalescingDateTime() => testCoalescing(default(DateTime), DateTime.Now);

        [Test]
        public void TestCoalescingBool() => testCoalescing(false, true);

        [Test]
        public void TestCoalescingInt() => testCoalescing(0, 133);

        [Test]
        public void TestCoalescingDouble() => testCoalescing(0d, 123.45);

        [Test]
        public void TestCoalescingString() => testCoalescing(null, "Just a string");

        [Test]
        public void TestCoalescingCustomStruct()
            => testCoalescing(new CustomStruct(), new CustomStruct() { I = 333, B = true });

        [Test]
        public void TestCoalescingCustomClass()
            => testCoalescing(null, new CustomClass() { I = 333, B = true });


        private void testCoalescing<T>(T emptyValue, T nonEmptyValue)
        {
            Assert.AreEqual(emptyValue, emptyValue.CoalesceWith(emptyValue));
            Assert.AreEqual(nonEmptyValue, emptyValue.CoalesceWith(nonEmptyValue));
            Assert.AreEqual(nonEmptyValue, emptyValue.CoalesceWith(emptyValue).CoalesceWith(emptyValue).CoalesceWith(emptyValue).CoalesceWith(nonEmptyValue));
        }

        private struct CustomStruct
        {
            public int I;
            public bool B;
        }

        private class CustomClass
        {
            public int I;
            public bool B;
        }
    }
}
