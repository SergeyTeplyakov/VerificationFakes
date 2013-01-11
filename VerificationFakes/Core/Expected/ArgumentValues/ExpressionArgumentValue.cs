using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Expected value that represents by predicate expression.
    /// </summary>
    internal sealed class ExpressionArgumentValue<T> : ArgumentValue
    {
        private readonly Expression<Func<T, bool>> _predicateExpression;
        private readonly Func<T, bool> _predicate;

        public ExpressionArgumentValue(Expression<Func<T, bool>> predicateExpression)
        {
            Contract.Requires(predicateExpression != null, "predicateExpression should not be null.");
            _predicateExpression = predicateExpression;
            _predicate = predicateExpression.Compile();
        }

        public override bool Match(object value)
        {
            return _predicate((T)value);
        }

        public override string ToString()
        {
            return _predicateExpression.ToString();
        }

        private bool Equals(ExpressionArgumentValue<T> other)
        {
            // Expressions are not support equality.
            // Lets compare string representation instead.
            return _predicateExpression.ToString().Equals(other._predicateExpression.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ExpressionArgumentValue<T> && Equals((ExpressionArgumentValue<T>) obj);
        }

        public override int GetHashCode()
        {
            return _predicateExpression.GetHashCode();
        }

        public static bool operator ==(ExpressionArgumentValue<T> left, ExpressionArgumentValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ExpressionArgumentValue<T> left, ExpressionArgumentValue<T> right)
        {
            return !Equals(left, right);
        }
    }
}