using System.Diagnostics.Contracts;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Models any expected value for behavior testing.
    /// </summary>
    internal sealed class AnyArgumentValue : ArgumentValue
    {
        /// <summary>
        /// Any <paramref name="value"/> will match to this object
        /// </summary>
        /// <returns>Always returns true.</returns>
        public override bool Match(object value)
        {
            Contract.Ensures(Contract.Result<bool>() == true);
            return true;
        }

        private bool Equals(AnyArgumentValue other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AnyArgumentValue && Equals((AnyArgumentValue) obj);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
}