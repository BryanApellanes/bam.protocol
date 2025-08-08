namespace Bam.Protocol.Server;

public interface IAuthenticator
{
    BamAuthentication Authenticate(IActor actor);
}