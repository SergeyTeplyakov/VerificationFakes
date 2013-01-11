using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;
using VerificationFakesTests.FluentTestCaseBuilder;

namespace VerificationFakesTests
{
    [TestFixture]
    internal class MockFuncsParametrizedTests
    {
        [TestCaseSource("GetVerifyWithOneFuncTestCases")]
        public void Test_Verify_With_One_Func(Expression<Func<ILogWriter, int>> action,
            Expression<Func<ILogWriter, int>> verificationExpression, Times times)
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub);

            // Act
            action.Compile()(stub);

            // Assert
            mock.Verify(verificationExpression, times);
        }
        
        [TestCaseSource("GetVerifyWithOneFuncTestCases")]
        public void Test_VerifyAll_With_One_Func(Expression<Func<ILogWriter, int>> action,
            Expression<Func<ILogWriter, int>> verificationExpression, Times times)
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub);

            mock.Setup(verificationExpression, times);

            // Act
            action.Compile()(stub);

            // Assert
            mock.VerifyAll();
        }

        public static IEnumerable<TestCaseData> GetVerifyWithOneFuncTestCases()
        {
            var builder = new Builder<ILogWriter>();
            // Test cases for ILogWriter.WriteR(int)
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(42));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(42));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(It.IsAny<int>()));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(It.IsInRange(41, 43)));

            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(It.IsInRange(44, 46)),
                                                        Times.Never());

            // Failure cases
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(21))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(It.IsInRange(43, 44)))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(42), Times.Never())
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(42), Times.AtLeast(2))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(42), Times.Exactly(2))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.ToString().Length)
                .Throws(typeof(InvalidOperationException));

            // Test cases for ILogWriter.WriteR(string)
            yield return builder.Do(lw => lw.WriteR("foo")).Expects(lw => lw.WriteR("foo"));
            yield return builder.Do(lw => lw.WriteR("foo")).Expects(lw => lw.WriteR(It.IsAny<string>()));

            // Test cases for ILogWriter.WriteR(int, string)
            yield return builder.Do(lw => lw.WriteR(42, "Foo")).Expects(lw => lw.WriteR(42, "Foo"));
            yield return builder.Do(lw => lw.WriteR(42, "Foo"))
                .Expects(lw => lw.WriteR(It.IsAny<int>(), It.IsAny<string>()));
            yield return builder.Do(lw => lw.WriteR(42, "Foo"))
                .Expects(lw => lw.WriteR(It.IsInRange(1, 43), It.IsAny<string>()));

            // Tests cases with predicate-base verification
            yield return builder.Do(lw => lw.WriteR(42)).Expects(lw => lw.WriteR(It.Is<int>(n => n == 42)));
            yield return builder.Do(lw => lw.WriteR("Foooo"))
                .Expects(lw => lw.WriteR(It.Is<string>(s => s.ToLower().StartsWith("foo"))));
        }

    }
}