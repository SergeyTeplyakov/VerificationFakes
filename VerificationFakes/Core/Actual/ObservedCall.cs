using System.Diagnostics.Contracts;
using System.Reflection;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Represents method calls that really occurred for the mocked object.
    /// </summary>
    internal sealed class ObservedCall
    {
        public MethodInfo Method { get; private set; }
        public object[] Arguments { get; private set; }

        public ObservedCall(MethodInfo method, object[] arguments)
        {
            Contract.Requires(method != null, "method should not be null.");
            Contract.Requires(arguments != null, "arguments should not be null.");

            Method = method;
            Arguments = arguments;
        }

        public override string ToString()
        {
            var args = string.Join(",", Arguments);
            return string.Format("{0}({1})", Method.Name, args);
        }
    }
}