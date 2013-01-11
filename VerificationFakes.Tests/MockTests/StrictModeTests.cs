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
            ((ILogWriter)stub).Write(42);

            Assert.Throws<VerificationException>(() => mock.VerifyAll(),
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
        public void Test_Call_To_Unknown_Method_With_Invalid_Verify_Should_Fail()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = new Mock<ILogWriter>(stub, MockBehavior.Strict);

            mock.Setup(lw => lw.Write(It.IsAny<int>()));

            // Act
            ((ILogWriter)stub).Write("value");
            ((ILogWriter)stub).Write(42);

            Assert.Throws<VerificationException>(() => mock.VerifyAll(),
                "Strict mock should fail because of unknown method call.");
        }
    }
}