using System.Net;
using Bam.Protocol;
using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

/// <summary>
/// Default implementation of <see cref="IBamServerContext"/> that holds all request processing state.
/// </summary>
public class BamServerContext : IBamServerContext
{
    /// <summary>
    /// Gets or sets the type of request (HTTP, TCP, or UDP).
    /// </summary>
    public RequestType RequestType { get; set; }

    /// <summary>
    /// Gets or sets the HTTP listener context, if applicable.
    /// </summary>
    public HttpListenerContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the unique request identifier.
    /// </summary>
    public string RequestId { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the parsed BAM request.
    /// </summary>
    public IBamRequest BamRequest { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the BAM response.
    /// </summary>
    public IBamResponse BamResponse { get; set; } = null!;

    /// <summary>
    /// Gets or sets the output stream for writing the response.
    /// </summary>
    public Stream? OutputStream { get; set; }

    /// <summary>
    /// Gets the actor associated with this request.
    /// </summary>
    public IActor Actor { get; private set; } = null!;

    /// <summary>
    /// Gets the authentication result for this request.
    /// </summary>
    public BamAuthentication Authentication { get; private set; } = null!;

    /// <summary>
    /// Gets the resolved command for this request.
    /// </summary>
    public ICommand Command { get; private set; } = null!;

    /// <summary>
    /// Gets the server session state for this request.
    /// </summary>
    public IServerSessionState ServerSessionState { get; private set; } = null!;

    /// <summary>
    /// Gets the authorization calculation result for this request.
    /// </summary>
    public IAuthorizationCalculation AuthorizationCalculation { get; private set; } = null!;

    /// <summary>
    /// Sets the session state on this context.
    /// </summary>
    /// <param name="sessionState">The session state to set.</param>
    /// <returns>True if the session state has a valid session ID.</returns>
    public bool SetSessionState(IServerSessionState sessionState)
    {
        ServerSessionState = sessionState;
        return sessionState?.SessionId != null;
    }

    /// <summary>
    /// Sets the actor on this context.
    /// </summary>
    /// <param name="actor">The actor to set.</param>
    /// <returns>True if the actor is not null.</returns>
    public bool SetActor(IActor actor)
    {
        Actor = actor;
        return actor != null;
    }

    /// <summary>
    /// Sets the authentication result on this context.
    /// </summary>
    /// <param name="authentication">The authentication result to set.</param>
    /// <returns>True if the authentication was successful.</returns>
    public bool SetAuthentication(BamAuthentication authentication)
    {
        Authentication = authentication;
        return authentication?.Success == true;
    }

    /// <summary>
    /// Sets the resolved command on this context.
    /// </summary>
    /// <param name="command">The command to set.</param>
    /// <returns>True if the command is not null.</returns>
    public bool SetCommand(ICommand command)
    {
        Command = command;
        return command != null;
    }

    /// <summary>
    /// Sets the authorization calculation result on this context.
    /// </summary>
    /// <param name="authorizationCalculation">The authorization calculation to set.</param>
    /// <returns>True if the authorization calculation is not null.</returns>
    public bool SetAuthorizationCalculation(IAuthorizationCalculation authorizationCalculation)
    {
        AuthorizationCalculation = authorizationCalculation;
        return authorizationCalculation != null;
    }

    /// <summary>
    /// Records an exception that occurred during initialization.
    /// </summary>
    /// <param name="exception">The exception to record.</param>
    public void SetInitializationException(Exception exception)
    {
        this.InitializationException = exception;
    }

    protected Exception InitializationException { get; private set; } = null!;
}