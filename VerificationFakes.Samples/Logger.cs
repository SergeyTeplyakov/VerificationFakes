namespace VerificationFakes.Samples
{
    public class Logger
    {
        private readonly ILogWriter _logWriter;

        public Logger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void Write(string message)
        {
            _logWriter.Write(message);
        }

        public void Write(int value)
        {
            _logWriter.Write(value);
        }
    }
}