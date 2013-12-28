using System.Diagnostics.Contracts;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using VerificationFakes.Core;

namespace VerificationFakes
{
    /// <summary>
    /// Contains set of extension methods for creating <see cref="Mock{T}"/> objects
    /// from the <see cref="StubBase{T}"/> objects (privided by Microsoft Fakes framework).
    /// </summary>
    public static class StubBaseEx
    {
        /// <summary>
        /// Creates mock-object out of Microsoft Fakes stubs.
        /// </summary>
        public static Mock<T> AsMock<T>(this StubBase<T> stub) where T : class
        {
            Contract.Requires(stub != null);
            Contract.Ensures(Contract.Result<Mock<T>>() != null);

            return new Mock<T>(stub);
        }

        /// <summary>
        /// Creates strict mock-object out of Microsoft Fakes stub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stub"></param>
        /// <returns></returns>
        public static Mock<T> AsStrictMock<T>(this StubBase<T> stub) where T : class
        {
            Contract.Requires(stub != null);
            Contract.Ensures(Contract.Result<Mock<T>>() != null);

            return new Mock<T>(stub, MockBehavior.Strict);
        } 

        internal static ObservedCall[] GetObservedCalls(this StubObserver observer)
        {
            return observer.GetCalls()
                .Select(ci => new ObservedCall(ci.StubbedMethod, ci.GetArguments()))
                .ToArray();
        }
    }
}