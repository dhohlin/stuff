using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Stuff.Tests
{
    [TestFixture]
    public class CachingGeneratorTests
    {
        private IEnumerable<Guid> getGuidsGenerator()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return Guid.NewGuid();
            }
            yield break;
        }

        [Test]
        public void SameValuesAfterMultipleEnumerations()
        {
            var generator = getGuidsGenerator();
            var cachingGenerator = CachingGenerator.Create(generator);

            var firstEnumeration = cachingGenerator.ToArray();
            var secongEnumeration = cachingGenerator.ToArray();
            CollectionAssert.AreEqual(firstEnumeration, secongEnumeration);
        }

        [Test]
        public void NewEnumeratorAtEachRequest()
        {
            var cachingGenerator = CachingGenerator.Create(new List<string>() { "qwe", "wer", "ert" });
            var enumerator1 = cachingGenerator.GetEnumerator();
            var enumerator2 = cachingGenerator.GetEnumerator();
            Assert.AreNotSame(enumerator1, enumerator2);
        }

        [Test]
        public void SimultaneousIteration()
        {
            var generator = getGuidsGenerator();
            var cachingGenerator = CachingGenerator.Create(generator);

            var firstEnumerator = cachingGenerator.GetEnumerator();
            var secondEnumerator = cachingGenerator.GetEnumerator();

            List<Guid> lst1 = new List<Guid>();
            List<Guid> lst2 = new List<Guid>();

            firstEnumerator.MoveNext();
            lst1.Add(firstEnumerator.Current);
            firstEnumerator.MoveNext();
            lst1.Add(firstEnumerator.Current);

            secondEnumerator.MoveNext();
            lst2.Add(secondEnumerator.Current);

            firstEnumerator.MoveNext();
            lst1.Add(firstEnumerator.Current);

            secondEnumerator.MoveNext();
            lst2.Add(secondEnumerator.Current);
            secondEnumerator.MoveNext();
            lst2.Add(secondEnumerator.Current);

            CollectionAssert.AreEqual(lst1, lst2);
        }
    }
}
