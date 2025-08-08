using System.Reflection;
using Bam.Console;
using Bam.DependencyInjection;
using Bam.Test;

namespace Bam.Protocol.Tests.Tests.Unit;

[UnitTestMenu("InvocationRequest should", "irs")]
public class InvocationRequestShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ClientInitialize()
    {
        Type type = typeof(TestClass);
        MethodInfo methodInfo = type.GetMethod("TestMethod");
        
        After.Setup(reg =>
        {
            TestMethodInvocationRequest request = new TestMethodInvocationRequest(methodInfo);
            reg.For<TestMethodInvocationRequest>().Use(request);
        })
        .When<TestMethodInvocationRequest>("is client initialized", (request, reg) =>
        {
            request.ClientInitialize(new ServiceRegistry());
            return request;
        })
        .TheTest
        .ShouldPass(because =>
        {
            TestMethodInvocationRequest result = because.TheResult.As<TestMethodInvocationRequest>();
            because.TheResult.IsNotNull();
            because.TheResult.Is<TestMethodInvocationRequest>();
            because.ItsTrue(
                "result.OperationIdentifier.Equals(\"Bam.Protocol.Tests.TestClass+TestMethod, bam.protocol.tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")", 
                result.OperationIdentifier.Equals("Bam.Protocol.Tests.TestClass+TestMethod, bam.protocol.tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ServerInitializeForOperationIdentifier()
    {
        Type type = typeof(TestClass);
        MethodInfo methodInfo = type.GetMethod("TestMethod");
        string operationIdentifier = OperationIdentifier.For(methodInfo);

        After.Setup(reg =>
        {
            TestMethodInvocationRequest request = new TestMethodInvocationRequest();
            request.OperationIdentifier = operationIdentifier;
            reg.For<TestMethodInvocationRequest>().Use(request);
        })
        .When<TestMethodInvocationRequest>("is server initialized", (request, reg) =>
        {
            request.ServerInitialize(new ServiceRegistry());
            return request;
        })
        .TheTest
        .ShouldPass(because =>
        {
            TestMethodInvocationRequest result = because.TheResult.As<TestMethodInvocationRequest>();
            because.ItsTrue("result was not null", result != null);
            because.ItsTrue($"OperationIdentifier = {operationIdentifier}",
                result.OperationIdentifier.Equals(operationIdentifier));
            because.ItsTrue("Instance was not null", result.GetInstance() != null);
            because.ItsTrue("Instance was of type TestClass", result.GetInstance().GetType() == typeof(TestClass));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
    
    [UnitTest]
    public void ServerInitializeForMethodInfo()
    {
        Type type = typeof(TestClass);
        MethodInfo methodInfo = type.GetMethod("TestMethod");
        string operationIdentifier = OperationIdentifier.For(methodInfo);

        TestClass instance = new TestClass() { Name = 16.RandomLetters() };
        MethodInvocationRequest request = new MethodInvocationRequest(instance, "TestMethod", "first string", "second string");
        request.ClientInitialize(new ServiceRegistry());
        
        TestMethodInvocationRequest test = request.CopyAs<TestMethodInvocationRequest>();
        Message.PrintLine(test.ToJson(true));
        test.GetInstance().ShouldBeNull();
        
        test.ServerInitialize(new ServiceRegistry());
        test.GetInstance().ShouldNotBeNull();
        test.GetInstance().ShouldBeOfType<TestClass>();
    }

    [UnitTest]
    public void ServerInitializeFromSerializedRequest()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };
        TestMethodInvocationRequest request = new TestMethodInvocationRequest()
        {
            SerializedContext = instance.ToJson(),
            OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod")
        };
        
        request.ServerInitialize(new ServiceRegistry());
        request.GetInstance().ShouldNotBeNull();
        request.GetInstance().ShouldBeOfType<TestClass>();
    }
    
    [UnitTest]
    public void ExecuteAfterServerInitialization()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };
        TestMethodInvocationRequest request = new TestMethodInvocationRequest()
        {
            SerializedContext = instance.ToJson(),
            OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod"),
            Arguments = Argument.ListForValues<TestClass>("TestMethod", "arg1", "arg2")
        };
        
        request.ServerInitialize(new ServiceRegistry());
        request.GetInstance().ShouldNotBeNull();
        request.GetInstance().ShouldBeOfType<TestClass>();

        string result = request.Invoke<string>();
        Message.PrintLine(result);
    }
    
    [UnitTest]
    public void ExecuteAfterSerializeAndDeserialize()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };
        MethodInvocationRequest request = new MethodInvocationRequest()
        {
            SerializedContext = instance.ToJson(),
            OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod"),
            Arguments = Argument.ListForValues<TestClass>("TestMethod", "arg1", "arg2")
        };

        string serialized = request.ToJson();
        Message.PrintLine(serialized);
        MethodInvocationRequest deserialized = serialized.FromJson<MethodInvocationRequest>();
        deserialized.ServerInitialize(new ServiceRegistry());

        string result = deserialized.Invoke<string>();
        string expected = $"name = {instance.Name}, argument1 = arg1, argument2 = arg2";
        result.ShouldBeEqualTo(expected);
        Message.PrintLine(result);
    }
}