namespace FakeMockTests.Expected
{
    interface ILogger
    {
        void Write();

        void Write(int i);
        
        void Write(int i, double d);
        
        void Write(double d);

        void Write(string s);

        int Read();

        int Read(int i);

        int Read(double d);

        int Read(string s);

        string DummyProperty { get; set; }
    }
}