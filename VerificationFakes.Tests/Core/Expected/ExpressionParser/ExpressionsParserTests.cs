using System;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace FakeMockTests.Expected
{
    [TestFixture]
    internal class ExpressionsParserTests
    {
        [TestFixture]
        public class Write_Void_MethodTests
        {
            private ExpectedCall _expectedCall;

            [SetUp]
            public void Setup()
            {
                // Arrange
                var expression = (Expression<Action<ILogger>>)(l => l.Write());
                var parser = new ExpressionsParser();

                // Act
                _expectedCall = parser.Parse(expression);
            }

            [Test]
            public void Expected_Method_Is_Write()
            {
                Assert.That(_expectedCall.Method.Name, Is.EqualTo("Write"));
            }

            [Test]
            public void Expected_Arguments_Are_Empty()
            {
                CollectionAssert.IsEmpty(_expectedCall.Arguments);
            }

            [Test]
            public void Expected_Times_Is_Once()
            {
                Assert.That(_expectedCall.Times, Is.EqualTo(Times.Once()));
            }
        }

        [Test]
        public void Test_Parse_Returns_Valid_CallExpression()
        {
            // Arrange
            var expression = (Expression<Action<ILogger>>)(l => l.Write(1, 2.0));
            var parser = new ExpressionsParser();

            // Act
            var result = parser.Parse(expression);
            Console.WriteLine("Expression as string: {0}", result.CallExpression);

            // Assert
            Assert.That(result.CallExpression.Contains("l.Write"));
        }
        
        [Test]
        public void Test_Parse_Two_Arguments()
        {
            // Arrange
            var expression = (Expression<Action<ILogger>>)(l => l.Write(1, 2.0));
            var parser = new ExpressionsParser();

            // Act
            var result = parser.Parse(expression);

            // Assert
            Assert.That(result.Arguments.Length, Is.EqualTo(2));
            Assert.That(result.Arguments[0], Is.EqualTo(new SingleArgumentValue(1)));
            Assert.That(result.Arguments[1], Is.EqualTo(new SingleArgumentValue(2.0)));
        }

        [Test]
        public void Test_Parse_Simple_Expression()
        {
            // Arrange
            var expression = (Expression<Action<ILogger>>)(l => l.Write());
            var parser = new ExpressionsParser();

            // Act
            var result = parser.Parse(expression);

            // Assert
            Assert.That(result.Method.Name, Is.EqualTo("Write"));
            CollectionAssert.IsEmpty(result.Arguments);
            Assert.That(result.Times, Is.EqualTo(Times.Once()));
        }

        [Test]
        public void Test_Non_Dependencies_Method_Call_Should_Not_Fail()
        {
            // Arrange
            var expression = (Expression<Action<ILogger>>) (l => Console.WriteLine("Oops!"));
            var parser = new ExpressionsParser();

            // Act
            var expected = parser.Parse(expression);

            // Assert
            Assert.IsNotNull(expected, "Parser should parse expressions with other calls as well.");
        }
        
        [Test]
        public void Test_Not_Method_Call_Expression_Should_Not_Fail()
        {
            // Arrange
            var expression = (Expression<Action<ILogger>>) (l => l.DummyProperty.ToString());
            var parser = new ExpressionsParser();

            // Act
            var expected = parser.Parse(expression);

            // Assert
            Assert.IsNotNull(expected, "Parser should parse property access expressions.");
        }
    }
}