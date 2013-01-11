using System.Collections;
using System.Diagnostics.Contracts;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Models expected range values for behavior testing.
    /// </summary>
    internal sealed class RangeArgumentValue : ArgumentValue 
    {
        private readonly object _left;
        private readonly object _right;

        public RangeArgumentValue(object left, object right)
        {
            Contract.Requires(left != null);
            Contract.Requires(right != null);

            _left = left;
            _right = right;
        }

        /// <summary>
        /// Returns true if specified <paramref name="value"/> lays between
        /// left and right bounds.
        /// </summary>
        public override bool Match(object value)
        {
            var comparer = Comparer.DefaultInvariant;

            return comparer.Compare(_left, value) <= 0 && comparer.Compare(value, _right) <= 0;
        }

        private bool Equals(RangeArgumentValue other)
        {
            return Equals(_left, other._left) && Equals(_right, other._right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RangeArgumentValue && Equals((RangeArgumentValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _left.GetHashCode()*397 ^ _right.GetHashCode();
            }
        }
    }
}