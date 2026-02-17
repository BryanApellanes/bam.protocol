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
        MethodInfo methodInfo = type.GetMethod("TestMethod")!;

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
            because.TheResult.IsNotNull();
            TestMethodInvocationRequest result = because.TheResult.As<TestMethodInvocationRequest>();
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
        MethodInfo methodInfo = type.GetMethod("TestMethod")!;
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
                result!.OperationIdentifier.Equals(operationIdentifier));
            because.ItsTrue("Instance was not null", result.GetInstance() != null);
            because.ItsTrue("Instance was of type TestClass", result.GetInstance()!.GetType() == typeof(TestClass));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ServerInitializeForMethodInfo()
    {
        Type type = typeof(TestClass);
        MethodInfo methodInfo = type.GetMethod("TestMethod")!;
        string operationIdentifier = OperationIdentifier.For(methodInfo);
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };

        When.A<MethodInvocationRequest>("server initializes for method info",
            () => new MethodInvocationRequest(instance, "TestMethod", "first string", "second string"),
            (request) =>
            {
                request.ClientInitialize(new ServiceRegistry());
                TestMethodInvocationRequest test = request.CopyAs<TestMethodInvocationRequest>();
                bool instanceNullBeforeInit = test.GetInstance() == null;
                test.ServerInitialize(new ServiceRegistry());
                return new object?[] { instanceNullBeforeInit, test.GetInstance(), test.GetInstance()?.GetType() };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object?[]>("instance is null before server init", r => (bool)r[0]!)
                .As<object?[]>("instance is not null after server init", r => r[1] != null)
                .As<object?[]>("instance is of type TestClass", r => typeof(TestClass).Equals(r[2]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ServerInitializeFromSerializedRequest()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };

        When.A<TestMethodInvocationRequest>("server initializes from serialized request",
            () => new TestMethodInvocationRequest()
            {
                SerializedContext = instance.ToJson(),
                OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod")
            },
            (request) =>
            {
                request.ServerInitialize(new ServiceRegistry());
                return new object?[] { request.GetInstance(), request.GetInstance()?.GetType() };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object?[]>("instance is not null", r => r[0] != null)
                .As<object?[]>("instance is of type TestClass", r => typeof(TestClass).Equals(r[1]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ExecuteAfterServerInitialization()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };

        When.A<TestMethodInvocationRequest>("executes after server initialization",
            () => new TestMethodInvocationRequest()
            {
                SerializedContext = instance.ToJson(),
                OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod"),
                Arguments = Argument.ListForValues<TestClass>("TestMethod", "arg1", "arg2")
            },
            (request) =>
            {
                request.ServerInitialize(new ServiceRegistry());
                return new object?[] { request.GetInstance(), request.GetInstance()?.GetType(), request.Invoke<string>() };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object?[]>("instance is not null", r => r[0] != null)
                .As<object?[]>("instance is of type TestClass", r => typeof(TestClass).Equals(r[1]))
                .As<object?[]>("result is not null", r => r[2] != null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ExecuteAfterSerializeAndDeserialize()
    {
        TestClass instance = new TestClass() { Name = 16.RandomLetters() };
        string expected = $"name = {instance.Name}, argument1 = arg1, argument2 = arg2";

        When.A<MethodInvocationRequest>("executes after serialize and deserialize",
            () => new MethodInvocationRequest()
            {
                SerializedContext = instance.ToJson(),
                OperationIdentifier = OperationIdentifier.For<TestClass>("TestMethod"),
                Arguments = Argument.ListForValues<TestClass>("TestMethod", "arg1", "arg2")
            },
            (request) =>
            {
                string serialized = request.ToJson();
                MethodInvocationRequest deserialized = serialized.FromJson<MethodInvocationRequest>();
                deserialized.ServerInitialize(new ServiceRegistry());
                return deserialized.Invoke<string>();
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.As<string>("result equals expected", r => expected.Equals(r));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
