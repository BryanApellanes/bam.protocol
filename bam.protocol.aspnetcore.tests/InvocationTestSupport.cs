using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace Bam.Protocol.AspNetCore.Tests;

/// <summary>A service registered for dispatch in the invocation tests; every method requires Execute unless it says otherwise.</summary>
[RequiredAccess(BamAccess.Execute)]
public class TestInvocationService
{
    private readonly IActorContext _actors;

    public TestInvocationService(IActorContext actors)
    {
        _actors = actors;
    }

    public string Echo(string text)
    {
        return "Echo: " + text;
    }

    public int Add(int left, int right)
    {
        return left + right;
    }

    public TestInvocationReply Describe(TestInvocationPayload payload)
    {
        return new TestInvocationReply(payload.Name, payload.Count * 2);
    }

    public async Task<string> EchoAsync(string text)
    {
        await Task.Yield();
        return "Async: " + text;
    }

    public async Task DoNothingAsync()
    {
        await Task.Yield();
    }

    public string WhoAmI()
    {
        return _actors.Actor.Handle + "@" + (_actors.ClientIp ?? "unknown");
    }

    public string Fail()
    {
        throw new InvalidOperationException("secret detail that must not leak");
    }

    public string Overloaded(int value)
    {
        return value.ToString();
    }

    public string Overloaded(string value)
    {
        return value;
    }

    [RequiredAccess(BamAccess.Write)]
    public string Privileged()
    {
        return "privileged";
    }

    [AnonymousAccess]
    public string Open()
    {
        return "open";
    }

    public static string Ping()
    {
        return "pong";
    }

    public class Nested
    {
        public string Hello()
        {
            return "hello from nested";
        }
    }
}

/// <summary>A registered type carrying no access attribute at all: every method must default to Denied.</summary>
public class UnattributedService
{
    public string Hi()
    {
        return "hi";
    }
}

/// <summary>A base type that is only sometimes registered; its public method must not ride on a derived type's grant.</summary>
public class BaseService
{
    public string FromBase()
    {
        return "base";
    }
}

[RequiredAccess(BamAccess.Execute)]
public class DerivedService : BaseService
{
    public string Own()
    {
        return "own";
    }

    public string Generic<T>(T value)
    {
        return value?.ToString() ?? string.Empty;
    }
}

/// <summary>An interface-keyed registration: only the interface's attributes count.</summary>
public interface IKeyedService
{
    [RequiredAccess(BamAccess.Execute)]
    string Granted();

    string Ungranted();
}

public class KeyedService : IKeyedService
{
    public string Granted()
    {
        return "granted";
    }

    [RequiredAccess(BamAccess.Read)]
    public string Ungranted()
    {
        return "ungranted";
    }
}

/// <summary>A type whose construction is the observable event a polymorphic <c>$type</c> would trigger.</summary>
public class BoobyTrap
{
    public BoobyTrap()
    {
        throw new InvalidOperationException("BoobyTrap was instantiated");
    }
}

/// <summary>A type that is loaded but never registered for dispatch.</summary>
public class UnregisteredService
{
    public string Secret()
    {
        return "must not be reachable";
    }
}

public sealed class TestInvocationPayload
{
    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }
}

public sealed record TestInvocationReply(string Name, int Doubled);

/// <summary>The minimal actor the handler tests place on the request.</summary>
internal sealed class TestActor : IActor
{
    internal TestActor(string handle)
    {
        Handle = handle;
        Name = handle;
    }

    public string Handle { get; }

    public string Name { get; }
}

/// <summary>An access policy that answers one fixed level for every actor.</summary>
internal sealed class FixedAccessPolicy : IActorAccessPolicy
{
    private readonly BamAccess _access;

    internal FixedAccessPolicy(BamAccess access)
    {
        _access = access;
    }

    public BamAccess GetAccess(IActor actor, string? keyFingerprint)
    {
        return _access;
    }
}

/// <summary>Builds operation identifiers and invocation envelopes the way <c>BamClient&lt;T&gt;</c> does.</summary>
internal static class Invocations
{
    internal static string Operation<T>(string methodName)
    {
        return Operation(typeof(T), methodName);
    }

    internal static string Operation(Type type, string methodName)
    {
        return $"{type.FullName}+{methodName}, {type.Assembly.GetName().Name}, Version=9.9.9.9, Culture=neutral, PublicKeyToken=null";
    }

    internal static string Envelope<T>(string methodName, Dictionary<string, object?>? arguments = null)
    {
        return JsonConvert.SerializeObject(new InvocationEnvelope(Operation<T>(methodName), arguments ?? new Dictionary<string, object?>()));
    }

    internal static string Envelope(string operationIdentifier, Dictionary<string, object?>? arguments = null)
    {
        return JsonConvert.SerializeObject(new InvocationEnvelope(operationIdentifier, arguments ?? new Dictionary<string, object?>()));
    }

    internal static Dictionary<string, object?> Args(params (string Name, object? Value)[] values)
    {
        Dictionary<string, object?> arguments = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach ((string name, object? value) in values)
        {
            arguments[name] = value;
        }

        return arguments;
    }

    internal static int? StatusOf(IResult result)
    {
        return (result as IStatusCodeHttpResult)?.StatusCode;
    }

    internal static string? BodyOf(IResult result)
    {
        return result switch
        {
            ContentHttpResult content => content.ResponseContent,
            ProblemHttpResult problem => problem.ProblemDetails.Detail,
            _ => null,
        };
    }

    private sealed record InvocationEnvelope(string OperationIdentifier, Dictionary<string, object?> Arguments);
}
