using System;

namespace VerificationFakes.Samples
{
    public interface ILoggerDependency
    {
        string GetCurrentDirectory();
        string GetDirectoryByLoggerName(string loggerName);
        string DefaultLogger { get; }

        event EventHandler RollingRequired;

        T GetConfigValue<T>();

        void Foo();
    }
}