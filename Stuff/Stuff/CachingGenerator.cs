using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stuff
{
    /// <summary>
    /// Caching decorator for IEnumerable generators 
    /// </summary>
    public static class CachingGenerator
    {
        public static IEnumerable<T> Create<T>(IEnumerable<T> decoratee)
        {
            return new CachingGeneratorImpl<T>(decoratee);
        }

        private class CachingGeneratorImpl<T> : IEnumerable<T>
        {
            private IEnumerable<T> origGenerator;
            private IEnumerator<T> origEnumerator;

            private readonly ConcurrentDictionary<int, T> cache = new ConcurrentDictionary<int, T>();

            internal CachingGeneratorImpl(IEnumerable<T> decoratee)
            {
                origGenerator = decoratee;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new CachingEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private bool tryMoveToIndex(int index)
            {
                if (cache.ContainsKey(index))
                    return true;

                if (origEnumerator == null)
                    origEnumerator = origGenerator.GetEnumerator();
                if (!origEnumerator.MoveNext())
                    return false;

                if (!cache.TryAdd(index, origEnumerator.Current))
                    throw new InvalidOperationException("Unable to add item to cache");

                return true;
            }

            private T getValueAtIndex(int index)
            {
                T result;
                if (!cache.TryGetValue(index, out result))
                    throw new InvalidOperationException("Cache slot is empty");
                return result;
            }

            internal bool check()
            {
                return true;
            }

            private class CachingEnumerator : IEnumerator<T>
            {
                private readonly CachingGeneratorImpl<T> parent;
                private int index;

                internal CachingEnumerator(CachingGeneratorImpl<T> parent)
                {
                    this.parent = parent;
                    index = -1;
                }

                public T Current
                {
                    get
                    {
                        return parent.getValueAtIndex(index);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                public void Dispose() { }

                public bool MoveNext()
                {
                    index++;
                    return parent.tryMoveToIndex(index);
                }

                public void Reset()
                {
                    index = -1;
                }
            }
        }
    }

}