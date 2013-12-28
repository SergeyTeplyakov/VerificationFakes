using System;
using System.Collections;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using NUnit.Framework;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakes.Samples.Tests
{
    [TestFixture]
    public class ShimSamples
    {
        [Test]
        public void Test_GetCurrentDirrectory_Simple_Shim()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                shim.GetCurrentDirectory = () => "D:\\Temp";

                // Act
                var loggerDependency = shim.Instance;
                var currentDirectory = loggerDependency.GetCurrentDirectory();
                Console.WriteLine("Current directory is {0}", currentDirectory);

                // Assert
                Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
            }
        }

        [Test]
        public void Test_GetCurrentDirrectory_Simple_Shim_For_All_Instances()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                ShimSealedLoggerDependency.AllInstances.GetCurrentDirectory = 
                    (SealedLoggerDependency d) => "D:\\Temp";
                var loggerDependency = new SealedLoggerDependency();
                
                // Act
                var currentDirectory = loggerDependency.GetCurrentDirectory();
                Console.WriteLine("Current directory is {0}", currentDirectory);

                // Assert
                Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
            }
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Shim_Always_Returns_The_Same_Value(string loggerName)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                // For any parameter in the GetDirectoryByLoggerName Stub should return "C:\\Foo".
                shim.GetDirectoryByLoggerNameString = (string logger) => "C:\\Foo";
                var loggerDependency = shim.Instance;

                // Act
                string directory = loggerDependency.GetDirectoryByLoggerName(loggerName);
                Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

                // Assert
                Assert.That(directory, Is.EqualTo("C:\\Foo"));
            }
        }

        [TestCase("Foo")]
        [TestCase("Boo")]
        public void Test_GetDirectoryByLoggerName_Shim_Returns_Different_Value_Based_On_The_Arguments(string loggerName)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                // Setting up our stub to return different values based on the argument.
                // This code is similar to following implementation:
                // public string GetDirectoryByLoggername(string s) { return "C:\\" + s; }
                shim.GetDirectoryByLoggerNameString = (string logger) => "C:\\" + logger;
                var loggerDependency = shim.Instance;

                string directory = loggerDependency.GetDirectoryByLoggerName(loggerName);

                Console.WriteLine("Directory for the logger '{0}' is '{1}'", loggerName, directory);

                // Assert
                Assert.That(directory, Is.EqualTo("C:\\" + loggerName));
            }
        }

        [Test]
        public void Test_DefaultLogger_Simple_Shim()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                shim.DefaultLoggerGet = () => "DefaultLogger";
                var loggerDependency = shim.Instance;

                // Act
                string defaultLogger = loggerDependency.DefaultLogger;
                Console.WriteLine("Default logger is '{0}'", defaultLogger);

                // Assert
                Assert.That(defaultLogger, Is.EqualTo("DefaultLogger"));
            }
        }

        [Test]
        public void Test_RollingRequired_Event_Simple_Shim()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                var loggerDependency = shim.Instance;

                // Act
                // We can't override firing event directly!
                // But we can override subscription and unsubscription.
                EventHandler backingDelegate = null;
                shim.RollingRequiredAddEventHandler = handler => backingDelegate += handler;

                bool eventOccurred = false;
                loggerDependency.RollingRequired += (sender, args) => eventOccurred = true;
                
                // Raising event manually
                backingDelegate(this, EventArgs.Empty);

                Assert.IsTrue(eventOccurred, "Event should be raised!");
            }
        }

        [Test]
        public void Test_Shim_For_Generic_GetConfigValue_Of_T()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var shim = new ShimSealedLoggerDependency();
                shim.GetConfigValueOf1(() => "default");

                // Act
                var loggerDependency = shim.Instance;
                var str = loggerDependency.GetConfigValue<string>();
                Console.WriteLine("Config value is '{0}'", str);

                // Assert
                Assert.That(str, Is.EqualTo("default"));
            }
        }

        [Test]
        public void Test_Shim_For_Static_GetDefaultDirectory()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                ShimSealedLoggerDependency.GetDefaultDirectory = () => "C:\\Windows";

                // Act
                var defaultDirectory = SealedLoggerDependency.GetDefaultDirectory();
                Console.WriteLine("Default directory is '{0}'", defaultDirectory);

                Assert.That(defaultDirectory, Is.EqualTo("C:\\Windows"));
            }
        }
    }
}