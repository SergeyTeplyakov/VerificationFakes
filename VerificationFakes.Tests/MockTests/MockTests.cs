using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakesTests
{
    [TestFixture]
    public class MockTests
    {
        [TestFixture]
        public class Verify_Write_42
        {
            StubILogWriter _stub = new StubILogWriter();
            Mock<ILogWriter> _mock;

            [SetUp]
            public void Setup()
            {
                // Arrange
                _mock = new Mock<ILogWriter>(_stub);

                // Act
                ((ILogWriter)_stub).Write(42);
            }

            [Test]
            public void Write_Method_Called()
            {
                _mock.Verify(lw => lw.Write(42));
            }

            [Test]
            public void Write_Method_Called_For_Any()
            {
                _mock.Verify(lw => lw.Write(It.IsAny<int>()));
            }

            [Test]
            public void Write_Method_Never_Called_For_Invalid_Arg()
            {
                _mock.Verify(lw => lw.Write(36), Times.Never());
            }
            
            [Test]
            public void Verify_Failed_For_Invalid_Argument()
            {
                Assert.Throws<VerificationException>(() => _mock.Verify(lw => lw.Write(36)));
            }

            [Test]
            public void Verify_Failed_For_Invalid_Range()
            {
                Assert.Throws<VerificationException>(() => _mock.Verify(lw => lw.Write(It.IsInRange(20, 22))));
            }

            [Test]
            public void Verify_Failed_For_AtLeast_5()
            {
                Assert.Throws<VerificationException>(
                    () => _mock.Verify(lw => lw.Write(42), Times.AtLeast(5)));
            }
            
            [Test]
            public void Verify_Failed_For_Valid_Argument_And_Never()
            {
                Assert.Throws<VerificationException>(
                    () => _mock.Verify(lw => lw.Write(42), Times.Never()));
            }

        }

        [TestFixture]
        public class MockVerifyTests
        {
            [Test]
            public void Test_Verify_Method_Fails_If_Required_Method_Was_Not_Called()
            {
                // Arrange
                var stub = new StubILogWriter();
                var mock = new Mock<ILogWriter>(stub);

                // Act
                ((ILogWriter)stub).Write("42");

                mock.Verify(lw => lw.Write(It.IsAny<string>()));

                // Assert
                Assert.Throws<VerificationException>(
                    () => mock.Verify(lw => lw.Write("")));
            }

            [Test]
            public void Verify_Write_With_Invalid_Arguments_Never_Called()
            {
                // Arrange
                var stub = new StubILogWriter();
                var mock = new Mock<ILogWriter>(stub);

                // Act
                ((ILogWriter)stub).Write("42");

                // Assert
                mock.Verify(lw => lw.Write(""), Times.Never());
            }

            [Test]
            public void Test_Verify_Method_Fails_If_Wrong_Method_Was_Called()
            {
                // Arrange
                var stub = new StubILogWriter();
                var mock = new Mock<ILogWriter>(stub);

                // Act
                ((ILogWriter)stub).Write("42");

                // Assert
                Assert.Throws<VerificationException>(
                    () => mock.Verify(lw => lw.ToString()));
            }

            [Test]
            public void Test_Verify_Succeeds_If_Correct_Method_Calls()
            {
                // Arrange
                var stub = new StubILogWriter();
                var mock = stub.AsMock();

                // Act
                ((ILogWriter)stub).Write("42");

                // Assert
                mock.Verify(lw => lw.Write(It.IsAny<string>()));
            }
        }
    }
}
