namespace Bam.Protocol.Server;

/// <summary>
/// Default implementation of <see cref="IGroupAccessConfiguration"/> that stores group access levels in a dictionary.
/// </summary>
public class GroupAccessConfiguration : IGroupAccessConfiguration
{
    private readonly Dictionary<string, BamAccess> _groupAccessLevels = new();

    /// <summary>
    /// Gets or sets the default access level for authenticated users not in any configured group. Defaults to Read.
    /// </summary>
    public BamAccess DefaultAuthenticatedAccess { get; set; } = BamAccess.Read;

    /// <summary>
    /// Sets the access level for the specified group.
    /// </summary>
    /// <param name="groupName">The group name.</param>
    /// <param name="access">The access level to assign.</param>
    public void SetGroupAccess(string groupName, BamAccess access)
    {
        _groupAccessLevels[groupName] = access;
    }

    /// <summary>
    /// Gets the access level for the specified group.
    /// </summary>
    /// <param name="groupName">The group name.</param>
    /// <returns>The access level for the group, or <see cref="BamAccess.Denied"/> if not configured.</returns>
    public BamAccess GetGroupAccess(string groupName)
    {
        return _groupAccessLevels.TryGetValue(groupName, out BamAccess access)
            ? access
            : BamAccess.Denied;
    }
}
