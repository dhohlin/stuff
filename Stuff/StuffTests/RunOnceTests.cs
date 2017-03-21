using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public void ExamineGenericFunctions()
        {
            var func2Type = typeof(Func<int, string, decimal, double, bool>);

            
        }
    }
}
