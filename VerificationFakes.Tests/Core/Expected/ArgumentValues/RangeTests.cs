using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace FakeMockTests
{
    [TestFixture]
    public class RangeTests
    {
        [TestCase("(0,0)", Result = false)]
        public bool Test_Range_Validity(string range)
        {
            Range dummy;
            return Range.TryParse(range, out dummy);
        }
    }
}