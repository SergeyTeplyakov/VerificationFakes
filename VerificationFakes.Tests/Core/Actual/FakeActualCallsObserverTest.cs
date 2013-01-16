using System.Dynamic;
using System.Reflection;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using NUnit.Framework;
using VerificationFakes.Core;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakesTests.Core.Actual
{
    [TestFixture]
    public class FakeActualCallsObserverTest
    {
        [Test]
        public void Test_Event_Occurred_When_Observed_Method_Calls()
        {
            // Arrange
            var mock = new StubILogWriter();
            var actualObserver = new FakesCustomStubObserver(mock);
            MethodInfo methodCalled = null;
            object[] arguments = null;
            actualObserver.MethodCalled += (e, ea) => { methodCalled = ea.Method; arguments = ea.Arguments; };

            // Act
            ((ILogWriter)mock).Write(1, "2");

            // Assert
            Assert.IsNotNull(methodCalled, "MethodCalled event handler should be called.");
            Assert.That(methodCalled.Name, Is.EqualTo("Write"));

            Assert.IsNotNull(arguments, "MethodCall event handler should fill arguments.");
            CollectionAssert.AreEquivalent(new object[]{1, "2"}, arguments);
        }
    }
}