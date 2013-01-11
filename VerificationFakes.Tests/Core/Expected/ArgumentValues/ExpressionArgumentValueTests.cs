using System;
using System.Linq.Expressions;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;
using VerificationFakes.Samples;

namespace VerificationFakesTests.Expected
{
    [TestFixture]
    public class ExpressionArgumentValueTests
    {
        [Test]
        public void Two_Instances_Are_Equals_If_Expressions_Are_Equals()
        {
            // Arrange
            var lhs = new ExpressionArgumentValue<ILogWriter>(
                lw => lw.GetHashCode() != 0);
            var rhs = new ExpressionArgumentValue<ILogWriter>(
                lw => lw.GetHashCode() != 0);

            // Act & Assert
            Assert.That(lhs, Is.EqualTo(rhs),
                "Two ExpressionArgumentValue should be equal for the same expressions.");
        }

        [Test]
        public void Predicate_Matches_If_Func_Match()
        {
            // Arrange
            var predicate = (Expression<Func<int, bool>>)(n => n%2 == 0);
            var arg = new ExpressionArgumentValue<int>(predicate);

            // Act & Assert
            Assert.That(arg.Match(42), Is.EqualTo(predicate.Compile()(42)),
                "ExpressionArgumentValue should use specified predicate.");
        }
    }
}