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
    internal class MockActionsParametrizedTests
    {
        [TestCaseSource("GetVerifyWithOneActionTestCases")]
        public void Test_Verify_With_One_Action(Expression<Action<ILogWriter>> action,
            Expression<Action<ILogWriter>> verificationExpression, Times times)
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub);

            // Act
            action.Compile()(stub);

            // Assert
            mock.Verify(verificationExpression, times);
        }
        
        [TestCaseSource("GetVerifyWithOneActionTestCases")]
        public void Test_VerifyAll_With_One_Action(Expression<Action<ILogWriter>> action,
            Expression<Action<ILogWriter>> verificationExpression, Times times)
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub);

            mock.Setup(verificationExpression, times);
            // Act
            action.Compile()(stub);

            // Assert
            mock.Verify();
        }

        public static IEnumerable<TestCaseData> GetVerifyWithOneActionTestCases()
        {
            var builder = new Builder<ILogWriter>();
            // Test cases for ILogWriter.Write(int)
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(42));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(42));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(It.IsAny<int>()));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(It.IsInRange(41, 43)));

            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(It.IsInRange(44, 46)),
                                                        Times.Never());

            // Failure cases
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(21))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(It.IsInRange(43, 44)))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(42), Times.Never())
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(42), Times.AtLeast(2))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(42), Times.Exactly(2))
                .Throws(typeof(VerificationException));
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.ToString())
                .Throws(typeof(VerificationException));

            // Test cases for ILogWriter.Write(string)
            yield return builder.Do(lw => lw.Write("foo")).Expects(lw => lw.Write("foo"));
            yield return builder.Do(lw => lw.Write("foo")).Expects(lw => lw.Write(It.IsAny<string>()));

            // Test cases for ILogWriter.Write(int, string)
            yield return builder.Do(lw => lw.Write(42, "Foo")).Expects(lw => lw.Write(42, "Foo"));
            yield return builder.Do(lw => lw.Write(42, "Foo"))
                .Expects(lw => lw.Write(It.IsAny<int>(), It.IsAny<string>()));
            yield return builder.Do(lw => lw.Write(42, "Foo"))
                .Expects(lw => lw.Write(It.IsInRange(1, 43), It.IsAny<string>()));

            // Tests cases with predicate-base verification
            yield return builder.Do(lw => lw.Write(42)).Expects(lw => lw.Write(It.Is<int>(n => n == 42)));
            yield return builder.Do(lw => lw.Write("Foooo"))
                .Expects(lw => lw.Write(It.Is<string>(s => s.ToLower().StartsWith("foo"))));
        }

        [TestCaseSource("GetVerifyWithSequenceOfActionsTestCases")]
        public void Test_Verify_With_Sequence_Of_Actions(
            List<Expression<Action<ILogWriter>>> actions,
            Expression<Action<ILogWriter>> verificationExpression, Times times)
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub);

            // Act
            foreach (var a in actions)
            {
                a.Compile()(stub);
            }

            // Assert
            mock.Verify(verificationExpression, times);
        }

        public IEnumerable<TestCaseData> GetVerifyWithSequenceOfActionsTestCases()
        {
            var builder = new Builder<ILogWriter>();

            yield return builder.Do(lw => lw.Write(42))
                .Do(lw => lw.Write(42))
                .Expects(lw => lw.Write(42), Times.Exactly(2));

        }
    }
}