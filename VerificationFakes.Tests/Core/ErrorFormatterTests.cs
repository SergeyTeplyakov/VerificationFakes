using System;
using System.Collections.Generic;
using NUnit.Framework;
using VerificationFakes;
using System.Linq;
using VerificationFakes.Core;

namespace FakeMockTests
{
    [TestFixture]
    public class ErrorFormatterTests
    {
        [Test]
        public void Test_Times_Onces()
        {
            // Arrange
            var formatter = new ErrorFormatter();
            var expected = new ExpectedCall
                {
                    Arguments = new ArgumentValue[] {new SingleArgumentValue(1), },
                    CallExpression = "SomeMethod(foo)",
                    Method = GetType().GetMethods().First(),
                    Times = Times.Once()
                };

            var observedCalls = new List<ObservedCall>
                {
                    new ObservedCall(GetType().GetMethods().First(), new object[]{1})
                };
            
            // Act
            string error = formatter.FormatVerifyError(expected, 0, observedCalls);
            Console.WriteLine("Error: {0}", error);

            Assert.That(error, Is.StringContaining("SomeMethod(foo)"),
                "Error should contain expected call expression.");
            
            Assert.That(error, Is.StringContaining("Test_Times_Once"),
                "Error should contain actual call.");
        }

        // TODO ST: add more tests for error formatter
    }
}