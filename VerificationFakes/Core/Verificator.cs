using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace VerificationFakes.Core
{
    internal sealed class Verificator
    {
        private readonly MockBehavior _behavior;
        private readonly IErrorFormatter _errorFormatter;

        public Verificator(MockBehavior behavior, IErrorFormatter errorFormatter)
        {
            Contract.Requires(errorFormatter != null, "errorFormatter should not be null.");

            _behavior = behavior;
            _errorFormatter = errorFormatter;
        } 

        public void VerifyAll(ICollection<ExpectedCall> expectedCalls, ICollection<ObservedCall> actualCalls)
        {
            Contract.Requires(expectedCalls != null, "expectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(expectedCalls, c => c != null),
                "expectedCalls should contain not-null objects.");

            Contract.Requires(actualCalls != null, "actualCalls should not be null.");
            Contract.Requires(Contract.ForAll(actualCalls, c => c != null),
                "actualCalls should contain not-null objects.");

            VerifyAllExpectedCalls(expectedCalls, actualCalls);

            // Even for strict mocks we're checking current assumptions first
            // and only if all of them matches we'll check oustanding calls
            if (_behavior == MockBehavior.Strict)
            {
                var unexpectedCalls = GetUnexpectedCalls(expectedCalls, actualCalls).ToList();
                if (unexpectedCalls.Count != 0)
                {
                    var error = _errorFormatter.FormatNotExpectedCallsInStrictMode(expectedCalls, unexpectedCalls);
                    throw new VerificationException(error);
                }
            }
        }

        public void Verify(ExpectedCall expected, ICollection<ObservedCall> actual)
        {
            Contract.Requires(expected != null, "expected should not be null.");
            Contract.Requires(actual != null, "actual should not be null.");

            VerifyExpectedCall(expected, actual);

            // Even for strict mocks we're checking current assumptions first
            // and only if all of them matches we'll check oustanding calls
            if (_behavior == MockBehavior.Strict)
            {
                var unexpected = GetUnexpectedCalls(new[] {expected}, actual).ToList();
                if (unexpected.Count != 0)
                {
                    var error = _errorFormatter.FormatNotExpectedCallsInStrictMode(new[] {expected}, unexpected);
                    throw new VerificationException(error);
                }
            }
        }

        private void VerifyAllExpectedCalls(ICollection<ExpectedCall> expectedCalls, 
            ICollection<ObservedCall> actualCalls)
        {
            var mismatches = GetExpectedMismatches(expectedCalls, actualCalls);
            if (mismatches.Any())
            {
                string error = _errorFormatter.FormatVerifyAllError(expectedCalls, actualCalls);
                throw new VerificationException(error);
            }
        }

        private void VerifyExpectedCall(ExpectedCall expectedCall, ICollection<ObservedCall> actualCalls)
        {
            var mismatches = GetExpectedMismatches(new []{expectedCall}, actualCalls);
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

        private IEnumerable<ObservedCall> GetUnexpectedCalls(IEnumerable<ExpectedCall> expectedCalls,
            IEnumerable<ObservedCall> actualCalls)
        {
            // Unexpected calls is a set of unique actual calls without appropriate expected call
            
            var expectedDictionary = expectedCalls
                .GroupBy(c => c.Method)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach(var ac in actualCalls)
            {
                if (!expectedDictionary.ContainsKey(ac.Method))
                    yield return ac;
            }
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