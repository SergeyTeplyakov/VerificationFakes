using VerificationFakes;

namespace VerificationFakesTests
{
    interface IFoo
    { }
    public class Test_Contract
    {
        void Test()
        {
            var mock = new Mock<IFoo>(null, MockBehavior.Loose);
        } 
    }
}