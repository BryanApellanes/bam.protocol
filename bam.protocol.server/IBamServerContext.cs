/*
	Copyright © Bryan Apellanes 2022  
*/

using System.Net;
using Bam.Protocol;
using Bam.Protocol.Data;

namespace Bam.Protocol.Server
{
    public interface IBamServerContext
    {
        
        RequestType RequestType { get; set; }
        HttpListenerContext? HttpContext { get; set; }
        string RequestId { get; }
        IBamRequest BamRequest { get; }
        IBamResponse BamResponse { get; set; }
        
        IServerSessionState ServerSessionState { get; }
        IActor Actor { get; }
        ICommand Command { get; }
        BamAuthentication Authentication { get; }
        IAuthorizationCalculation AuthorizationCalculation { get; }

        bool SetSessionState(IServerSessionState sessionState);
        bool SetActor(IActor actor);
        bool SetAuthentication(BamAuthentication authentication);
        bool SetCommand(ICommand command);
        bool SetAuthorizationCalculation(IAuthorizationCalculation authorizationCalculation);
        
        void SetInitializationException(Exception exception);
    }
}