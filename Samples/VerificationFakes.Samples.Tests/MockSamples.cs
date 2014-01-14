using NUnit.Framework;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakes.Samples.Tests
{
    [TestFixture]
    public class MockSamples
    {
        [Test]
        public void Test_WriteLine_Calls_Write_With_Any_Of_String()
        {
            // StubILogWriter : StubBase<ILogWriter>, ILogWriter
            // Arrange
            var stub = new StubILogWriter();
            Mock<ILogWriter> mock = stub.AsMock();
            var logger = new Logger(stub);

            // Act
            logger.Write("Hello, logger!");
            
            // Assert
            // Checking that Write method of the ILogWriter was called
            mock.Verify(lw => lw.Write(It.IsAny<string>()));
        }

        [Test]
        public void Test_WriteLine_Calls_Write_With_Appropriate_Argument()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsMock();
            var logger = new Logger(stub);

            // Act
            logger.Write("Hello, logger!");

            // Assert
            // Checking that Write method was called with appropriate argument
            mock.Verify(lw => lw.Write("Hello, logger!"));
        }

        [Test]
        public void Test_WriteLine_Called_Exactly_Once()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsMock();
            var logger = new Logger(mock.Object);

            // Act
            logger.Write("Hello, logger!");

            // Assert
            // We could check, that particular method calls specified number of times
            mock.Verify(lw => lw.Write(It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void Test_WriteLine_Never_Calls_Write_Int()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsMock();
            var logger = new Logger(mock.Object);

            // Act
            logger.Write("Hello, logger");

            // Assert
            mock.Verify(lw => lw.Write(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void Test_Write_Int_Calls_Write_With_Argument_In_Specific_Interval()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsMock();
            var logger = new Logger(mock.Object);

            // Act
            logger.Write(42);

            // Assert
            mock.Verify(lw => lw.Write(It.IsInRange(40, 50)));
        }

        // TODO ST: add test that fails!

        [Test]
        public void Test_WriteLine_Calls_Write_With_Setup_Method()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsMock();
            mock.Setup(lw => lw.Write(It.IsAny<string>()));
            mock.Setup(lw => lw.Write(It.IsAny<int>()));

            var logger = new Logger(mock.Object);

            // Act
            logger.Write("Hello, logger!");
            logger.Write(42);

            // Assert
            // We're not explicitly stated what we're expecting.
            // mock.Setup expectations would be use.
            mock.Verify();
        }

        [Test]
        public void Test_WriteLine_Will_Fail_With_Strict_Mode()
        {
            // Arrange
            var stub = new StubILogWriter();
            var mock = stub.AsStrictMock();
            mock.Setup(lw => lw.Write("Foo"));

            // Act
            var logger = new Logger(mock.Object);

            logger.Write("Foo");
            // logger.Write will call logWriter.Write,
            // but we do not expect this, thats why logger.Write will fail!
            Assert.Throws<VerificationException>(() => logger.Write("Hello, Logger"));
        }
    }
}