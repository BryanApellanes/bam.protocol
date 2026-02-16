/*
    Copyright © Bryan Apellanes 2015
*/

using Bam.ServiceProxy;
using Bam.Protocol;

namespace Bam.Server
{
    /// <summary>
    /// Defines a component that responds to HTTP requests.
    /// </summary>
    public interface IHttpResponder: IInitialize
    {
        /// <summary>
        /// Gets the name of this responder.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Occurs when this responder successfully responds to a request.
        /// </summary>
        event ResponderEventHandler Responded;

        /// <summary>
        /// Occurs when this responder does not respond to a request.
        /// </summary>
        event ResponderEventHandler DidNotRespond;

        /// <summary>
        /// Occurs when the requested content is not found.
        /// </summary>
        event ContentNotFoundEventHandler ContentNotFound;

        /// <summary>
        /// Responds to the specified HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context to respond to.</param>
        /// <returns>True if the responder handled the request; otherwise, false.</returns>
        bool Respond(IHttpContext context);

        /// <summary>
        /// Attempts to respond to the specified HTTP context without throwing exceptions.
        /// </summary>
        /// <param name="context">The HTTP context to respond to.</param>
        /// <returns>True if the responder handled the request; otherwise, false.</returns>
        bool TryRespond(IHttpContext context);
    }
}