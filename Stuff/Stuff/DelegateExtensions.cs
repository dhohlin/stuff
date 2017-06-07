using System;
using System.Diagnostics.CodeAnalysis;

namespace Stuff
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class DelegateExtensions
    {
        public static Action AsRunOnce(this Action act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1> AsRunOnce<T1>(this Action<T1> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2> AsRunOnce<T1, T2>(this Action<T1, T2> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3> AsRunOnce<T1, T2, T3>(this Action<T1, T2, T3> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3, T4> AsRunOnce<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3, T4, T5> AsRunOnce<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3, T4, T5, T6> AsRunOnce<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3, T4, T5, T6, T7> AsRunOnce<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> AsRunOnce<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> act)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(act);

        public static Func<TResult> AsRunOnce<TResult>(this Func<TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, TResult> AsRunOnce<T1, TResult>(this Func<T1, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, TResult> AsRunOnce<T1, T2, TResult>(this Func<T1, T2, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, TResult> AsRunOnce<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, T4, TResult> AsRunOnce<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, T4, T5, TResult> AsRunOnce<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, T4, T5, T6, TResult> AsRunOnce<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> AsRunOnce<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> AsRunOnce<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func)
            => (new RunOnceDelegateDecorator()).DecorateRunOnce(func);
    }
}