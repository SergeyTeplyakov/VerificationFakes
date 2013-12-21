using System.Diagnostics.Contracts;
using System.Reflection;
using System.Linq;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Data object that holds all required assumptions for expected calls.
    /// </summary>
    internal class ExpectedCall
    {
        public string CallExpression { get; set; }
        public MethodInfo Method { get; set; }
        public ArgumentValue[] Arguments { get; set; }
        public Times Times { get; set; }

        // TODO ST: move to other class.
        public bool MatchArguments(object[] actualArguments)
        {
            Contract.Requires(actualArguments != null, "actualArguments should not be null.");
            Contract.Requires(actualArguments.Length == Arguments.Length,
                "Expected and actual argument length should match");

            // First of all we should combine Expected arguments with Actual values.
            // Then we should make sure that all actual values match to appropriate
            // expected arguments
            return Arguments.Zip(actualArguments,
                                (arg, actual) => new {Expected = arg, Actual = actual})
                .All(a => a.Expected.Match(a.Actual));
        }
    }
}