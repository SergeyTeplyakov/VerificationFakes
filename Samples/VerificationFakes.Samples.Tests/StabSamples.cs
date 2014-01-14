using System;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using NUnit.Framework;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakes.Samples.Tests
{
    /// <summary>
    /// Contains sample code for creating stubs using Stubs from Microsoft Fakes.
    /// </summary>
    /// <remarks>
    /// For more information about differences between mocks and stubs you can read following articles:
    /// 1. Mocks Aren't Stubs by Martin Fowler (http://martinfowler.com/articles/mocksArentStubs.html)
    /// 2. Mocks, Stubs and Fakes: it’s a continuum by Daniel Cazzulino
    ///    (http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/)
    /// 3. Стабы и моки (http://sergeyteplyakov.blogspot.com/2011/12/blog-post.html) (in Russian)
    /// </remarks>
    [TestFixture]
    public class StubSamples
    {
        [Test]
        public void Test_GetCurrentDirrectory_Simple_Stub()
        {
            // Arrange
            var stub = new StubILoggerDependency();
            stub.GetCurrentDirectory = () => "D:\\Temp";

            // Act
            // StubILoggerDependency реализует интерфейс ILoggerDependency
            ILoggerDependency loggerDependency = stub;
            var currentDirectory = loggerDependency.GetCurrentDirectory();
            Console.WriteLine("Current directory is {0}", currentDirectory);

            // Assert
            Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Stub_Always_Returns_The_Same_Value(string loggerName)
        {
            // Arrange
            var stub = new StubILoggerDependency();
            // For any parameter in the GetDirectoryByLoggerName Stub should return "C:\\Foo".
            stub.GetDirectoryByLoggerNameString = (string logger) => "C:\\Foo";

            ILoggerDependency loggerDependency = stub;
            // Act
            string directory = loggerDependency.GetDirectoryByLoggerName(loggerName);
            Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

            // Assert
            Assert.That(directory, Is.EqualTo("C:\\Foo"));
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Stub_Returns_Different_Value_Based_On_The_Arguments(string loggerName)
        {
            // Arrange
            var stub = new StubILoggerDependency();
            // Setting up our stub to return different values based on the argument.
            // This code is similar to following implementation:
            // public string GetDirectoryByLoggername(string s) { return "C:\\" + s; }
            stub.GetDirectoryByLoggerNameString = (string logger) => "C:\\" + logger;
            ILoggerDependency loggerDependency = stub;

            string directory = loggerDependency.GetDirectoryByLoggerName(loggerName);

            Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

            // Assert
            Assert.That(directory, Is.EqualTo("C:\\" + loggerName));
        }

        [Test]
        public void Test_DefaultLogger_Simple_Stub()
        {
            // Arrange
            var stub = new StubILoggerDependency();
            stub.DefaultLoggerGet = () => "DefaultLogger";
            ILoggerDependency loggerDependency = stub;

            // Act
            string defaultLogger = loggerDependency.DefaultLogger;
            Console.WriteLine("Default logger is '{0}'", defaultLogger);

            // Assert
            Assert.That(defaultLogger, Is.EqualTo("DefaultLogger"));
        }

        [Test]
        public void Test_RollingRequired_Event_Simple_Stub()
        {
            // Arrange
            var stub = new StubILoggerDependency();
            ILoggerDependency loggerDependency = stub;

            // Act
            bool eventOccurred = false;
            loggerDependency.RollingRequired += (sender, args) => eventOccurred = true;
            // Raising event manually
            stub.RollingRequiredEvent(this, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventOccurred, "Event should be raised!");
        }

        [Test]
        public void Test_Stubs_For_Generic_GetConfigValue_Of_T()
        {
            // Arrange
            var stub = new StubILoggerDependency();
            stub.GetConfigValueOf1<string>(() => "default");

            // Act
            ILoggerDependency loggerDependency = stub;
            var str = loggerDependency.GetConfigValue<string>();
            Console.WriteLine("Config value is '{0}'", str);

            // Assert
            Assert.That(str, Is.EqualTo("default"));
        }

        // TODO: and what the hell this interface for??
        class CustomStubBehavior : IStubBehavior
        {
            public bool TryGetValue<TValue>(object name, out TValue value)
            {
                Console.WriteLine("calling CustomStubBehavior.TryGetValue()");
                value = default(TValue);
                return true;
            }

            public TResult Result<TStub, TResult>(TStub target, string name) where TStub : IStub
            {
                Console.WriteLine("calling CustomStubBehavior.Result()");
                return default(TResult);
            }

            public void ValueAtReturn<TStub, TValue>(TStub target, string name, out TValue value) where TStub : IStub
            {
                Console.WriteLine("calling CustomStubBehavior.ValueAtReturn");
                value = default(TValue);
            }

            public void ValueAtEnterAndReturn<TStub, TValue>(TStub target, string name, ref TValue value) where TStub : IStub
            {
                Console.WriteLine("calling CustomStubBehavior.ValueAtEnterAndReturn");
            }

            public void VoidResult<TStub>(TStub target, string name) where TStub : IStub
            {
                Console.WriteLine("calling CustomStubBehavior.VoidResult");
            }
        }

        [Test]
        public void Test_BehaveAsDefaultValue()
        {
            // Arrange
            var stub = new StubILoggerDependency();
            stub.BehaveAsDefaultValue();

            ILoggerDependency logerDependency = stub;

            // Act
            var currentDirectory = logerDependency.GetCurrentDirectory();

            // Assert
            Assert.IsNull(currentDirectory);
        }

        [Test]
        public void Test_Custom_Stub_Behavior()
        {
            var stub = new StubILoggerDependency();
            stub.InstanceBehavior = new CustomStubBehavior();

            stub.BehaveAsDefaultValue();
            ILoggerDependency logerDependency = stub;
            var currentDirectory = logerDependency.GetCurrentDirectory();
        }

    }
}
