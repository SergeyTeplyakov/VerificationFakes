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
        
        /// <summary>
        /// Returns string representation of the Times object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} times", _range) ;
        }

        private bool Equals(Times other)
        {
            return Equals(_range, other._range);
        }

        /// <summary>
        /// Returns true if <paramref name="obj"/> is of type <see cref="Times"/> and
        /// two instances are equals.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Times && Equals((Times) obj);
        }

        /// <summary>
        /// Returns true if <paramref name="left"/> and <paramref name="right"/> instances are equals.
        /// </summary>
        public static bool operator ==(Times left, Times right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns true if <paramref name="left"/> and <paramref name="right"/> instance
        /// are not equals.
        /// </summary>
        public static bool operator !=(Times left, Times right)
        {
            return !Equals(left, right);
        }
        
        /// <summary>
        /// Returns hash code of the instance.
        /// </summary>
        public override int GetHashCode()
        {
            return _range.GetHashCode();
        }

        /// <summary>
        /// Represents expected calls count when expected method should be called exactly once.
        /// </summary>
        public static Times Once()
        {
            Contract.Ensures(Contract.Result<Times>() != null,
                             "Resulting object should not be null.");
            Contract.Ensures(Contract.Result<Times>().Match(1),
                             "Resulting Times object should match to 1.");

            return Exactly(1);
        }

        /// <summary>
        /// Represents expected calls count when expected method should be called at 
        /// least <paramref name="times"/> times.
        /// </summary>
        public static Times AtLeast(int times)
        {
            Contract.Requires(times >= 0);
            Contract.Ensures(Contract.Result<Times>() != null,
                "Resulting object should not be null.");

            var range = RangeBuilder.From(times).To(int.MaxValue).Value;
            return new Times(range);
        }

        /// <summary>
        /// Represents expected calls count when expected method should be called exactly
        /// <paramref name="times"/> times.
        /// </summary>
        public static Times Exactly(int times)
        {
            Contract.Requires(times >= 0);
            Contract.Ensures(Contract.Result<Times>() != null,
                "Resulting object should not be null.");

            var range = RangeBuilder.From(times).To(times).Value;
            return new Times(range);
        }

        /// <summary>
        /// Represents expected calls count when expected method should never be called.
        /// </summary>
        /// <returns></returns>
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