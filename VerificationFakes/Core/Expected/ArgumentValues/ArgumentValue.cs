namespace VerificationFakes.Core
{
    /// <summary>
    /// Base abstract class for modeling values expected by verification test.
    /// </summary>
    internal abstract class ArgumentValue
    {
        /// <summary>
        /// Return true if specified <paramref name="value"/> matches to the current
        /// expected value.
        /// </summary>
        public abstract bool Match(object value);
    }
}