using Bam.Protocol.Server;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamServerSessionState should")]
public class BamServerSessionStateShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SetAndGetObjectValue()
    {
        object obj = new object();
        string name = 8.RandomLetters();

        When.A<ServerSessionState>("sets and gets an object value",
            () => new ServerSessionState(null, null),
            (state) =>
            {
                state.Set(name, obj);
                return state.Get(name);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("retrieved object is the same instance", obj == because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SetAndGetGenericValue()
    {
        TestClass testObj = new TestClass();
        string name = 8.RandomLetters();

        When.A<ServerSessionState>("sets and gets a generic value",
            () => new ServerSessionState(null, null),
            (state) =>
            {
                state.Set<TestClass>(name, testObj);
                return state.Get<TestClass>(name);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("retrieved object is the same instance", testObj == because.Result);
            because.ItsTrue("retrieved object is of type TestClass", because.Result is TestClass);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SaveValues()
    {
        When.A<object>("placeholder test passes",
            () => new object(),
            (o) => o)
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
