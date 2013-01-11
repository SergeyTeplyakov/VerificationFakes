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
        public static T IsAny<T>()
        {
            return default(T);
        }

        public static T Is<T>(Expression<Func<T, bool>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            throw new NotImplementedException();
        }

        public static T IsInRange<T>(T from, T to) where T : IComparable
        {
            Contract.Requires(from != null, "from should not be null for value type T.");
            Contract.Requires(from != null, "to should not be null for value type T.");

            return from;
        }
    }
}