using Bam.Console;
using Bam.DependencyInjection;
using Bam.Test;
using Newtonsoft.Json.Linq;

namespace Bam.Protocol.Tests.Tests.Unit;

[UnitTestMenu("InvocationArgumentCoercer should", "iac")]
public class InvocationArgumentCoercerShould : UnitTestMenuContainer
{
    [UnitTest]
    public void MaterializeNewtonsoftTokensNumbersAndEnumsForInvoke()
    {
        TestCoercionService instance = new TestCoercionService();

        When.A<MethodInvocationRequest>("invokes a method whose arguments arrived as JSON",
            () => new MethodInvocationRequest()
            {
                OperationIdentifier = OperationIdentifier.For<TestCoercionService>("Describe"),
                Arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["widget"] = JObject.Parse("{\"Name\":\"sprocket\",\"Count\":7,\"Tags\":[\"a\",\"b\"]}"),
                    ["times"] = new JValue(3L),
                    ["kind"] = new JValue("gizmo"),
                    ["note"] = JValue.CreateNull()
                }
            },
            (request) =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.For<TestCoercionService>().Use(instance);
                request.ServerInitialize(registry);
                return request.Invoke<string>();
            })
        .TheTest
        .ShouldPass<string>((because, _, result) =>
        {
            because.ItsTrue("the JObject became a TestWidget, the long an int, the string an enum and the JSON null a null",
                "sprocket x3 [a,b] kind=Gizmo count=7 note=<none>".Equals(result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SurviveAFullSerializeDeserializeRoundTripWithComplexArguments()
    {
        TestWidget widget = new TestWidget { Name = 8.RandomLetters(), Count = 2, Tags = new List<string> { "x" } };
        string expected = $"{widget.Name} x5 [x] kind=Gadget count=2 note=hi";

        When.A<MethodInvocationRequest>("round-trips complex arguments through JSON",
            () => new MethodInvocationRequest()
            {
                OperationIdentifier = OperationIdentifier.For<TestCoercionService>("Describe"),
                Arguments = Argument.ForValues<TestCoercionService>("Describe", widget, 5, TestWidgetKind.Gadget, "hi")
            },
            (request) =>
            {
                MethodInvocationRequest deserialized = request.ToJson().FromJson<MethodInvocationRequest>();
                deserialized.ServerInitialize(new ServiceRegistry());
                return deserialized.Invoke<string>();
            })
        .TheTest
        .ShouldPass<string>((because, _, result) =>
        {
            because.ItsTrue($"result equals '{expected}'", expected.Equals(result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ParseStringEncodedBclTypesAndConvertPrimitives()
    {
        Guid id = Guid.NewGuid();
        DateTimeOffset at = new DateTimeOffset(2026, 10, 24, 12, 0, 0, TimeSpan.FromHours(-5));
        string expected = $"{id:D}|{at:O}|9000000000|0.5|True";

        When.A<MethodInvocationRequest>("coerces strings and numbers to Guid, DateTimeOffset, long, double and bool",
            () => new MethodInvocationRequest()
            {
                OperationIdentifier = OperationIdentifier.For<TestCoercionService>("Identify"),
                Arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id"] = id.ToString("D"),
                    ["at"] = at.ToString("O"),
                    ["big"] = 9000000000L,
                    ["ratio"] = new JValue(0.5),
                    ["flag"] = "true"
                }
            },
            (request) =>
            {
                request.ServerInitialize(new ServiceRegistry());
                return request.Invoke<string>();
            })
        .TheTest
        .ShouldPass<string>((because, _, result) =>
        {
            because.ItsTrue($"result equals '{expected}'", expected.Equals(result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void MaterializeArraysAndPassAssignableValuesThrough()
    {
        When.A<MethodInvocationRequest>("coerces a JArray to int[] and leaves an int[] alone",
            () => new MethodInvocationRequest()
            {
                OperationIdentifier = OperationIdentifier.For<TestCoercionService>("Sum"),
                Arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["values"] = JArray.Parse("[1,2,3]")
                }
            },
            (request) =>
            {
                request.ServerInitialize(new ServiceRegistry());
                int fromJson = request.Invoke<int>();
                request.Arguments["values"] = new int[] { 10, 20 };
                int fromArray = request.Invoke<int>();
                return new SumOutcome(fromJson, fromArray);
            })
        .TheTest
        .ShouldPass<SumOutcome>((because, _, outcome) =>
        {
            because.ItsTrue("JArray [1,2,3] summed to 6", outcome.FromJson == 6);
            because.ItsTrue("int[] {10,20} passed through and summed to 30", outcome.FromArray == 30);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void NameTheParameterAndTypesWhenCoercionIsImpossible()
    {
        When.A<MethodInvocationRequest>("refuses an argument that cannot become the parameter type",
            () => new MethodInvocationRequest()
            {
                OperationIdentifier = OperationIdentifier.For<TestCoercionService>("Sum"),
                Arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["values"] = "not an array"
                }
            },
            (request) =>
            {
                request.ServerInitialize(new ServiceRegistry());
                try
                {
                    request.Invoke();
                    return new FailureOutcome(null, null);
                }
                catch (InvocationArgumentException ex)
                {
                    return new FailureOutcome(ex.ParamName, ex.Message);
                }
            })
        .TheTest
        .ShouldPass<FailureOutcome>((because, _, outcome) =>
        {
            because.ItsTrue("an InvocationArgumentException named the parameter", "values".Equals(outcome.ParamName));
            because.ItsTrue("the message names both types",
                outcome.Message != null && outcome.Message.Contains("System.String") && outcome.Message.Contains("System.Int32[]"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record SumOutcome(int FromJson, int FromArray);

    private sealed record FailureOutcome(string? ParamName, string? Message);
}
