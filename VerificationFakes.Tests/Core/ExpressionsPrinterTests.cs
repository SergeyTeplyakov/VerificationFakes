using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Core;

namespace VerificationFakesTests
{
    [TestFixture]
    public class ExpressionsPrinterTests
    {
        private static string GetString<T>()
        {
            return "11";
        }

        [TestCaseSource("GetDataForBodyPrinting")]
        public string Test_Expression_Body_Printer(LambdaExpression expression)
        {
            return new ExpressionsPrinter(expression).GetBody();
        }

        private static IEnumerable<TestCaseData> GetDataForBodyPrinting()
        {
            yield return Create(() => new Generic<string>() != null)
                .Returns("new Generic<string>() != null");
            
            yield return Create(() => Generic<long>.Id == 0)
                .Returns("Generic<long>.Id == 0");
            
            yield return Create(() => Generic<float>.Default() > 0.0)
                .Returns("Generic<float>.Default() > 0");
            
            yield return Create(() => new Generic<double>().Get() <= 0.0)
                .Returns("new Generic<double>().Get() <= 0");

            yield return Create(() => new Generic<object>(1, "foo").t != null)
                .Returns("new Generic<object>(1, \"foo\").t != null");

            yield return Create(() => TimeSpan.FromSeconds(1) == TimeSpan.FromSeconds(2))
                .Returns("TimeSpan.FromSeconds(1) == TimeSpan.FromSeconds(2)");

            yield return Create(() => It.Is<int>(n => Generic<int>.Id == 0) == 0)
                .Returns("It.Is<int>(n => Generic<int>.Id == 0) == 0");

            yield return Create(() => Times.Once().Equals(Times.Exactly(5)))
                .Returns("Times.Once().Equals(Times.Exactly(5))");

            yield return Create(
                () => new Generic<string>(GetString<string>().Length, GetString<int>()).t == 
                    WithGenericMethod.Create<string>())
                .Returns("new Generic<string>(ExpressionsPrinterTests.GetString<string>().Length, ExpressionsPrinterTests.GetString<int>()).t == WithGenericMethod.Create<string>()");
        }

        private static TestCaseData Create(Expression<Func<bool>> e)
        {
            return new TestCaseData(e);
        }


        [TestCaseSource("CreateExpressionsForPrintingArguments")]
        public string Test_Print_Arguments(LambdaExpression lambdaExpression)
        {
            // Arrange
            var printer = new ExpressionsPrinter(lambdaExpression);

            // Act & Assert
            return printer.GetParameters();
        }

        private IEnumerable<TestCaseData> CreateExpressionsForPrintingArguments()
        {
            yield return Create(
                (Expression<Func<string, object, bool>>)((s, o) => false))
                    .Returns("(s, o)");
            yield return Create(
                (Expression<Func<string, object, int, bool>>)((s, o, i) => true))
                    .Returns("(s, o, i)");
            yield return Create(
                (Expression<Func<string, bool>>)((s) => true))
                    .Returns("s");
        }

        //private static TestCaseData Create<T, U>(Expression<Func<T, U, bool>> expression)
        private static TestCaseData Create(LambdaExpression expression)
        {
            return new TestCaseData(expression);
        }            
    }

    class WithGenericMethod
    {
        public static T Create<T>()
        {
            return default(T);
        }

        
    }

    class Generic<T>
    {
        public static readonly int Id = 42;

        public T t = default(T);

        public Generic(int i, string s)
        {
        }

        public Generic()
        {
        }

        public T Get()
        {
            return t;
        }

        public static T Default()
        {
            return default(T);
        }
    }
}