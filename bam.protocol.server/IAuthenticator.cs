namespace Bam.Protocol.Server;

public interface IAuthenticator
{
    BamAuthentication Authenticate(IBamServerContext serverContext);
}
