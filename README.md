scurvy.test
===========

Embeddable unit testing framework for C# projects.

Game development is by its nature a visual thing. Unit testing is code that tests other code ... and code is by its nature a non-visual exercise. Ever since XNA launched, a recurring thread I've often seen how to best apply unit testing methodology. This attempts to solve that

You can also check out my blog to see what I'm up to: http://codecube.net

Introduction
------------

On the surface, Scurvy.Test has most of the basic features of a unit test framework. You can add some custom attributes to your unit test classes:
[TestClass], to signify this is a unit test class
[TestSetup], run at the beginning to initialize the entire test class
[TestPre], run before every unit test. will be in the upcoming v1.3
[TestMethod], the actual unit test
[TestPost], run after every unit test. will be in the upcoming v1.3
[TestCleanup], run after all unit tests in a test class have run

This follows suit with most any unit testing framework I've come across. Where things diverge is in the method of running the unit tests. This framework is designed to run the unit tests in-process. You can run your unit test in one of two ways:

```CSharp
TestRunner<Program>.RunTests(myServiceProvider);
```

Where the generic argument is any type in the assembly that houses your unit tests. This automatic mode will run all unit tests. Good if you want to use this framework in a standalone project (since Scurvy.Test doesn't actually have any references to XNA libraries).

Here's where things get interesting though. What if you wanted to write unit tests that use content managers, or even that render content. This is (probably) impossible with other unit test frameworks. However, Scurvy.Test enables these scenarios in a few different ways.

First, the TestRunner class can also be instantiated as a regular instance. 

```CSharp
this.runner = new TestRunner<Game1>(this.Services);
...
this.runner.Update(gameTime.ElapsedGameTime);
...
this.runner.Draw();
```

This will cause the test runner to run in manual mode. When you do that, simply call the Update and Draw methods on the test runner instance. A unit test will be run once every time the Update method is called, unless one of the unit tests specifies an ExitCriteria (more on this in a moment).

Second, a [TestMethod] can either have no arguments, or can accept a TestContext. This TestContext has two properties:
public IServiceProvider Services
public ExitCriteria ExitCriteria

The IServiceProvider is cool because if you use the "Go To Source" feature on the game class' .Services property, you'll notice it's of type GameServiceContainer, which in turn implements the IServiceProvider interface. Having a reference to this lets you instantiate ContentManagers, and also query for services like the IGraphicsDeviceService which could be used to instantiate a SpriteBatch.

The ExitCriteria is an abstract class that has the following definition:

```CSharp
public abstract class ExitCriteria
{
    public bool IsFinished;
    public readonly TestContext Context; 

    public ExitCriteria(TestContext context)
    {
        this.Context = context;
    } 

    public abstract void Update(TimeSpan elapsedTime); 

    public abstract void Draw();
}
```

With this, you can write a unit test that can render to the game's viewport and allow a QA tester to verify correctness visually. Simply set the context's ExitCriteria method from a unit test, and the framework will take care of executing the exit criteria until it sets the IsFinished property to true.

```CSharp
[TestMethod]
public void ManualContentVerificationTest(TestContext context)
{
    context.ExitCriteria = new ContentVerificationExitCriteria(this.content, context);
}
```

The sample project included in the source code has a manual verification exit criteria that will wait until the tester presses a button.

Feedback can be gathered in several different ways. If you do nothing, there is a default status reporter that will write the status to the debug stream. This can be seen in visual studio's Output window. However, this is obviously very easy to miss and not very user friendly. The latest version allows you to implement a custom status reporter, that way, if you are using this in an XNA project you can choose how you want to render the status. The sample project (which can be downloaded from the source code tab) contains examples of how to write a custom status reporter.
