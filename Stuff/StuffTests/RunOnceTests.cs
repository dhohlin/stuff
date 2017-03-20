using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stuff;

namespace StuffTests
{
    [TestFixture]
    class RunOnceTests
    {
        [Test]
        public void SimpleRun()
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
    }
}
