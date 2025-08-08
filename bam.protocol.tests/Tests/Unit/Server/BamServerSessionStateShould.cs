using Bam.Protocol.Server;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamServerSessionState should")]
public class BamServerSessionStateShould : UnitTestMenuContainer
{
    [UnitTest]
    public async Task SetAndGetObjectValue()
    {
        ServerSessionState state = new ServerSessionState(null, null);
        object obj = new object();
        string name = 8.RandomLetters();
        state.Set(name, obj);
        object retrieved = state.Get(name);
        
        retrieved.ShouldBe(obj, "Retrieved object was not the object expected");
    }
    
    [UnitTest]
    public async Task SetAndGetGenericValue()
    {
        ServerSessionState state = new ServerSessionState(null, null);
        TestClass testObj = new TestClass();
        string name = 8.RandomLetters();
        state.Set<TestClass>(name, testObj);
        TestClass retrieved = state.Get<TestClass>(name);
        
        retrieved.ShouldBe(testObj, "Retrieved object was not the object expected");
        retrieved.ShouldBeOfType(typeof(TestClass));
    }

    [UnitTest]
    public async Task SaveValues()
    {
        
    }
}