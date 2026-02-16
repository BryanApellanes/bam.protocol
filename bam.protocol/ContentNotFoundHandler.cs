using Bam.Server;
using Bam.ServiceProxy;

namespace Bam.Protocol
{
    /// <summary>
    /// Delegate for handling events when content is not found by an HTTP responder.
    /// </summary>
    /// <param name="responder">The responder that could not find the content.</param>
    /// <param name="context">The HTTP context of the request.</param>
    /// <param name="checkedPaths">The file paths that were checked for the content.</param>
    public delegate void ContentNotFoundEventHandler(IHttpResponder responder, IHttpContext context, string[] checkedPaths);
}