using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Interface for getting correct string for verification errors.
    /// </summary>
    [ContractClass(typeof(ErrorFormatterContracts))]
    internal interface IErrorFormatter
    {
        string FormatVerifyError(ExpectedCall expected, int actualCalls, 
            ICollection<ObservedCall> actual);
        
        string FormatVerifyAllError(ICollection<ExpectedCall> expectedCalls, 
            ICollection<ObservedCall> actualCalls);

        string FormatNotExpectedCallsInStrictMode(ICollection<ExpectedCall> expectedCalls,
            ICollection<ObservedCall> unexpectedCalls);

        string TimesToString(int times);
    }

    [ContractClassFor(typeof(IErrorFormatter))]
    internal abstract class ErrorFormatterContracts : IErrorFormatter
    {
        string IErrorFormatter.FormatVerifyError(ExpectedCall expected, int actualCalls, 
            ICollection<ObservedCall> actual)
        {
            Contract.Requires(expected != null, "expected should not be null.");
            Contract.Requires(actual != null, "actual should not be null.");

            Contract.Ensures(Contract.Result<string>() != null);

            throw new NotImplementedException();
        }

        string IErrorFormatter.FormatVerifyAllError(ICollection<ExpectedCall> expectedCalls, ICollection<ObservedCall> actualCalls)
        {
            Contract.Requires(expectedCalls != null, "expectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(expectedCalls, c => c != null),
                "expectedCalls should contain not-null objects.");

            Contract.Requires(actualCalls != null, "unexpectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(actualCalls, c => c != null),
                "unexpectedCalls should contain not-null objects.");

            Contract.Ensures(Contract.Result<string>() != null);

            throw new NotImplementedException();
        }

        public string FormatNotExpectedCallsInStrictMode(ICollection<ExpectedCall> expectedCalls, ICollection<ObservedCall> unexpectedCalls)
        {
            Contract.Requires(expectedCalls != null, "expectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(expectedCalls, c => c != null),
                "expectedCalls should contain not-null objects.");

            Contract.Requires(unexpectedCalls != null, "unexpectedCalls should not be null.");
            Contract.Requires(Contract.ForAll(unexpectedCalls, c => c != null),
                "unexpectedCalls should contain not-null objects.");

            Contract.Ensures(Contract.Result<string>() != null);
            throw new NotImplementedException();
        }

        public string TimesToString(int times)
        {
            Contract.Requires(times >= 0, "times should be non-negative value");
            Contract.Ensures(Contract.Result<string>() != null);

            throw new System.NotImplementedException();
        }
    }
}