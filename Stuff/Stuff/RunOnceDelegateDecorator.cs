using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stuff
{
    public class RunOnceDelegateDecorator
    {
        private readonly AutoResetEvent _syncObj = new AutoResetEvent(true);
#pragma warning disable 169, 414
        private volatile bool _runBefore = false;
#pragma warning restore 169, 414

        /// <summary> Simple decoration of given delegate. Does not introduce any additional behavior </summary>
        /// <typeparam name="T"> Delegate type</typeparam>
        /// <param name="del"> Delegate to decorate </param>
        /// <returns> New delegate which is wrapper around given one </returns>
        public T DecorateDummy<T>(T del) where T : class
        {
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("Parameter must be of delegate type");

            var invokeMethodInfo = delegateType.GetMethod("Invoke");
            var parametersInfo = invokeMethodInfo.GetParameters();

            var parameters = parametersInfo.Select(pi => Expression.Parameter(pi.ParameterType)).ToArray();
            var delegateInstance = del as Delegate;
                
            var ce = Expression.Call(
                // ReSharper disable once PossibleNullReferenceException
                Expression.Constant(delegateInstance.Target),
                delegateInstance.Method,
                parameters);
            var lambda = Expression.Lambda(ce, parameters);
            return lambda.Compile() as T;
        }

        /// <summary> Decorate given delegate to be executed only once. Not thread safe! </summary>
        /// <typeparam name="T"> Delegate type</typeparam>
        /// <param name="del"> Delegate to decorate </param>
        /// <returns> New delegate which is wrapper around given one </returns>
        public T DecorateRunOnceUnsafe<T>(T del) where T : class
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
                // ReSharper disable once PossibleNullReferenceException
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
            return lambda.Compile() as T;
        }

        /// <summary> Decorate given delegate to be executed only once. Thread-safe. </summary>
        /// <typeparam name="T"> Delegate type</typeparam>
        /// <param name="del"> Delegate to decorate </param>
        /// <returns> New delegate which is wrapper around given one </returns>
        public T DecorateRunOnce<T>(T del) where T : class
        {
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("Parameter must be of delegate type");

            var invokeMethodInfo = delegateType.GetMethod("Invoke");
            var parametersInfo = invokeMethodInfo.GetParameters();
            var returnType = invokeMethodInfo.ReturnType;

            var parameters = parametersInfo.Select(pi => Expression.Parameter(pi.ParameterType)).ToArray();
            var delegateInstance = del as Delegate;

            var callOrignalDelegateExpression = Expression.Call(
                // ReSharper disable once PossibleNullReferenceException
                Expression.Constant(delegateInstance.Target),
                delegateInstance.Method,
                parameters);

            var defaultExpression = Expression.Default(returnType);
            var runBeforeFieldExpression = Expression.Field(Expression.Constant(this), "_runBefore");

            var conditionExpression = Expression.Condition(
                Expression.IsTrue(runBeforeFieldExpression),
                defaultExpression,
                Expression.Block(
                    Expression.Call(
                        Expression.Constant(_syncObj),
                        ((Func<bool>) _syncObj.WaitOne).Method),
                    Expression.TryFinally(
                        Expression.Condition(
                            Expression.IsTrue(runBeforeFieldExpression),
                            defaultExpression,
                            Expression.TryFinally(
                                callOrignalDelegateExpression,
                                Expression.Assign(runBeforeFieldExpression, Expression.Constant(true))),
                            returnType),
                        Expression.Call(
                            Expression.Constant(_syncObj),
                            ((Func<bool>) _syncObj.Set).Method))),
                returnType);

            var lambda = Expression.Lambda(conditionExpression, parameters);
            return lambda.Compile() as T;
        }
    }
}