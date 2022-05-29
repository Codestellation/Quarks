using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.Testing;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Testing
{
    [TestFixture]
    public class SomeCollectionTests : SomeTests
    {
        private IList<int> _list;
        private IList<int> _nullList;
        private IList<int> _emptyList;

        private Func<int> _elementOf;
        private Func<int> _indexOf;

        [SetUp]
        public void Setup()
        {
            const int size = 10;
            _list = Enumerable
                .Range(0, size)
                .ConvertToArray<int, int, IEnumerable<int>>(i => i, size);

            _nullList = null;
            _emptyList = Array.Empty<int>();

            _elementOf = () => Some.ElementOf(_list);
            _indexOf = () => Some.IndexOf(_list);
        }

        [Test]
        public void ElementOf_should_generate_different_values()
        {
            Should_generate_different_values(_elementOf);
        }

        [Test, Explicit]
        public void ElementOf_should_generate_uniform_distribution()
        {
            Should_generate_uniform_distribution(_elementOf);
        }

        [Test]
        public void ElementOf_should_throw_if_list_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Some.ElementOf(_nullList));
        }

        [Test]
        public void ElementOf_should_throw_if_list_is_empty()
        {
            Assert.Throws<ArgumentException>(() => Some.ElementOf(_emptyList));
        }

        [Test]
        public void IndexOf_should_generate_different_values()
        {
            Should_generate_different_values(_indexOf);
        }

        [Test, Explicit]
        public void IndexOf_should_generate_uniform_distribution()
        {
            Should_generate_uniform_distribution(_indexOf);
        }

        [Test]
        public void IndexOf_should_throw_if_list_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Some.IndexOf(_nullList));
        }

        [Test]
        public void IndexOf_should_throw_if_list_is_empty()
        {
            Assert.Throws<ArgumentException>(() => Some.IndexOf(_emptyList));
        }
    }
}