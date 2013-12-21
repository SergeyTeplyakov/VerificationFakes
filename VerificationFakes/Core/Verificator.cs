using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Verifies that expected calls corresponds to actualls calls depending on mocking behavior.
    /// </summary>
    internal sealed class Verificator
    {
        private readonly MockBehavior _behavior;
        private readonly IErrorFormatter _errorFormatter = new ErrorFormatter();

        public Verificator(MockBehavior behavior, IErrorFormatter errorFormatter)
        {
            Contract.Requires(errorFormatter != null, "errorFormatter should not be null.");

            _behavior = behavior;
            _errorFormatter = errorFormatter;
        }

        public void VerifyForUnexpectedCall(ICollection<ExpectedCall> expectedCalls, ObservedCall actualCall)
        {
            if (_behavior != MockBehavior.Strict)
                return;

            if (!IsCallExpected(expectedCalls, actualCall))
            {
                var error = _errorFormatter.FormatNotExpectedCallsInStrictMode(expectedCalls, new[] {actualCall});
                throw new VerificationException(error);
            }

        }

        public void VerifyAll(ICollection<ExpectedCall> expectedCalls, ICollection<ObservedCall> actualCalls)
        {
            Contract.Requires(expectedCalls != null, "expectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(expectedCalls, c => c != null),
                "expectedCalls should contain not-null objects.");

            Contract.Requires(actualCalls != null, "actualCall should not be null.");
            Contract.Requires(Contract.ForAll(actualCalls, c => c != null),
                "actualCall should contain not-null objects.");

            var mismatches = GetExpectedMismatches(expectedCalls, actualCalls);
            if (mismatches.Any())
            {
                string error = _errorFormatter.FormatVerifyAllError(expectedCalls, actualCalls);
                throw new VerificationException(error);
            }
        }

        public void Verify(ExpectedCall expected, ICollection<ObservedCall> actual)
        {
            Contract.Requires(expected != null, "expected should not be null.");
            Contract.Requires(actual != null, "actual should not be null.");

            var mismatches = GetExpectedMismatches(new []{expected}, actual);
            var firstMismatch = mismatches.FirstOrDefault();

            if (firstMismatch != null)
            {
                string error = _errorFormatter.FormatVerifyError(firstMismatch.ExpectedCall, 
                                                                 firstMismatch.Matches, firstMismatch.ObservedCalls);
                throw new VerificationException(error);
            }
        }

        private IEnumerable<Match> GetExpectedMismatches(ICollection<ExpectedCall> expectedCalls, 
            ICollection<ObservedCall> actualCalls)
        {
            var actuals = actualCalls
                .GroupBy(c => c.Method)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var e in expectedCalls)
            {
                List<ObservedCall> observedCalls;
                actuals.TryGetValue(e.Method, out observedCalls);
                observedCalls = observedCalls ?? new List<ObservedCall>();
                
                var match = Match.MatchCall(e, observedCalls);
                
                if (!match.IsMatched)
                {
                    yield return match;
                }
            }
        }

        private bool IsCallExpected(IEnumerable<ExpectedCall> expectedCalls, ObservedCall actualCall)
        {
            // Call is unexpected if we don't have any matched calls (not counting times)
            foreach(var expectedCall in expectedCalls.Where(ac => ac.Method == actualCall.Method))
            {
                if (expectedCall.MatchArguments(actualCall.Arguments))
                    return true;
            }

            return false;
        }

        private class Match
        {
            public bool IsMatched { get; private set; }
            public int Matches { get; private set; }
            public int ActualCalls { get; private set; }
            public ExpectedCall ExpectedCall { get; private set; }
            public IList<ObservedCall> ObservedCalls { get; private set; }

            public static Match MatchCall(ExpectedCall expected, IList<ObservedCall> actual)
            {
                int matches = actual.Count(e => expected.MatchArguments(e.Arguments));
                bool matched = expected.Times.Match(matches);
                return new Match
                {
                    IsMatched = matched,
                    Matches = matches,
                    ActualCalls = matches,
                    ExpectedCall = expected,
                    ObservedCalls = actual,
                };
            }
        }
    }

    
}