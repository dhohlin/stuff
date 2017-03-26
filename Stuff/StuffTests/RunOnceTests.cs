using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Stuff;

namespace StuffTests
{
    [TestFixture]
    internal class RunOnceTests
    {
        [Test]
        public void TestUnsafe()
        {
            var i = 0;
            Func<int, bool> fun = q =>
            {
                i++;
                return true;
            };

            Assert.AreEqual(true, fun(3));
            Assert.AreEqual(1, i);
            Assert.AreEqual(true, fun(3));
            Assert.AreEqual(2, i);
            Assert.AreEqual(true, fun(3));
            Assert.AreEqual(3, i);

            i = 0;
            var decorated = new RunOnceDelegateDecorator().DecorateRunOnceUnsafe(fun);
            Assert.AreNotEqual(fun, decorated);

            Assert.AreEqual(true, decorated(3));
            Assert.AreEqual(1, i);
            Assert.AreEqual(false, decorated(3));
            Assert.AreEqual(1, i);
            Assert.AreEqual(false, decorated(3));
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_FiveParametersAction()
        {
            var i = 0;
            Action<int, bool, double, DateTime, object> act = (I, b, d, D, o) => { i++; };

            var runOnce = act.AsRunOnce();
            for (var j = 0; j < 10; j++)
                runOnce(33, false, 23d, DateTime.Now, new object());
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_FiveParametersFunc()
        {
            var i = 0;
            Func<int, bool, double, DateTime, object, bool> act = (I, b, d, D, o) =>
            {
                i++;
                return true;
            };

            var runOnce = act.AsRunOnce();
            Assert.AreEqual(true, runOnce(33, false, 2d, DateTime.Now, new object()));
            Assert.AreEqual(1, i);

            for (var j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j, false, 2d, DateTime.Now, new object()));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_ParameterlessAction()
        {
            var i = 0;
            Action act = () => { i++; };

            var runOnce = act.AsRunOnce();
            for (var j = 0; j < 10; j++)
                runOnce();
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_ParameterlessFunc()
        {
            var i = 0;
            Func<bool> fun = () =>
            {
                i++;
                return true;
            };

            var runOnce = fun.AsRunOnce();

            Assert.AreEqual(true, runOnce());
            Assert.AreEqual(1, i);

            for (var j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce());
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_SingleParameterAction()
        {
            var i = 0;
            Action<bool> act = b => { i++; };

            var runOnce = act.AsRunOnce();
            for (var j = 0; j < 10; j++)
                runOnce(false);
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_SingleParameterFunc()
        {
            var i = 0;
            Func<int, bool> fun = I =>
            {
                i++;
                return true;
            };

            var runOnce = fun.AsRunOnce();

            Assert.AreEqual(true, runOnce(33));
            Assert.AreEqual(1, i);

            for (var j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_TwoParametersAction()
        {
            var i = 0;
            Action<bool, double> act = (b, d) => { i++; };

            var runOnce = act.AsRunOnce();
            for (var j = 0; j < 10; j++)
                runOnce(false, 23d);
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_TwoParametersFunc()
        {
            var i = 0;
            Func<int, double, bool> act = (I, d) =>
            {
                i++;
                return true;
            };

            var runOnce = act.AsRunOnce();
            Assert.AreEqual(true, runOnce(33, 22d));
            Assert.AreEqual(1, i);

            for (var j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j, 73d));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDummyWrapping()
        {
            Func<int, bool> isEven = i => i % 2 == 0;

            Assert.AreEqual(true, isEven(2));
            Assert.AreEqual(false, isEven(3));

            var decoreated = new RunOnceDelegateDecorator().DecorateDummy(isEven);
            Assert.AreEqual(true, decoreated(2));
            Assert.AreEqual(false, decoreated(3));
        }

        [Test]
        public void TestMultiThread()
        {
            var i = 0;
            Action act = () => { i++; };

            var runOnce = act.AsRunOnce();

            var evt = new ManualResetEvent(false);

            var tasks = Enumerable.Range(0, 5000)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    evt.WaitOne();
                    runOnce();
                }))
                .ToArray();

            evt.Set();
            Task.WaitAll(tasks);
            Assert.AreEqual(1, i);
        }
    }
}