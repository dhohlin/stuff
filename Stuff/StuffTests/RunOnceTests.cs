using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
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
        public void TestWrapperWithSingleParameterlessFunc()
        {
            int i = 0;
            Func<bool> func = () =>
            {
                i++;
                return true;
            };

            var q = func.AsRunOnce();
            q();
            q();
            q();
            q();
            q();
            q();
            Assert.AreEqual(1, i);
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
    }
}
