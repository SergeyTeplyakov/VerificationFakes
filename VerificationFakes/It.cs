using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace VerificationFakes
{
    /// <summary>
    /// Allows specification of a matching condition for an argument in
    /// method invocation, rather than specific argument value.
    /// </summary>
    public static class It
    {
        /// <summary>
        /// Matches any value of the give type <typeparamref name="T"/>.
        /// </summary>
        public static T IsAny<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Mathes any value that satisfy given predicate.
        /// </summary>
        public static T Is<T>(Expression<Func<T, bool>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mathces any value that satisfy specified range.
        /// </summary>
        public static T IsInRange<T>(T from, T to) where T : IComparable
        {
            Contract.Requires(from != null, "from should not be null for value type T.");
            Contract.Requires(from != null, "to should not be null for value type T.");

            return from;
        }
    }
}