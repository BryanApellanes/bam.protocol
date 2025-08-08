namespace Bam.Protocol.Server;

public interface IIdentityResolver
{
    IIdentity ResolveIdentity(IBamServerContext serverContext);
}