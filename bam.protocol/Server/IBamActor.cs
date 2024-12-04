using System.Security.Principal;

namespace Bam.Protocol.Server;

public interface IBamActor
{
    string UserName { get; set; }
    IPrincipal GetPrincipal();
}