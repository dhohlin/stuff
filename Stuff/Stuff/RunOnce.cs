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
                return;
            lock (_locker)
            {
                if (_runBefore)
                    return;

                action();
                _runBefore = true;
            }
        }
    }

    /// <summary>
    /// Simple thread safe wrapper for Action which should 
    /// </summary>
    public class RunOnceWrapper
    {
        private volatile bool _runBefore = false;
        private readonly object _locker = new object();
        private Action _action;

        public RunOnceWrapper(Action action)
        {
            _action = action;
        }

        public Action GetAction => run;

        private void run()
        {
            if (_runBefore)
                return;
            lock (_locker)
            {
                if (_runBefore)
                    return;

                _action();
                _runBefore = true;
            }
        }
    }

    /// <summary>
    /// Simple thread safe wrapper for Action which should 
    /// </summary>
    public class RunOnceFuncWrapper<T>
    {
        private volatile bool _runBefore = false;
        private readonly object _locker = new object();
        private readonly Func<T> _fun;

        public RunOnceFuncWrapper(Func<T> fun)
        {
            _fun = () =>
            {
                if (_runBefore)
                    return default(T);
                lock (_locker)
                {
                    if (_runBefore)
                        return default(T);

                    try
                    {
                        return fun();
                    }
                    finally
                    {
                        _runBefore = true;
                    }
                }
            };
        }

        public Func<T> GetFunc => _fun;
    }

    public class RunOnceHelper
    {
        private volatile bool _runBefore = false;
        private readonly object _locker = new object();

        public Func<T> WrapFunc<T>(Func<T> func)
        {
            return () =>
            {
                if (_runBefore)
                    return default(T);
                lock (_locker)
                {
                    if (_runBefore)
                        return default(T);

                    try
                    {
                        return func();
                    }
                    finally
                    {
                        _runBefore = true;
                    }
                }
            };
        }
    }

    public static class Ext
    {
        public static Func<T> AsRunOnce<T>(this Func<T> func)
        {
            var t = func.GetType();

            var helper = new RunOnceHelper();
            return helper.WrapFunc(func);            
        }
    }
}
