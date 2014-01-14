using System;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using NUnit.Framework;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakes.Samples.Tests
{
    [TestFixture]
    public class ManualVerificationTesting
    {
        class CustomObserver : IStubObserver
        {
            public string CalledMethodName;
            public object CalledMethodArgument;

            public void Enter(Type stubbedType, Delegate stubCall, object arg1)
            {
                CalledMethodName = stubCall.Method.Name;
                CalledMethodArgument = arg1;
            }

            // Остальные перегрузки методы Enter опущены

            public void Enter(Type stubbedType, Delegate stubCall)
            {
                throw new NotImplementedException();
            }

            public void Enter(Type stubbedType, Delegate stubCall, object arg1, object arg2)
            {
                throw new NotImplementedException();
            }

            public void Enter(Type stubbedType, Delegate stubCall, object arg1, object arg2, object arg3)
            {
                throw new NotImplementedException();
            }

            public void Enter(Type stubbedType, Delegate stubCall, params object[] args)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Logger_Write_Calls_For_Enter_Method()
        {
            // Arrange
            var stub = new StubILogWriter();
            var customObserver = new CustomObserver();
            // Providing custom observer to the stub
            stub.InstanceObserver = customObserver;

            // Act
            ILogWriter logWriter = stub;
            logWriter.Write("Message");

            // Assert
            Assert.That(customObserver.CalledMethodName, 
                Is.StringContaining("ILogWriter.Write"));
            Assert.That(customObserver.CalledMethodArgument, Is.EqualTo("Message"));
        }
    }
}