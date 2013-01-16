using System;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakesTests
{
    [TestFixture]
    public class StrictModeTests
    {
        [Test]
        public void Test_Call_To_Unknown_Method_Should_Fail()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub, MockBehavior.Strict);

            // Act
            Assert.Throws<VerificationException>(() => ((ILogWriter) stub).Write(42),
                "Strict mock should fail because of unknown method call.");
        }
        
        [Test]
        public void Test_Call_To_Known_Method_Should_Not_Fail()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub, MockBehavior.Strict);
            mock.Setup(lw => lw.Write(It.IsAny<int>()));

            // Act
            ((ILogWriter)stub).Write(42);

            mock.VerifyAll();
        }
        
        [Test]
        public void Test_Unexpected_Call_Should_Fail_To_Method_With_Another_Argument_Value()
        {
            // Arrange
            var stub = new StubILogWriter();
            ILogWriter logWriter = stub;
            var mock = new Mock<ILogWriter>(stub, MockBehavior.Strict);

            mock.Setup(lw => lw.Write(2));
            mock.Setup(lw => lw.Write(3));

            // Act
            logWriter.Write(2);
            logWriter.Write(3);

            Assert.Throws<VerificationException>(() => logWriter.Write(5), 
                "Unexpected method should fail with strict mode.");
        }
    }
}