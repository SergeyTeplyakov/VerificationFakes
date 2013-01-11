using System.Diagnostics.Contracts;
using VerificationFakes.Core;

namespace VerificationFakes
{
    /// <summary>
    /// Defines the number of invocations allowed by a mocked method.
    /// </summary>
    public sealed class Times
    {
        private readonly Range _range;

        private Times(Range range)
        {
            Contract.Requires(range != null);

            _range = range;
        }

        [Pure]
        internal bool Match(int value)
        {
            return _range.Contains(value);
        }

        public override string ToString()
        {
            return string.Format("{0} times", _range) ;
        }

        private bool Equals(Times other)
        {
            return Equals(_range, other._range);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Times && Equals((Times) obj);
        }

        public static bool operator ==(Times left, Times right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Times left, Times right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return _range.GetHashCode();
        }

        public static Times Once()
        {
            Contract.Ensures(Contract.Result<Times>() != null,
                             "Resulting object should not be null.");
            Contract.Ensures(Contract.Result<Times>().Match(1),
                             "Resulting Times object should match to 1.");

            return Exactly(1);
        }

        public static Times AtLeast(int times)
        {
            Contract.Ensures(Contract.Result<Times>() != null,
                "Resulting object should not be null.");

            var range = RangeBuilder.From(times).To(int.MaxValue).Value;
            return new Times(range);
        }

        public static Times Exactly(int times)
        {
            Contract.Ensures(Contract.Result<Times>() != null,
                "Resulting object should not be null.");

            var range = RangeBuilder.From(times).To(times).Value;
            return new Times(range);
        }

        public static Times Never()
        {
            Contract.Ensures(Contract.Result<Times>() != null,
                "Resulting object should not be null.");
            Contract.Ensures(Contract.Result<Times>().Match(0),
                "Resulting Times object should match to 0.");

            var range = RangeBuilder.From(0).To(0).Value;
            return new Times(range);
        }
    }
}