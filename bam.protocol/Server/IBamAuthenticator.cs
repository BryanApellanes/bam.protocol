namespace Bam.Protocol.Server;

public interface IBamAuthenticator
{
    BamAuthentication Authenticate(IBamActor actor);
}