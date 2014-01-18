### Microsoft Fakes

[Microsoft Fakes](http://msdn.microsoft.com/en-us/library/hh549175.aspx) is a great isolation framework that gives the user an ability to “fake” virtual method as well as non-virtual instance and static method. To distinguish two types of fakes Microsoft Fakes uses two different concepts – [stubs](http://msdn.microsoft.com/en-us/library/hh549174.aspx) and [shims](http://msdn.microsoft.com/en-us/library/hh549176.aspx): Stubs are uses for “faking” virtual method of the non-sealed classes or interfaces and Shims are uses for “faking” non-virtual instance method and static methods.

Many other testing frameworks support two different kind of fakes as well: stubs – for state-based testing and mocks – for behavioral testing. In this case, stubs are used to “fake” return value for a particular polymorphic method to emulate desired state for the class under test. On the other hand, mocks are used to ensure that the class under test performs some operation and calsl the dependency under certain conditions.

Unlike many other isolation frameworks, Microsoft Fakes focused primarily on state testing and provide limited support for behavioral testing. Behavioral testing using Microsoft Fakes limited by exposing `IStubObserver` property for every generated Stub. Unfortunately, this approach is tedious and cumbersome. Here is an example.
Suppose we have `Logger` class that takes `ILogWriter` as a dependency in the constructor:

```cs
public interface ILogWriter
{
    void Write(string message);
 
    void Write(int value);
}
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
```

Suppose we want to test, that `Logger.Write` method calls `ILogWriter.Write`. To test this assumption we can use following approach:

```cs
class CustomObserver : IStubObserver
{
    public string CalledMethodName;
    public object CalledMethodArgument;
 
    public void Enter(Type stubbedType, Delegate stubCall, object arg1)
    {
        CalledMethodName = stubCall.Method.Name;
        CalledMethodArgument = arg1;
    }
 
    // Other overloaded versions of the Enter method are omitted
 }
 
[Test]
public void Logger_Write_Calls_For_Enter_Method()
{
    // Arrange
    var stub = new StubILogWriter();
    var customObserver = new CustomObserver();
    // Указываем кастомный IStubObserver
    stub.InstanceObserver = customObserver;
 
    // Act
    ILogWriter logWriter = stub;
    logWriter.Write("Message");
 
    // Assert
    Assert.That(customObserver.CalledMethodName, 
        Is.StringContaining("ILogWriter.Write"));
    Assert.That(customObserver.CalledMethodArgument, 
        Is.EqualTo("Message"));
}
```

Obviously enough that current approach is too tedious and error prone and basically not far from writing mocks manually. To fill the gap in behavioral testing with Microsoft Fakes Verification Fakes project emerged.

###Verification Fakes

Verification Fakes is a lightweight wrapper around Microsoft Fakes for behavioral testing using Moq-like syntax. Therefore, if you’re familiar with [Moq](https://github.com/Moq/moq4), you’re familiar with Verification Fakes.

The easiest way to get general understanding of Microsoft Fakes is to show few examples. Suppose that we’re going to test the same Logger class that takes `ILogWriter` as a dependency. First, we should add Fake Assembly and generate stubs and shims. After generating stubs (`StubILogWriter` for `ILogWriter` interface) we would be able to get `Mock<T>` using `AsMock` or `AsStrictMock` extension methods and use it in “moq-like” fashion. 

####1.  Logger.Write(message) should call ILogWriter.Write with any argument:

```cs
var stub = new StubILogWriter();
Mock<ILogWriter> mock = stub.AsMock();
var logger = new Logger(stub);
 
logger.Write("Hello, logger!");
 
mock.Verify(lw => lw.Write(It.IsAny<string>()));
```

Now, if you’ll comment out second line with a call to the `logger.Write` or will change an implementation of the `Logger.Write` method to not call appropriate method, you’ll get VerificationException exception during a call to the `mock.Verify`:

> Expected invocation on the mock 1 times, but was 0 times.
Expected call: lw => lw.Write(It.IsAny<string>())
Actual calls:
No invocations performed.

####2. LogWriter.Write method called with a specific argument:

```cs
var logger = new Logger(stub);
 
logger.Write("Hello, logger!");
 
mock.Verify(lw => lw.Write("Hello, logger!"));
```

####3. LogWriter.Write called exactly once:

```cs
var logger = new Logger(mock.Object);
 
logger.Write("Hello, logger!");
 
mock.Verify(lw => lw.Write(It.IsAny<string>()),
    Times.Once());
```

*NOTE*
Like Moq framework, Verification Fakes support different factory methods in the Times class to specify how many times method should be called: `Times.AtLeast(int)`, `Times.AtMost(int)`, `Times.Exactly(int)`, `Times.Between(from, to)`.

####4. LogWriter.Write(int) didn’t call when calling Logger.Write(string):

```cs
var logger = new Logger(mock.Object);
 
logger.Write("Hello, logger");
 
mock.Verify(lw => lw.Write(It.IsAny<int>()), Times.Never());
```

####5. LogWriter.Write(int) called with an argument in the specified range:

```cs
var logger = new Logger(mock.Object);
 
logger.Write(42);
 
mock.Verify(lw => lw.Write(It.IsInRange(40, 50)));
```

####6. Using Setup method to specify few different expectations:

```cs
var mock = stub.AsMock();
mock.Setup(lw => lw.Write(It.IsAny<string>()));
mock.Setup(lw => lw.Write(It.IsAny<int>()));
 
var logger = new Logger(mock.Object);
 
logger.Write("Hello, logger!");
logger.Write(42);
 
mock.Verify();
```

####7. Using strict mocks:

```cs
var stub = new StubILogWriter();
var mock = stub.AsStrictMock();
mock.Setup(lw => lw.Write("Foo"));
 
var logger = new Logger(mock.Object);
 
logger.Write("Foo");
```

Now if anyone will call any method of the mocked object except Write(“foo”), VerifactionException will be thrown. For example, if we’ll manually call logger.Write(42) we’ll get following error message:

> Following invocations failed with mock behavior Strict:
VerificationFakes.Samples.ILogWriter.Write(42)
Expected invocations:
lw => lw.Write("Foo"), 1 times.

From my point of view, behavioral testing is much more rare approach in unit testing than state testing, but in some cases, it could be valuable. So if you’re using Microsoft Fakes and you need such kind of testing you have three options: 1) build mocks manually, 2) use Verification Fakes or 3) use another isolation framework. What option will you choose?

For more information about stubs and mocks, please look into the following links:

* [Martin Fowler – Mocks Aren’t Stubs](http://martinfowler.com/articles/mocksArentStubs.html)
* [xUnit Patterns – Mocks, Fakes, Stubs and Dummies](http://xunitpatterns.com/Mocks,%20Fakes,%20Stubs%20and%20Dummies.html)
* [Mocks, Stubs and Fakes: it’s a continuum](http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/)
* [Rus] [Стабы и моки](http://sergeyteplyakov.blogspot.com/2011/12/blog-post.html)

For more information about Microsoft Fakes:
* [Isolating Code Under Test with Microsoft Fakes](http://msdn.microsoft.com/en-us/library/hh549175.aspx)
* [Microsoft Fakes. Тестирование состояния](http://sergeyteplyakov.blogspot.com/2014/01/microsoft-fakes-state-verification.html)
