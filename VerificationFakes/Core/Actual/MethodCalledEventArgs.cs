using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace VerificationFakes.Core
{
    internal class MethodCalledEventArgs : EventArgs
    {
        public MethodCalledEventArgs(MethodInfo method, object[] arguments)
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
}