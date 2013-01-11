namespace VerificationFakes.Core
{
    /// <summary>
    /// Models single expected value for behavior testing.
    /// </summary>
    internal sealed class SingleArgumentValue : ArgumentValue
    {
        private readonly object _value;

        public SingleArgumentValue(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns true if specified <paramref name="value"/> is equals to
        /// expected value.
        /// </summary>
        public override bool Match(object value)
        {
            return _value.Equals(value);
        }

        private bool Equals(SingleArgumentValue other)
        {
            return Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SingleArgumentValue && Equals((SingleArgumentValue) obj);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }
    }
}