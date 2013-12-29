using System;

namespace VerificationFakes.Samples
{
    /// <summary>
    /// Similar to <see cref="ILoggerDependency"/> except that all
    /// methods and properties are sealed.
    /// </summary>
    public sealed class SealedLoggerDependency
    {
        public string GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public string GetDirectoryByLoggerName(string loggerName)
        {
            throw new NotImplementedException();
        }

        public string DefaultLogger
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler RollingRequired;

        public T GetConfigValue<T>()
        {
            throw new NotImplementedException();
        }

        public static int GetDefaultDirectoryCallsCount;

        public static string GetDefaultDirectory()
        {
            GetDefaultDirectoryCallsCount++;
            return GetDefaultDirectoryCallsCount.ToString();
        }

    }
}