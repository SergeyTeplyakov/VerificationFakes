using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Produces correct string for verification errors.
    /// </summary>
    internal class ErrorFormatter : IErrorFormatter
    {
        public string FormatVerifyError(ExpectedCall expected, int actualCalls, 
            ICollection<ObservedCall> actual)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Expected invocation on the mock {0}, but was {1}.",
                                expected.Times, TimesToString(actualCalls))
               .AppendLine()
               .AppendFormat("Expected call: {0}", expected.CallExpression)
               .AppendLine().AppendLine();

            sb.AppendLine("Actual calls:");
            PerformedActionsToString(sb, actual);

            return sb.ToString();
        }

        public string FormatVerifyAllError(ICollection<ExpectedCall> expectedCalls, 
            ICollection<ObservedCall> actualCalls)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Expected invocations failed.").AppendLine();

            if (expectedCalls.Count == 0)
            {
                sb.AppendLine("No expected invocations.");
            }
            else
            {
                sb.AppendLine("Expected calls:");
                foreach(var ec in expectedCalls)
                {
                    sb.AppendFormat("{0}, {1}.", ec.CallExpression, ec.Times).AppendLine();
                }
            }
            
            sb.AppendLine();

            PerformedActionsToString(sb, actualCalls);
            return sb.ToString();
        }

        public string FormatNotExpectedCallsInStrictMode(ICollection<ExpectedCall> expectedCalls, 
            ICollection<ObservedCall> unexpectedCalls)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Following invocations failed with mock behavior Strict:");

            PerformedActionsToString(sb, unexpectedCalls);

            sb.AppendLine("Expected invocations:");
            if (expectedCalls.Count == 0)
            {
                sb.AppendLine("No expected invocations.");
            }
            else
            {
                foreach (var ec in expectedCalls)
                {
                    sb.AppendFormat("{0}, {1}.", ec.CallExpression, ec.Times).AppendLine();
                }
            }
            
            return sb.ToString();
        }

        private static void PerformedActionsToString(StringBuilder sb, 
                                                     ICollection<ObservedCall> actual)
        {
            if (actual.Count == 0)
            {
                sb.AppendLine("No invocations performed.");
            }
            else
            {
                foreach (var ac in actual)
                {
                    var arguments = ac.Arguments.Select(a =>
                                                        a is string ? string.Format("\"{0}\"", a) : a);

                    sb.AppendFormat("{0}.{1}({2})", ac.Method.DeclaringType,
                                    ac.Method.Name, string.Join(", ", arguments));
                }
            }
        }

        public string TimesToString(int times)
        {
            return times == 1 ? "1 time" : string.Format("{0} times", times);
        }
    }
}