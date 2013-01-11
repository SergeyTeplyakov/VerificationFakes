using System;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace FakeMockTests.Expected
{
    [TestFixture]
    public class AnyArgumentValueTests
    {
        [TestCase(null)]
        [TestCase(42)]
        [TestCase(double.NaN)]
        [TestCase("")]
        public void Test_Match_Always_Returns_True(object value)
        {
            // Arrange 
            var expected = new AnyArgumentValue();
            
            // Act & Assert
            Assert.That(expected.Match(value), 
                "AnyArgumentValue should match with any argument.");
        }

        [Test]
        public void Test_Two_Objects_Are_Equals()
        {
            // Arrange
            var lhs = new AnyArgumentValue();
            var rhs = new AnyArgumentValue();

            // Act & Assert
            Assert.That(lhs.Equals(rhs), "Any two instances should be equal.");
        }
    }
}