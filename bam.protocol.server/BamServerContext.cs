using System.Net;
using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

public class BamServerContext : IBamServerContext
{
    public RequestType RequestType { get; set; }
    public HttpListenerContext? HttpContext { get; set; }
    public string RequestId { get; internal set; }
    public IBamRequest BamRequest { get; internal set; }
    public IBamResponse BamResponse { get; set; }
    public IActor Actor { get; private set; }
    public ICommand Command { get; private set; }
    public IServerSessionState ServerSessionState { get; private set; }
    public IAuthorizationCalculation AuthorizationCalculation { get; private set; }
    public bool SetSessionState(IServerSessionState sessionState)
    {
        ServerSessionState = sessionState;
        return sessionState?.SessionId != null;
    }

    public bool SetActor(IActor actor)
    {
        Actor = actor;
        return actor != null;
    }

    public bool SetCommand(ICommand command)
    {
        Command = command;
        return command != null;
    }

    public bool SetAuthorizationCalculation(IAuthorizationCalculation authorizationCalculation)
    {
        AuthorizationCalculation = authorizationCalculation;
        return authorizationCalculation != null;
    }

    public void SetInitializationException(Exception exception)
    {
        this.InitializationException = exception;
    }

    protected Exception InitializationException { get; private set; }
}