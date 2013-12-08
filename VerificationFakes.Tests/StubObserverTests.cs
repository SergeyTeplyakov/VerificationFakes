using System;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using NUnit.Framework;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;

namespace FakeMockTests
{
    [TestFixture]
    public class StubObserverTests
    {
        [Test]
        public void Sampe()
        {
            var mock = new StubILogWriter();
            mock.WriteRInt32 = n => 42;

            //int exptected = 0;
            //mock.WriteInt32 = n =>
            //                      {
            //                          //
            //                          exptected = n;
            //                          Console.WriteLine(n == 42);
            //                      };

            ILogWriter logWriter = (ILogWriter) mock;
            var expected = logWriter.WriteR(12312);

            //logWriter.Write(42);

            Assert.That(expected, Is.EqualTo(42));


        }

        [Test]
        public void Test_Stub_Observer_Contains_Valid_Argument_And_Method_Info()
        {
            // Arrange
            var mock = new StubILogWriter();
            var stubObserver = new StubObserver();
            mock.InstanceObserver = stubObserver;

            // Act
            ((ILogWriter)mock).Write("Foo");
            
            // Assert
            var calls = stubObserver.GetCalls();
            CollectionAssert.IsNotEmpty(calls, "Should be at least one call to the observer.");

            var callInfo = calls[0];
            
            Assert.That(callInfo.GetArguments()[0], Is.EqualTo("Foo"),
                "Mocked method should be called with 'Foo' argument.");

            Assert.That(callInfo.StubbedMethod.Name, Is.EqualTo("Write"),
                "ILogWriter.Write method should be called.");
        }
    }
}