namespace Bam.Protocol.Server;

public interface IGroupAccessConfiguration
{
    BamAccess DefaultAuthenticatedAccess { get; set; }
    void SetGroupAccess(string groupName, BamAccess access);
    BamAccess GetGroupAccess(string groupName);
}
