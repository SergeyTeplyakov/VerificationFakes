namespace VerificationFakes.Samples
{
    public interface ILogWriter
    {
        void Write(string message);

        void Write(int value);

        void Write(int value1, string value2);

        int WriteR(int value);
        int WriteR(string message);

        int WriteR(int value1, string value2);
    }
}