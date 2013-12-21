namespace VerificationFakes
{
    /// <summary>
    /// Options to specify mocking validation behavior.
    /// </summary>
    public enum MockBehavior
    {
        /// <summary>
        /// Default behavior is Loose behavior.
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// Loose behavior meens that other method could be called on mocking objects as well.
        /// </summary>
        Loose = 0,
        
        /// <summary>
        /// Strict behavior restricts mocking objects from other calls except specified.
        /// </summary>
        Strict = 1,
    }
}