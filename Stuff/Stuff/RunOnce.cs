using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuff
{
    /// <summary>
    /// Simple thread safe wrapper for Action which should 
    /// </summary>
    public class RunOnce
    {
        private volatile bool _runBefore = false;
        private readonly object _locker = new object();        

        public void Run(Action action)
        {
            if (_runBefore)
                return ;
            lock (_locker)
            {
                if (_runBefore)
                    return;

                action();
                _runBefore = true;
            }
        }
    }    
}
