using Bam.Server;
using Bam.ServiceProxy;

namespace Bam.Protocol
{
    public delegate void ContentNotFoundEventHandler(IHttpResponder responder, IHttpContext context, string[] checkedPaths);
}