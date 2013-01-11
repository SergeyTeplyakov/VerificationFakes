using System;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using VerificationFakes.Samples;
using VerificationFakes.Samples.Fakes;

namespace VerificationFakesTests
{
    [TestFixture]
    public class MockSetupTests
    {
        private ILogWriter _logWriter;
        private StubILogWriter _stub = new StubILogWriter();
        private Mock<ILogWriter> _mock;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<ILogWriter>(_stub);
            _logWriter = _stub;
        }

        [Test]
        public void Test_Setup_With_Two_Method_Calls()
        {
            // Arrange
            _mock.Setup(lw => lw.Write(It.IsAny<int>()));
            _mock.Setup(lw => lw.Write(It.IsAny<string>()));
            
            // Act
            _logWriter.Write(42);
            _logWriter.Write("foo");
            
            // Assert
            _mock.VerifyAll();
        }
        
        [Test]
        public void Test_Setup_With_Two_Method_Calls_Fails_If_Only_One_Method_Called()
        {
            // Arrange
            _mock.Setup(lw => lw.Write(It.IsAny<int>()));
            _mock.Setup(lw => lw.Write(It.IsAny<string>()));
            
            // Act
            _logWriter.Write(42);
            
            // Assert
            Assert.Throws<VerificationException>(() => _mock.VerifyAll(),
                "VerifyAll should fail because only one method was called.");
        }
    }
}