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

    [UnitTest]
    public void IgnorePolymorphicTypeHintsEvenUnderPoisonedDefaultSettings()
    {
        string trap = typeof(TestBoobyTrap).AssemblyQualifiedName!;
        When.A<MethodInvocationRequest>("coerces a $type-bearing object while the process default is TypeNameHandling.All",
            () => new MethodInvocationRequest(new TestCoercionService(), typeof(TestCoercionService).GetMethod("Describe")!)
            {
                Arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["widget"] = JObject.Parse("{\"$type\":\"" + trap + "\",\"Name\":\"safe\",\"Count\":1}"),
                    ["times"] = new JValue(1L),
                    ["kind"] = new JValue("gadget"),
                    ["note"] = "plain"
                }
            },
            (request) =>
            {
                Func<Newtonsoft.Json.JsonSerializerSettings>? previous = Newtonsoft.Json.JsonConvert.DefaultSettings;
                try
                {
                    Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
                    string? failure = null;
                    string? result = null;
                    try
                    {
                        result = request.Invoke<string>();
                    }
                    catch (Exception ex)
                    {
                        failure = ex.ToString();
                    }

                    return new PoisonOutcome(result, failure);
                }
                finally
                {
                    Newtonsoft.Json.JsonConvert.DefaultSettings = previous;
                }
            })
        .TheTest
        .ShouldPass<PoisonOutcome>((because, _, outcome) =>
        {
            because.ItsTrue($"the $type is ignored and the JObject becomes the declared TestWidget ({outcome.Result})", outcome.Result == "safe x1 [] kind=Gadget count=1 note=plain");
            because.ItsTrue("the named type was never instantiated", outcome.Failure is null || !outcome.Failure.Contains("TestBoobyTrap was instantiated", StringComparison.Ordinal));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static string Describe(Dictionary<string, object?> arguments)
    {
        MethodInvocationRequest request = new MethodInvocationRequest(new TestCoercionService(), typeof(TestCoercionService).GetMethod("Describe")!)
        {
            Arguments = arguments
        };
        return request.Invoke<string>();
    }

    private static string? Refusal(Dictionary<string, object?> arguments)
    {
        try
        {
            Describe(arguments);
            return null;
        }
        catch (InvocationArgumentException ex)
        {
            return ex.ParamName + ": " + ex.Message;
        }
    }

    private static InvocationArgumentException? RefusalOf(Dictionary<string, object?> arguments)
    {
        try
        {
            Describe(arguments);
            return null;
        }
        catch (InvocationArgumentException ex)
        {
            return ex;
        }
    }

    private static string Identify(Dictionary<string, object?> arguments)
    {
        MethodInvocationRequest request = new MethodInvocationRequest(new TestCoercionService(), typeof(TestCoercionService).GetMethod("Identify")!)
        {
            Arguments = arguments
        };
        return request.Invoke<string>();
    }

    private static string Measure(Dictionary<string, object?> arguments)
    {
        MethodInvocationRequest request = new MethodInvocationRequest(new TestCoercionService(), typeof(TestCoercionService).GetMethod("Measure")!)
        {
            Arguments = arguments
        };
        return request.Invoke<string>();
    }

    private static string? IdentifyRefusal(Dictionary<string, object?> arguments)
    {
        try
        {
            Identify(arguments);
            return null;
        }
        catch (InvocationArgumentException ex)
        {
            return ex.ParamName + ": " + ex.Message;
        }
    }

    private static Dictionary<string, object?> Arguments(params (string Name, object? Value)[] values)
    {
        Dictionary<string, object?> arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach ((string name, object? value) in values)
        {
            arguments[name] = value;
        }

        return arguments;
    }

    [UnitTest]
    public void MaterializeSystemTextJsonElementsLikeNewtonsoftTokens()
    {
        using System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse("{\"widget\":{\"name\":\"sprocket\",\"count\":7,\"tags\":[\"a\",\"b\"]},\"times\":3,\"kind\":\"gizmo\",\"note\":null}");
        System.Text.Json.JsonElement root = document.RootElement;
        When.A<TestCoercionService>("invokes Describe with JsonElement arguments",
            () => new TestCoercionService(),
            (_) => Describe(Arguments(
                ("widget", root.GetProperty("widget").Clone()),
                ("times", root.GetProperty("times").Clone()),
                ("kind", root.GetProperty("kind").Clone()),
                ("note", root.GetProperty("note").Clone()))))
        .TheTest
        .ShouldPass<string>((because, _, result) =>
        {
            because.ItsTrue($"a camelCase JsonElement becomes a TestWidget, a number an int, a string an enum and a JSON null a null ({result})",
                "sprocket x3 [a,b] kind=Gizmo count=7 note=<none>".Equals(result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void KeepStringParametersHonestAndRoundTripDates()
    {
        DateTime stamp = new DateTime(2026, 10, 24, 12, 0, 0, DateTimeKind.Utc);
        JObject widget = JObject.Parse("{\"Name\":\"w\",\"Count\":1}");
        When.A<TestCoercionService>("coerces string parameters from text, dates, primitives and refuses objects",
            () => new TestCoercionService(),
            (_) => new StringOutcome(
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", stamp))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", new DateTimeOffset(stamp)))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", new JValue(stamp)))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", 42L))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", new JValue(true)))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", new TestWidget { Name = "object" }))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", DBNull.Value))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", JObject.Parse("{\"a\":1}"))))))
        .TheTest
        .ShouldPass<StringOutcome>((because, _, outcome) =>
        {
            because.ItsTrue($"a DateTime that the envelope reader produced is written back in round-trip form ({outcome.FromDateTime})", outcome.FromDateTime.EndsWith("note=2026-10-24T12:00:00.0000000Z", StringComparison.Ordinal));
            because.ItsTrue($"a DateTimeOffset likewise ({outcome.FromDateTimeOffset})", outcome.FromDateTimeOffset.EndsWith("note=2026-10-24T12:00:00.0000000+00:00", StringComparison.Ordinal));
            because.ItsTrue($"a date token the reader produced likewise ({outcome.FromDateToken})", outcome.FromDateToken.EndsWith("note=2026-10-24T12:00:00.0000000Z", StringComparison.Ordinal));
            because.ItsTrue($"a primitive becomes its invariant text ({outcome.FromNumber})", outcome.FromNumber.EndsWith("note=42", StringComparison.Ordinal));
            because.ItsTrue($"a scalar JValue becomes its text ({outcome.FromJValue})", outcome.FromJValue.EndsWith("note=True", StringComparison.Ordinal));
            because.ItsTrue($"an arbitrary object is refused naming the parameter ({outcome.ObjectRefusal})", outcome.ObjectRefusal?.StartsWith("note: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"DBNull is refused ({outcome.DbNullRefusal})", outcome.DbNullRefusal?.Contains("System.DBNull", StringComparison.Ordinal) == true);
            because.ItsTrue($"a JObject is refused for a string parameter ({outcome.JObjectRefusal})", outcome.JObjectRefusal?.StartsWith("note: ", StringComparison.Ordinal) == true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectUndefinedEnumValuesAndFractionalIntegers()
    {
        JObject widget = JObject.Parse("{\"Name\":\"w\",\"Count\":1}");
        When.A<TestCoercionService>("refuses enum values the enum does not define and non-integral numbers for int",
            () => new TestCoercionService(),
            (_) => new StrictOutcome(
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", 99L))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", "99"))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", new JValue(99)))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", "Gadget, Gizmo"))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", 2L))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", new JValue("GIZMO")))),
                Refusal(Arguments(("widget", widget), ("times", 3.7d), ("kind", "gadget"))),
                Refusal(Arguments(("widget", widget), ("times", new JValue(3.7d)), ("kind", "gadget"))),
                Describe(Arguments(("widget", widget), ("times", new JValue(3.0d)), ("kind", "gadget"))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", 2.5d))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", true))),
                Refusal(Arguments(("widget", widget), ("times", 123456789012345.67d), ("kind", "gadget"))),
                Refusal(Arguments(("widget", widget), ("times", true), ("kind", "gadget")))))
        .TheTest
        .ShouldPass<StrictOutcome>((because, _, outcome) =>
        {
            because.ItsTrue($"an undefined numeric enum value is refused ({outcome.UndefinedNumber})", outcome.UndefinedNumber?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"an undefined numeric string is refused ({outcome.UndefinedText})", outcome.UndefinedText?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"an undefined JValue number is refused ({outcome.UndefinedToken})", outcome.UndefinedToken?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a combined name on a non-flags enum is refused ({outcome.CombinedNames})", outcome.CombinedNames?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a defined numeric value still coerces ({outcome.DefinedNumber})", outcome.DefinedNumber.Contains("kind=Gizmo", StringComparison.Ordinal));
            because.ItsTrue($"a defined name in any case still coerces ({outcome.DefinedName})", outcome.DefinedName.Contains("kind=Gizmo", StringComparison.Ordinal));
            because.ItsTrue($"a fractional double for an int is refused ({outcome.FractionDouble})", outcome.FractionDouble?.StartsWith("times: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a fractional JValue for an int is refused ({outcome.FractionToken})", outcome.FractionToken?.StartsWith("times: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a whole double for an int coerces ({outcome.WholeToken})", outcome.WholeToken.Contains(" x3 ", StringComparison.Ordinal));
            because.ItsTrue($"a fractional number for an enum is refused rather than rounded ({outcome.EnumFraction})", outcome.EnumFraction?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a boolean for an enum is refused ({outcome.EnumBool})", outcome.EnumBool?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a fraction beyond fifteen significant digits is still refused ({outcome.WideFraction})", outcome.WideFraction?.StartsWith("times: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a boolean for an int is refused ({outcome.IntBool})", outcome.IntBool?.StartsWith("times: ", StringComparison.Ordinal) == true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DistinguishMissingOptionalExplicitNullAndMissingRequired()
    {
        JObject widget = JObject.Parse("{\"Name\":\"w\",\"Count\":1}");
        When.A<TestCoercionService>("applies declared defaults for absent keys, default(T) for explicit nulls, and refuses an absent required parameter",
            () => new TestCoercionService(),
            (_) =>
            {
                string absentOptional = Describe(Arguments(("widget", widget), ("times", 2L), ("kind", "gadget")));
                string explicitNullOptional = Describe(Arguments(("widget", widget), ("times", 2L), ("kind", "gadget"), ("note", null)));
                string explicitNullInt = Describe(Arguments(("widget", widget), ("times", null), ("kind", "gadget")));
                string? missingRequired = Refusal(Arguments(("widget", widget), ("kind", "gadget")));

                string? underSupplied;
                try
                {
                    MethodInvocationRequest request = MethodInvocationRequest.For(typeof(TestCoercionService), "Describe", new TestWidget { Name = "w" });
                    request.ServerInitialize(new ServiceRegistry());
                    request.Invoke();
                    underSupplied = null;
                }
                catch (InvocationArgumentException ex)
                {
                    underSupplied = ex.ParamName + ": " + ex.Message;
                }

                return new MissingOutcome(absentOptional, explicitNullOptional, explicitNullInt, missingRequired, underSupplied);
            })
        .TheTest
        .ShouldPass<MissingOutcome>((because, _, outcome) =>
        {
            because.ItsTrue($"an absent optional parameter takes its declared default ({outcome.AbsentOptional})", outcome.AbsentOptional.EndsWith("note=<none>", StringComparison.Ordinal));
            because.ItsTrue($"an explicit null for an optional parameter is null ({outcome.ExplicitNullOptional})", outcome.ExplicitNullOptional.EndsWith("note=<none>", StringComparison.Ordinal));
            because.ItsTrue($"an explicit null for an int is default(int) ({outcome.ExplicitNullInt})", outcome.ExplicitNullInt.Contains(" x0 ", StringComparison.Ordinal));
            because.ItsTrue($"an absent required parameter is refused by name ({outcome.MissingRequired})", outcome.MissingRequired?.StartsWith("times: ", StringComparison.Ordinal) == true && outcome.MissingRequired.Contains("required", StringComparison.Ordinal));
            because.ItsTrue($"an under-supplied For(...) request is refused by name ({outcome.UnderSupplied})", outcome.UnderSupplied?.StartsWith("times: ", StringComparison.Ordinal) == true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void TreatScalarTokensFromBothReadersAlike()
    {
        using System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse("{\"i\":99,\"s7\":\"7\",\"n7\":7,\"t\":true,\"e\":1e2,\"f\":2.5,\"s42\":\"42\",\"dec\":123456789.123456789,\"ul\":18446744073709551615,\"big\":12345678901234567890,\"tiny\":1e-30,\"small\":2.5e-28}");
        System.Text.Json.JsonElement root = document.RootElement;
        JObject widget = JObject.Parse("{\"Name\":\"w\",\"Count\":1}");
        Guid id = Guid.NewGuid();
        DateTimeOffset at = new DateTimeOffset(2026, 10, 24, 12, 0, 0, TimeSpan.Zero);
        When.A<TestCoercionService>("reduces scalar JsonElements and JValues to CLR values before applying the scalar rules",
            () => new TestCoercionService(),
            (_) => new ScalarOutcome(
                RefusalOf(Arguments(("widget", widget), ("times", 1L), ("kind", root.GetProperty("i").Clone())))?.ActualType,
                Describe(Arguments(("widget", widget), ("times", root.GetProperty("s7").Clone()), ("kind", "gadget"))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", root.GetProperty("n7").Clone()))),
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", root.GetProperty("t").Clone()))),
                Describe(Arguments(("widget", widget), ("times", root.GetProperty("e").Clone()), ("kind", "gadget"))),
                Refusal(Arguments(("widget", widget), ("times", 1L), ("kind", root.GetProperty("f").Clone()))),
                Identify(Arguments(("id", id.ToString()), ("at", at.ToString("O")), ("big", root.GetProperty("s42").Clone()), ("ratio", "1.5"), ("flag", "true"))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", "1,5"), ("flag", true))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", new JValue("1,5")), ("flag", true))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", "1,000"), ("ratio", 1.5d), ("flag", true))),
                Measure(Arguments(("amount", root.GetProperty("dec").Clone()), ("count", root.GetProperty("ul").Clone()))),
                Measure(Arguments(("amount", root.GetProperty("big").Clone()), ("count", 1L))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", 1.5d), ("flag", root.GetProperty("n7").Clone()))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", root.GetProperty("t").Clone()), ("flag", true))),
                IdentifyRefusal(Arguments(("id", id), ("at", at), ("big", true), ("ratio", 1.5d), ("flag", true))),
                Refusal(Arguments(("widget", widget), ("times", "1e-30"), ("kind", "gadget"))),
                Describe(Arguments(("widget", widget), ("times", "0e5"), ("kind", "gadget"))),
                Identify(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", root.GetProperty("tiny").Clone()), ("flag", true))),
                Identify(Arguments(("id", id), ("at", at), ("big", 1L), ("ratio", root.GetProperty("small").Clone()), ("flag", true))),
                RefusalOf(Arguments(("widget", widget), ("times", root.GetProperty("tiny").Clone()), ("kind", "gadget")))?.ActualType,
                Describe(Arguments(("widget", widget), ("times", 1L), ("kind", "gadget"), ("note", new JValue(System.Numerics.BigInteger.Parse("18446744073709551615")))))))
        .TheTest
        .ShouldPass<ScalarOutcome>((because, _, outcome) =>
        {
            because.ItsTrue($"a refused JSON integer is reported as the long it was, not a double ({outcome.IntegerActualType})", outcome.IntegerActualType == typeof(long));
            because.ItsTrue($"numeric text in a JsonElement becomes an int ({outcome.TextToInt})", outcome.TextToInt.Contains(" x7 ", StringComparison.Ordinal));
            because.ItsTrue($"a JSON number becomes a string's invariant text ({outcome.NumberToString})", outcome.NumberToString.EndsWith("note=7", StringComparison.Ordinal));
            because.ItsTrue($"a JSON boolean becomes a string ({outcome.BoolToString})", outcome.BoolToString.EndsWith("note=True", StringComparison.Ordinal));
            because.ItsTrue($"an exponent-form whole number becomes an int ({outcome.ExponentToInt})", outcome.ExponentToInt.Contains(" x100 ", StringComparison.Ordinal));
            because.ItsTrue($"a fractional JSON number for an enum is refused ({outcome.FractionToEnum})", outcome.FractionToEnum?.StartsWith("kind: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"text forms of Guid, DateTimeOffset, long, double and bool parse on the element path too ({outcome.TextForms})", outcome.TextForms == $"{id:D}|{at:O}|42|1.5|True");
            because.ItsTrue($"a group separator in numeric text is refused rather than dropped ({outcome.GroupedDouble})", outcome.GroupedDouble?.StartsWith("ratio: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"the same through a JValue ({outcome.GroupedToken})", outcome.GroupedToken?.StartsWith("ratio: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"and for an integral target ({outcome.GroupedLong})", outcome.GroupedLong?.StartsWith("big: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a JSON number keeps every digit for a decimal parameter and fills the ulong range ({outcome.ExactDigits})", outcome.ExactDigits == "123456789.123456789|18446744073709551615");
            because.ItsTrue($"a whole number past long.MaxValue binds to decimal exactly ({outcome.ExactWide})", outcome.ExactWide == "12345678901234567890|1");
            because.ItsTrue($"a JSON number for a bool is refused ({outcome.NumberToBool})", outcome.NumberToBool?.StartsWith("flag: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a JSON boolean for a double is refused ({outcome.BoolToDouble})", outcome.BoolToDouble?.StartsWith("ratio: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"a CLR boolean for a long is refused ({outcome.BoolToLong})", outcome.BoolToLong?.StartsWith("big: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"numeric text that decimal rounds to zero is refused for an int ({outcome.TinyText})", outcome.TinyText?.StartsWith("times: ", StringComparison.Ordinal) == true);
            because.ItsTrue($"exponent-form zero text is a real zero ({outcome.ZeroExponent})", outcome.ZeroExponent.Contains(" x0 ", StringComparison.Ordinal));
            because.ItsTrue($"a JSON number below decimal's floor reaches a double parameter exactly ({outcome.TinyToDouble})", outcome.TinyToDouble.Contains("|1E-30|", StringComparison.Ordinal));
            because.ItsTrue($"a JSON number decimal would round reaches a double parameter exactly ({outcome.SmallToDouble})", outcome.SmallToDouble.Contains("|2.5E-28|", StringComparison.Ordinal));
            because.ItsTrue($"a JSON number below decimal's floor is refused for an int as the double it is ({outcome.TinyToInt})", outcome.TinyToInt == typeof(double));
            because.ItsTrue($"a Newtonsoft BigInteger becomes a string's digits ({outcome.BigIntegerToString})", outcome.BigIntegerToString.EndsWith("note=18446744073709551615", StringComparison.Ordinal));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record ScalarOutcome(Type? IntegerActualType, string TextToInt, string NumberToString, string BoolToString, string ExponentToInt, string? FractionToEnum, string TextForms, string? GroupedDouble, string? GroupedToken, string? GroupedLong, string ExactDigits, string ExactWide, string? NumberToBool, string? BoolToDouble, string? BoolToLong, string? TinyText, string ZeroExponent, string TinyToDouble, string SmallToDouble, Type? TinyToInt, string BigIntegerToString);

    private sealed record StringOutcome(string FromDateTime, string FromDateTimeOffset, string FromDateToken, string FromNumber, string FromJValue, string? ObjectRefusal, string? DbNullRefusal, string? JObjectRefusal);

    private sealed record StrictOutcome(string? UndefinedNumber, string? UndefinedText, string? UndefinedToken, string? CombinedNames, string DefinedNumber, string DefinedName, string? FractionDouble, string? FractionToken, string WholeToken, string? EnumFraction, string? EnumBool, string? WideFraction, string? IntBool);

    private sealed record MissingOutcome(string AbsentOptional, string ExplicitNullOptional, string ExplicitNullInt, string? MissingRequired, string? UnderSupplied);

    private sealed record SumOutcome(int FromJson, int FromArray);

    private sealed record PoisonOutcome(string? Result, string? Failure);

    private sealed record FailureOutcome(string? ParamName, string? Message);
}
