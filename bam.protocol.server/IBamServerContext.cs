/*
	Copyright © Bryan Apellanes 2022  
*/

namespace Bam.Protocol.Server
{
    public interface IBamServerContext
    {
        
        RequestType RequestType { get; set; }
        string RequestId { get; }
        IBamRequest BamRequest { get; }
        IBamResponse BamResponse { get; set; }
        
        IServerSessionState ServerSessionState { get; }
        IActor Actor { get; }
        ICommand Command { get; }
        IAuthorizationCalculation AuthorizationCalculation { get; }
        
        bool SetSessionState(IServerSessionState sessionState);
        bool SetActor(IActor actor);
        bool SetCommand(ICommand command);
        bool SetAuthorizationCalculation(IAuthorizationCalculation authorizationCalculation);
        
        void SetInitializationException(Exception exception);
    }
}