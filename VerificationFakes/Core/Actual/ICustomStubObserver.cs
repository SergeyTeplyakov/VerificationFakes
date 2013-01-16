using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace VerificationFakes.Core
{
    internal class MethodCallsEventArgs : EventArgs
    {
        public MethodCallsEventArgs(MethodInfo method, object[] arguments)
        {
            Contract.Requires(method != null, "method should not be null.");
            Contract.Requires(arguments != null, "method should not be null.");

            Method = method;
            Arguments = arguments;
            ObservedCall = new ObservedCall(method, arguments);
        }

        public MethodInfo Method { get; private set; }
        public object[] Arguments { get; private set; }

        public ObservedCall ObservedCall { get; private set; }
    }

    /// <summary>
    /// Observes all actuall calls that occurs during testing period.
    /// </summary>
    internal interface ICustomStubObserver
    {
        ObservedCall[] GetObservedCalls();
        event EventHandler<MethodCallsEventArgs> MethodCalled;
    }
}