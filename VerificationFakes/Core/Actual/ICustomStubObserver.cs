using System;
using System.Collections.Generic;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Observes all actuall calls that occurs during testing period.
    /// </summary>
    /// <remarks>
    /// There is two main purposes for this interface: (1) abstracted away Microsoft Fakes
    /// and provide simple event that would be raied when stub method called.
    /// </remarks>
    internal interface ICustomStubObserver
    {
        ObservedCall[] GetObservedCalls();
        event EventHandler<MethodCalledEventArgs> MethodCalled;
    }
}