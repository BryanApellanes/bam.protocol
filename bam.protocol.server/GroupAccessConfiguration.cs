namespace Bam.Protocol.Server;

public class GroupAccessConfiguration : IGroupAccessConfiguration
{
    private readonly Dictionary<string, BamAccess> _groupAccessLevels = new();

    public BamAccess DefaultAuthenticatedAccess { get; set; } = BamAccess.Read;

    public void SetGroupAccess(string groupName, BamAccess access)
    {
        _groupAccessLevels[groupName] = access;
    }

    public BamAccess GetGroupAccess(string groupName)
    {
        return _groupAccessLevels.TryGetValue(groupName, out BamAccess access)
            ? access
            : BamAccess.Denied;
    }
}
