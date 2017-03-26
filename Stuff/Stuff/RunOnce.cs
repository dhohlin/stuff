using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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

        private readonly ManualResetEvent _syncObj = new ManualResetEvent(true);

        public Func<T> WrapFunc<T>(Func<T> func)
        {
            return () =>
            {
                if (_runBefore)
                    return default(T);

                _syncObj.WaitOne();
                try
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
                finally
                {
                    _syncObj.Set();
                }
            };
        }

        public T WrapDummy<T>(T del) where T : class
        {
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("Parameter must be of delegate type");

            var invokeMethodInfo = delegateType.GetMethod("Invoke");
            var parametersInfo = invokeMethodInfo.GetParameters();

            var parameters = parametersInfo.Select(pi => Expression.Parameter(pi.ParameterType)).ToArray();
            var delegateInstance = del as Delegate;

            var ce = Expression.Call(
                Expression.Constant(delegateInstance.Target),
                delegateInstance.Method,
                parameters);
            var lambda = Expression.Lambda(ce, parameters);
            var d = lambda.Compile() as T;
            return d;
        }

        public T WrapRunOnceUnsafe<T>(T del) where T : class
        {
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("Parameter must be of delegate type");

            var invokeMethodInfo = delegateType.GetMethod("Invoke");
            var parametersInfo = invokeMethodInfo.GetParameters();
            var returnType = invokeMethodInfo.ReturnType;

            var parameters = parametersInfo.Select(pi => Expression.Parameter(pi.ParameterType)).ToArray();
            var delegateInstance = del as Delegate;

            var ce = Expression.Call(
                Expression.Constant(delegateInstance.Target),
                delegateInstance.Method,
                parameters);

            var defaultExpression = Expression.Default(returnType);
            var runBeforeFieldExpression = Expression.Field(Expression.Constant(this), "_runBefore");
            var conditionExpression = Expression.Condition(
                Expression.IsTrue(runBeforeFieldExpression),
                defaultExpression,
                Expression.TryFinally(
                    ce,
                    Expression.Assign(runBeforeFieldExpression, Expression.Constant(true))),
                returnType);

            var lambda = Expression.Lambda(conditionExpression, parameters);
            var q = lambda.Compile();
            var d = q as T;
            return d;
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

        public static Action AsRunOnce(this Action action) => (new RunOnceHelper()).WrapDummy(action);
        public static Action<T> AsRunOnce<T>(this Action<T> action) => (new RunOnceHelper()).WrapDummy(action);
        public static Action<T1, T2> AsRunOnce<T1, T2>(this Action<T1, T2> action) => (new RunOnceHelper()).WrapDummy(action);
    }
}
