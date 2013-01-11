using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace FakeMockTests.Expected
{
    [TestFixture]
    internal class ArgumentExpressionParserTests
    {
        [TestCaseSource("GetArgumentValueExpressionsAsActions")]
        public ArgumentValue Parse_Arguments_For_Action_Expression(Expression<Action<ILogger>> expression)
        {
            // Arrange
            var parser = new ExpressionsParser();

            // Act & Assert
            return parser.Parse(expression).Arguments[0];
        }

        [TestCaseSource("GetArgumentValueExpressionsAsFuncs")]
        public ArgumentValue Parse_Arguments_For_Func_Expression(Expression<Func<ILogger, int>> expression)
        {
            // Arrange
            var parser = new ExpressionsParser();

            // Act & Assert
            return parser.Parse(expression).Arguments[0];
        }

        /// <summary>
        /// Factory method that provides all test data for argument parsing
        /// </summary>
        public static IEnumerable<TestCaseData> GetArgumentValueExpressionsAsActions()
        {
            // Single argument values
            yield return Create(lw => lw.Write(42))
                .Returns(new SingleArgumentValue(42));

            yield return Create(lw => lw.Write("42"))
                .Returns(new SingleArgumentValue("42"));

            // Values obtained by method call or property access
            yield return Create(lw => lw.Write(GetValue()))
                .Returns(new SingleArgumentValue(GetValue()));

            yield return Create(lw => lw.Write(Value))
                .Returns(new SingleArgumentValue(Value));

            // Any argument values
            yield return Create(lw => lw.Write(It.IsAny<int>()))
                .Returns(new AnyArgumentValue());

            yield return Create(lw => lw.Write(It.IsAny<string>()))
                .Returns(new AnyArgumentValue());

            // Range argument values
            yield return Create(lw => lw.Write(It.IsInRange(1, 10)))
                .Returns(new RangeArgumentValue(1, 10));

            yield return Create(lw => lw.Write(It.IsInRange(1, 10)))
                .Returns(new RangeArgumentValue(1, 10));

            yield return Create(lw => lw.Write(It.IsInRange(GetLowerBound(), UpperBound)))
                .Returns(new RangeArgumentValue(GetLowerBound(), UpperBound));

            // Predicate-based argument values
            yield return Create(lw => lw.Write(It.Is<int>(value => value % 2 == 0)))
                .Returns(new ExpressionArgumentValue<int>(value => value % 2 == 0));

            yield return Create(lw => lw.Write(It.Is<string>(value => value.StartsWith("aa"))))
                .Returns(new ExpressionArgumentValue<string>(value => value.StartsWith("aa")));
        }

        /// <summary>
        /// Factory method that provides all test data for argument parsing
        /// </summary>
        public static IEnumerable<TestCaseData> GetArgumentValueExpressionsAsFuncs()
        {
            // Simple value tests
            yield return Create(lw => lw.Read(42))
                .Returns(new SingleArgumentValue(42));

            yield return Create(lw => lw.Read("42"))
                .Returns(new SingleArgumentValue("42"));

            // Values obtained by method call or property access
            yield return Create(lw => lw.Read(GetValue()))
                .Returns(new SingleArgumentValue(GetValue()));

            yield return Create(lw => lw.Read(Value))
                .Returns(new SingleArgumentValue(Value));

            // Using It class
            yield return Create(lw => lw.Read(It.IsAny<int>()))
                .Returns(new AnyArgumentValue());

            yield return Create(lw => lw.Read(It.IsAny<string>()))
                .Returns(new AnyArgumentValue());

            yield return Create(lw => lw.Read(It.IsInRange(1, 10)))
                .Returns(new RangeArgumentValue(1, 10));

            yield return Create(lw => lw.Read(It.IsInRange(1, 10)))
                .Returns(new RangeArgumentValue(1, 10));

            yield return Create(lw => lw.Read(It.IsInRange(GetLowerBound(), UpperBound)))
                .Returns(new RangeArgumentValue(GetLowerBound(), UpperBound));

            yield return Create(lw => lw.ToString().Length)
                .Throws(typeof(InvalidOperationException))
                .SetDescription("Non method call expresion should fail!");
        }

        private static int GetValue() { return 42; }

        private static int Value { get { return 42; } }

        private static int GetLowerBound() { return 11; }
        private static int UpperBound { get { return 42; } }

        static TestCaseData Create(Expression<Action<ILogger>> expression)
        {
            return new TestCaseData(expression);
        }

        static TestCaseData Create(Expression<Func<ILogger, int>> expression)
        {
            return new TestCaseData(expression);
        }
    }
}