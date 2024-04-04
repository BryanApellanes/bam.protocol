using Bam.Net.Server;
using Bam.Net.ServiceProxy;

namespace Bam.Protocol
{
    public delegate void ContentNotFoundEventHandler(IHttpResponder responder, IHttpContext context, string[] checkedPaths);
}