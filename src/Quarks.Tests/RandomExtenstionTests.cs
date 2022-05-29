using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests
{
    [TestFixture]
    public class RandomExtenstionTests
    {
        [Test]
        public void RestoreStateTest()
        {
            const int n = 50;

            var original = new Random(123);

            for (var i = 0; i < 50; i++)
            {
                original.Next();
            }

            byte[] state = original.GetState();

            var expected = new int[n];
            for (var i = 0; i < expected.Length; i++)
            {
                expected[i] = original.Next();
            }

            var restored = new Random(0);
            restored.RestoreState(state);

            var actual = new int[n];
            for (var i = 0; i < actual.Length; i++)
            {
                actual[i] = restored.Next();
            }

            Assert.That(actual, Is.EquivalentTo(expected));
        }
#if NET6_0_OR_GREATER

        [TestCaseSource(nameof(CreateIncompatibleRandoms))]
        public void Should_throw_for_incompatible_implementations_on_get_state(Random random)
            => Assert.Throws<ArgumentException>(() => random.GetState());

        [TestCaseSource(nameof(CreateIncompatibleRandoms))]
        public void Should_throw_for_incompatible_implementations_on_restore_state(Random random)
            => Assert.Throws<ArgumentException>(() => random.RestoreState(Array.Empty<byte>()));

        public static IEnumerable<Random> CreateIncompatibleRandoms()
        {
            yield return new Random();
            yield return Random.Shared;
        }
#endif
    }
}