using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace FakeMockTests
{
    [TestFixture]
    public class RangeArgumentValueTests
    {
        [TestCase(1, 2, 1, Result = true)]
        [TestCase(1, 2, 3, Result = false)]
        [TestCase(1, 2, 0, Result = false)]
        [TestCase(1, 2, int.MinValue, Result = false)]
        [TestCase(int.MinValue, int.MaxValue, int.MaxValue, Result = true)]
        [TestCase(int.MinValue, int.MaxValue, int.MinValue, Result = true)]

        [TestCase(1.0, 2.0, 1.0, Result = true)]

        // TODO: add string & DateTime comparisons
        public bool Test_Match(object left, object right, object value)
        {
            var comparer = new RangeArgumentValue(left, right);
            return comparer.Match(value);
        }
    }
}