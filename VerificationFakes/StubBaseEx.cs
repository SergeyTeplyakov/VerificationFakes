using System.Diagnostics.Contracts;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Linq;
using VerificationFakes.Core;
using System;

namespace VerificationFakes
{
    /// <summary>
    /// Contains set of extension methods for creating <see cref="Mock{T}"/> objects
    /// from the <see cref="StubBase{T}"/> objects (privided by Microsoft Fakes framework).
    /// </summary>
    public static class StubBaseEx
    {
         public static Mock<T> AsMock<T>(this StubBase<T> stub) where T : class
         {
             Contract.Requires(stub != null);
             return new Mock<T>(stub);
         }

        internal static ObservedCall[] GetObservedCalls(this StubObserver observer)
        {
            return observer.GetCalls()
                .Select(ci => new ObservedCall(ci.StubbedMethod, ci.GetArguments()))
                .ToArray();
        }
    }
}