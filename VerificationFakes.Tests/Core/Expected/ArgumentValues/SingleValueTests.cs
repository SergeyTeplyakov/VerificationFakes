using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace VerificationFakesTests.Expected
{
    [TestFixture]
    public class SingleValueTests
    {
        [TestCase("", "", Result = true)]
        [TestCase("1", "", Result = false)]
        [TestCase(42, 42, Result = true)]
        public bool Test_Match(object expected, object actual)
        {
            // Arrange
            var expectedArgument = new SingleArgumentValue(expected);

            // Act & Assert
            return expectedArgument.Match(actual);
        }
        
        [TestCase("", "")]
        [TestCase("1", "")]
        [TestCase(42, 42)]
        public void Test_Two_Instances_Equals_If_Values_Are_Equal(object o1, object o2)
        {
            // Arrange
            var a1 = new SingleArgumentValue(o1);
            var a2 = new SingleArgumentValue(o2);

            // Act & Assert
            Assert.That(a1.Equals(a2), Is.EqualTo(o1.Equals(o2)),
                "Two SingleArgumentValue's should be equal if two objects are equal");
        }
    }
}