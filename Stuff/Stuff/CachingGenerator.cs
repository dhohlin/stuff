using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stuff
{
    /// <summary>
    ///     Caching decorator for IEnumerable generators
    /// </summary>
    public static class CachingGenerator
    {
        public static IEnumerable<T> Create<T>(IEnumerable<T> decoratee)
        {
            return new CachingGeneratorImpl<T>(decoratee);
        }

        public static State GetCachingGeneratorState<T>(IEnumerable<T> enumerable)
        {
            var cachingGenerator = enumerable as CachingGeneratorImpl<T>;
            return new State
            {
                IsCachingGenerator = cachingGenerator != null,
                EnumeratorCreated = cachingGenerator?.OrigEnumerator != null,
                CacheSize = cachingGenerator?.Cache.Keys.Count ?? 0
            };
        }

        public struct State
        {
            public bool IsCachingGenerator { get; internal set; }
            public bool EnumeratorCreated { get; internal set; }
            public int CacheSize { get; internal set; }
        }

        private class CachingGeneratorImpl<T> : IEnumerable<T>
        {
            internal readonly ConcurrentDictionary<int, T> Cache = new ConcurrentDictionary<int, T>();
            private readonly IEnumerable<T> _origGenerator;
            internal IEnumerator<T> OrigEnumerator;

            internal CachingGeneratorImpl(IEnumerable<T> decoratee)
            {
                _origGenerator = decoratee;
            }

            public IEnumerator<T> GetEnumerator() => new CachingEnumerator(this);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private bool TryMoveToIndex(int index)
            {
                if (Cache.ContainsKey(index))
                    return true;

                if (OrigEnumerator == null)
                    OrigEnumerator = _origGenerator.GetEnumerator();
                if (!OrigEnumerator.MoveNext())
                    return false;

                if (!Cache.TryAdd(index, OrigEnumerator.Current))
                    throw new InvalidOperationException("Unable to add item to cache");

                return true;
            }

            private T GetValueAtIndex(int index)
            {
                T result;
                if (!Cache.TryGetValue(index, out result))
                    throw new InvalidOperationException("Cache slot is empty");
                return result;
            }

            private class CachingEnumerator : IEnumerator<T>
            {
                private readonly CachingGeneratorImpl<T> _parent;
                private int _index;

                internal CachingEnumerator(CachingGeneratorImpl<T> parent)
                {
                    _parent = parent;
                    _index = -1;
                }

                public T Current => _parent.GetValueAtIndex(_index);

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    _index++;
                    return _parent.TryMoveToIndex(_index);
                }

                public void Reset()
                {
                    _index = -1;
                }
            }
        }
    }
}