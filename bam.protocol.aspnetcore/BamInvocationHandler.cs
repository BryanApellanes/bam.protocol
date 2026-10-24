using System.Reflection;
using System.Text;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Serves <c>POST /invoke</c>: runs a <see cref="MethodInvocationRequest"/> against a registered type on behalf of
/// the actor the middleware verified.
/// </summary>
/// <remarks>
/// The handler is the same three steps <c>BamRequestProcessor</c> performs in the native pipeline (deserialize,
/// bind the instance, invoke) placed behind actor authentication and made safe for caller-controlled input. In
/// order: the actor must be present and not the anonymous sentinel (401, the middleware's uniform message, before
/// any body is read); the body is read under <see cref="BamInvocationOptions.MaxBodyBytes"/> (413); the envelope is
/// deserialized (400); the operation is resolved by <see cref="IInvocationTargetResolver"/> against registered types
/// only (461, the native <c>CommandResolutionFailed</c> code); anonymous-marked operations are refused (403, the
/// endpoint is not an anonymous surface); the actor's access from <see cref="IActorAccessPolicy"/> must satisfy the target's
/// <see cref="InvocationTarget.RequiredAccess"/> (403); the body proof is verified by the registered
/// <see cref="RequestProofEvaluator"/> when <see cref="InvocationAccessRule.RequiresProof"/> says so (401); the
/// instance comes from the dispatch registry inside an <see cref="AmbientActorContext"/> scope (461 when it
/// refuses); a <see cref="Task"/> result is awaited and unwrapped; and the result is
/// written as bare JSON with 200, which is what <c>BamClient&lt;T&gt;.InvokeAsync</c> deserializes. An
/// <see cref="InvocationArgumentException"/> is the caller's fault (400); any other exception from the target is a
/// server fault answered with a generic 500 and logged in full on the host only.
/// </remarks>
public sealed class BamInvocationHandler
{
    /// <summary>The status answered when an operation does not resolve to a registered public method (the native <c>CommandResolutionFailed</c> code).</summary>
    public const int OperationNotFoundStatus = 461;

    /// <summary>The generic body for a target that threw; the exception itself is only logged.</summary>
    public const string InvocationFailedMessage = "Invocation failed.";

    /// <summary>The longest operation identifier echoed into a log message; longer ones are cut.</summary>
    public const int LoggedOperationLength = 256;

    /// <summary>
    /// Envelope parsing is pinned to no polymorphic type handling so a host's <see cref="JsonConvert.DefaultSettings"/>
    /// can never re-enable <c>$type</c> on caller-controlled JSON.
    /// </summary>
    private static readonly JsonSerializerSettings EnvelopeSettings = new JsonSerializerSettings
    {
        MaxDepth = 64,
        TypeNameHandling = TypeNameHandling.None,
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore
    };

    private readonly IInvocationTargetResolver _resolver;
    private readonly BamInvocationDispatchRegistry _targets;
    private readonly IActorAccessPolicy _policy;
    private readonly RequestProofEvaluator _proofs;
    private readonly ActorAuthenticationOptions _authentication;
    private readonly BamInvocationOptions _options;
    private readonly ILogger<BamInvocationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BamInvocationHandler"/> class.
    /// </summary>
    /// <param name="resolver">Resolves operation identifiers against registered types.</param>
    /// <param name="targets">The dispatch registry that supplies service instances.</param>
    /// <param name="policy">Maps the verified actor to its access level.</param>
    /// <param name="proofs">Verifies the per-request body proof.</param>
    /// <param name="authentication">The actor-auth options (proof threshold).</param>
    /// <param name="options">The endpoint options (body cap).</param>
    /// <param name="logger">Receives refusals, denials and target failures.</param>
    public BamInvocationHandler(
        IInvocationTargetResolver resolver,
        BamInvocationDispatchRegistry targets,
        IActorAccessPolicy policy,
        RequestProofEvaluator proofs,
        ActorAuthenticationOptions authentication,
        BamInvocationOptions options,
        ILogger<BamInvocationHandler>? logger = null)
    {
        Args.ThrowIfNull(resolver, nameof(resolver));
        Args.ThrowIfNull(targets, nameof(targets));
        Args.ThrowIfNull(policy, nameof(policy));
        Args.ThrowIfNull(proofs, nameof(proofs));
        Args.ThrowIfNull(authentication, nameof(authentication));
        Args.ThrowIfNull(options, nameof(options));
        _resolver = resolver;
        _targets = targets;
        _policy = policy;
        _proofs = proofs;
        _authentication = authentication;
        _options = options;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<BamInvocationHandler>.Instance;
    }

    /// <summary>
    /// Handles one invocation request.
    /// </summary>
    /// <param name="http">The request context; the middleware has already run.</param>
    /// <returns>The result to write.</returns>
    /// <remarks>
    /// The actor check comes first so a request without a verified actor is answered before any of its body is
    /// read, parsed or resolved: nothing about the registered surface is learnable without credentials.
    /// </remarks>
    public async Task<IResult> HandleAsync(HttpContext http)
    {
        Args.ThrowIfNull(http, nameof(http));

        IActor? actor = http.GetActor();
        if (actor is null || string.Equals(actor.Handle, AnonymousActorProvider.AnonymousHandle, StringComparison.Ordinal))
        {
            return Results.Text(ActorAuthenticationMiddleware.FailureMessage, statusCode: StatusCodes.Status401Unauthorized);
        }

        string? eccPublicKeyPem = http.GetActorEccPublicKeyPem();
        string? fingerprint = eccPublicKeyPem is null ? null : PublicKeyFingerprint.Of(eccPublicKeyPem);

        string? body = await ReadBodyAsync(http.Request).ConfigureAwait(false);
        if (body is null)
        {
            return Results.Problem(detail: $"Request body exceeds {_options.MaxBodyBytes} bytes.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        MethodInvocationRequest? envelope = Deserialize(body);
        if (envelope is null || string.IsNullOrWhiteSpace(envelope.OperationIdentifier))
        {
            return Results.Problem(detail: "The request body is not a method invocation.", statusCode: StatusCodes.Status400BadRequest);
        }

        string operation = Describe(envelope.OperationIdentifier);
        InvocationTarget? target = _resolver.Resolve(envelope.OperationIdentifier);
        if (target is null)
        {
            _logger.LogInformation("Invocation refused: '{Operation}' does not name a registered public method.", operation);
            return Results.Problem(detail: "The operation is not available.", statusCode: OperationNotFoundStatus);
        }

        if (target.IsAnonymous)
        {
            _logger.LogWarning("Invocation refused: '{Operation}' allows anonymous access, which /invoke does not serve.", operation);
            return Results.Problem(detail: "Anonymous operations are not served by this endpoint.", statusCode: StatusCodes.Status403Forbidden);
        }

        BamAccess held = _policy.GetAccess(actor, fingerprint);
        if (!InvocationAccessRule.IsAllowed(held, target.RequiredAccess))
        {
            _logger.LogWarning("Invocation denied: actor '{Handle}' holds {Held} but '{Operation}' requires {Required}.", actor.Handle, held, operation, target.RequiredAccess);
            return Results.Problem(detail: $"Actor '{actor.Handle}' has {held} access but the operation requires {target.RequiredAccess}.", statusCode: StatusCodes.Status403Forbidden);
        }

        if (InvocationAccessRule.RequiresProof(target, _authentication))
        {
            RequestProofResult proof = await _proofs.EvaluateAsync(http.Request, eccPublicKeyPem).ConfigureAwait(false);
            if (!proof.Success)
            {
                _logger.LogWarning("Invocation refused: request proof failed for actor '{Handle}' on '{Operation}': {Reason}", actor.Handle, operation, proof.Reason);
                return Results.Problem(detail: proof.Reason ?? "Request proof failed.", statusCode: StatusCodes.Status401Unauthorized);
            }
        }

        ActorInvocationContext context = new ActorInvocationContext(actor, fingerprint, http.Connection.RemoteIpAddress?.ToString());
        string json;
        try
        {
            using (AmbientActorContext.Enter(context))
            {
                // The instance is created inside the scope so a service may read IActorContext in its constructor.
                object? instance = null;
                if (!target.Method.IsStatic)
                {
                    try
                    {
                        instance = _targets.Registry.Get(target.DeclaringType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Invocation refused: the dispatch registry would not supply '{Type}'.", target.DeclaringType.FullName);
                        return Results.Problem(detail: "The operation is not available.", statusCode: OperationNotFoundStatus);
                    }
                }

                MethodInvocationRequest call = new MethodInvocationRequest(instance!, target.Method)
                {
                    Arguments = envelope.Arguments ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                };

                object? result = await UnwrapAsync(call.Invoke()).ConfigureAwait(false);
                json = JsonConvert.SerializeObject(result);
            }
        }
        catch (InvocationArgumentException ex)
        {
            _logger.LogInformation(ex, "Invocation rejected: argument '{Parameter}' on '{Operation}' could not be converted.", ex.ParamName, operation);
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (OperationCanceledException) when (http.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Invocation of '{Operation}' by actor '{Handle}' was abandoned: the client disconnected.", operation, actor.Handle);
            return Results.Problem(detail: InvocationFailedMessage, statusCode: StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            Exception cause = ex is TargetInvocationException { InnerException: not null } wrapped ? wrapped.InnerException! : ex;
            _logger.LogError(cause, "Invocation of '{Operation}' by actor '{Handle}' threw.", operation, actor.Handle);
            return Results.Problem(detail: InvocationFailedMessage, statusCode: StatusCodes.Status500InternalServerError);
        }

        _logger.LogDebug("Invocation of '{Operation}' by actor '{Handle}' succeeded.", operation, actor.Handle);
        return Results.Content(json, "application/json", Encoding.UTF8, StatusCodes.Status200OK);
    }

    /// <summary>
    /// Cuts a caller-supplied operation identifier down to <see cref="LoggedOperationLength"/> characters for logging.
    /// </summary>
    /// <param name="operationIdentifier">The identifier from the envelope.</param>
    /// <returns>The identifier, or its first <see cref="LoggedOperationLength"/> characters followed by an ellipsis.</returns>
    public static string Describe(string operationIdentifier)
    {
        Args.ThrowIfNull(operationIdentifier, nameof(operationIdentifier));
        return operationIdentifier.Length <= LoggedOperationLength ? operationIdentifier : operationIdentifier.Substring(0, LoggedOperationLength) + "\u2026";
    }

    /// <summary>
    /// Awaits a <see cref="Task"/> or <see cref="Task{TResult}"/> returned by the target and yields its value; any
    /// other result is returned as is.
    /// </summary>
    /// <remarks>
    /// An <c>async Task</c> method returns a <c>Task&lt;VoidTaskResult&gt;</c> at runtime, so a generic task whose
    /// result type is the runtime's internal <c>VoidTaskResult</c> yields null rather than an empty object.
    /// </remarks>
    /// <param name="result">The raw return value of the method.</param>
    /// <returns>The value to serialize.</returns>
    public static async Task<object?> UnwrapAsync(object? result)
    {
        if (result is not Task task)
        {
            return result;
        }

        await task.ConfigureAwait(false);
        Type taskType = task.GetType();
        if (taskType.IsGenericType)
        {
            PropertyInfo? resultProperty = taskType.GetProperty("Result");
            if (resultProperty is not null && !string.Equals(resultProperty.PropertyType.Name, "VoidTaskResult", StringComparison.Ordinal))
            {
                return resultProperty.GetValue(task);
            }
        }

        return null;
    }

    private async Task<string?> ReadBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is long declared && declared > _options.MaxBodyBytes)
        {
            return null;
        }

        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;
        using MemoryStream buffer = new MemoryStream();
        byte[] chunk = new byte[16 * 1024];
        int read;
        while ((read = await request.Body.ReadAsync(chunk, 0, chunk.Length).ConfigureAwait(false)) > 0)
        {
            if (buffer.Length + read > _options.MaxBodyBytes)
            {
                return null;
            }

            buffer.Write(chunk, 0, read);
        }

        request.Body.Position = 0;
        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private MethodInvocationRequest? Deserialize(string body)
    {
        try
        {
            return JsonConvert.DeserializeObject<MethodInvocationRequest>(body, EnvelopeSettings);
        }
        catch (JsonException ex)
        {
            _logger.LogInformation(ex, "Invocation rejected: the body is not a method invocation envelope.");
            return null;
        }
    }
}
