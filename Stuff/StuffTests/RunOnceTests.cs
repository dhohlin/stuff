using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stuff;

namespace StuffTests
{
    [TestFixture]
    class RunOnceTests
    {
        [Test]
        public void TestRunonceInstance()
        {
            var runOnce = new RunOnce();

            int i = 0;
            Action action = () => { i++; };

            runOnce.Run(action);
            runOnce.Run(action);
            runOnce.Run(action);
            runOnce.Run(action);
            runOnce.Run(action);
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestWrapperWithSimpleAction()
        {
            int i = 0;
            Action action = () => { i++; };

            var runOnce = new RunOnceWrapper(action);
            var act = runOnce.GetAction;

            act();
            act();
            act();
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_ParameterlessAction()
        {
            int i = 0;
            Action act = () =>
            {
                i++;
            };

            var runOnce = act.AsRunOnce();
            for (int j = 0; j < 10; j++)
            {
                runOnce();
            }
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_SingleParameterAction()
        {
            int i = 0;
            Action<bool> act = b =>
            {
                i++;
            };

            var runOnce = act.AsRunOnce();
            for (int j = 0; j < 10; j++)
            {
                runOnce(false);
            }
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_TwoParametersAction()
        {
            int i = 0;
            Action<bool, double> act = (b, d) =>
            {
                i++;
            };

            var runOnce = act.AsRunOnce();
            for (int j = 0; j < 10; j++)
            {
                runOnce(false, 23d);
            }
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_FiveParametersAction()
        {
            int i = 0;
            Action<int, bool, double, DateTime, object> act = (I, b, d, D, o) =>
            {
                i++;
            };

            var runOnce = act.AsRunOnce();
            for (int j = 0; j < 10; j++)
            {
                runOnce(33, false, 23d, DateTime.Now, new object());
            }
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestDecoration_ParameterlessFunc()
        {
            int i = 0;
            Func<bool> fun = () =>
            {
                i++;
                return true;
            };

            var runOnce = fun.AsRunOnce();

            Assert.AreEqual(true, runOnce());
            Assert.AreEqual(1, i);

            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce());
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_SingleParameterFunc()
        {
            int i = 0;
            Func<int, bool> fun = I =>
            {
                i++;
                return true;
            };

            var runOnce = fun.AsRunOnce();

            Assert.AreEqual(true, runOnce(33));
            Assert.AreEqual(1, i);

            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_TwoParametersFunc()
        {
            int i = 0;
            Func<int, double, bool> act = (I, d) =>
            {
                i++;
                return true;
            };

            var runOnce = act.AsRunOnce();
            Assert.AreEqual(true, runOnce(33, 22d));
            Assert.AreEqual(1, i);

            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j, 73d));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDecoration_FiveParametersFunc()
        {
            int i = 0;
            Func<int, bool, double, DateTime, object, bool> act = (I, b, d, D, o) =>
            {
                i++;
                return true;
            };

            var runOnce = act.AsRunOnce();
            Assert.AreEqual(true, runOnce(33, false, 2d, DateTime.Now, new object()));
            Assert.AreEqual(1, i);

            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(false, runOnce(j, false, 2d, DateTime.Now, new object()));
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void TestDummyWrapping()
        {
            Func<int, bool> isEven = i => i % 2 == 0;

            Assert.AreEqual(true, isEven(2));
            Assert.AreEqual(false, isEven(3));

            var decoreated = (new RunOnceHelper()).WrapDummy(isEven);
            Assert.AreEqual(true, decoreated(2));
            Assert.AreEqual(false, decoreated(3));
        }

        [Test]
        public void Test()
        {
            int i = 0;
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
            var decorated = (new RunOnceHelper()).WrapRunOnceUnsafe(fun);
            Assert.AreEqual(true, decorated(3));
            Assert.AreEqual(1, i);
            Assert.AreEqual(false, decorated(3));
            Assert.AreEqual(1, i);
            Assert.AreEqual(false, decorated(3));
            Assert.AreEqual(1, i);
        }

        [Test]
        public void TestMultiThread()
        {
            var i = 0;
            Action act = () => { i++; };

            var runOnce = act.AsRunOnce();

            var evt = new ManualResetEvent(false);

            for (int j = 0; j < 500; j++)
            {
                Task.Factory.StartNew(() =>
                {
                    evt.WaitOne();
                    runOnce();
                });
            }
            Assert.AreEqual(1, i);
        }
    }
}
