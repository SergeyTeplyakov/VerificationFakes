using System;

namespace VerificationFakes
{
    /// <summary>
    /// Exception thrown by mocks when setups are not matched,
    /// the mock is not properly setup, etc.
    /// </summary>
    public sealed class VerificationException : Exception
    {
        public VerificationException(string message) : base(message)
        {}
    }
}