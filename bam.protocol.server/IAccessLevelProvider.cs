namespace Bam.Protocol.Server;

public interface IAccessLevelProvider
{
    BamAccess GetAccessLevel(IBamServerContext context);
}
