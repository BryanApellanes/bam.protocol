/*
	Copyright © Bryan Apellanes 2022  
*/

using System.Net;
using Bam.Protocol;
using Bam.Protocol.Data;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// Represents the full context of a BAM server request, including request/response, session, actor, authentication, command, and authorization state.
    /// </summary>
    public interface IBamServerContext
    {
        /// <summary>
        /// Gets or sets the type of request (HTTP, TCP, or UDP).
        /// </summary>
        RequestType RequestType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP listener context, if the request is HTTP-based.
        /// </summary>
        HttpListenerContext? HttpContext { get; set; }

        /// <summary>
        /// Gets the unique identifier for this request.
        /// </summary>
        string RequestId { get; }

        /// <summary>
        /// Gets the parsed BAM request.
        /// </summary>
        IBamRequest BamRequest { get; }

        /// <summary>
        /// Gets or sets the BAM response.
        /// </summary>
        IBamResponse BamResponse { get; set; }

        /// <summary>
        /// Gets or sets the output stream for writing the response.
        /// </summary>
        Stream? OutputStream { get; set; }

        /// <summary>
        /// Gets the server session state for this request.
        /// </summary>
        IServerSessionState ServerSessionState { get; }

        /// <summary>
        /// Gets the actor associated with this request.
        /// </summary>
        IActor Actor { get; }

        /// <summary>
        /// Gets the resolved command for this request.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// Gets the authentication result for this request.
        /// </summary>
        BamAuthentication Authentication { get; }

        /// <summary>
        /// Gets the authorization calculation result for this request.
        /// </summary>
        IAuthorizationCalculation AuthorizationCalculation { get; }

        /// <summary>
        /// Sets the session state on this context.
        /// </summary>
        /// <param name="sessionState">The session state to set.</param>
        /// <returns>True if the session state was set successfully.</returns>
        bool SetSessionState(IServerSessionState sessionState);

        /// <summary>
        /// Sets the actor on this context.
        /// </summary>
        /// <param name="actor">The actor to set.</param>
        /// <returns>True if the actor was set successfully.</returns>
        bool SetActor(IActor actor);

        /// <summary>
        /// Sets the authentication result on this context.
        /// </summary>
        /// <param name="authentication">The authentication result to set.</param>
        /// <returns>True if the authentication was successful.</returns>
        bool SetAuthentication(BamAuthentication authentication);

        /// <summary>
        /// Sets the resolved command on this context.
        /// </summary>
        /// <param name="command">The command to set.</param>
        /// <returns>True if the command was set successfully.</returns>
        bool SetCommand(ICommand command);

        /// <summary>
        /// Sets the authorization calculation result on this context.
        /// </summary>
        /// <param name="authorizationCalculation">The authorization calculation to set.</param>
        /// <returns>True if the authorization calculation was set successfully.</returns>
        bool SetAuthorizationCalculation(IAuthorizationCalculation authorizationCalculation);

        /// <summary>
        /// Records an exception that occurred during initialization.
        /// </summary>
        /// <param name="exception">The exception to record.</param>
        void SetInitializationException(Exception exception);
    }
}