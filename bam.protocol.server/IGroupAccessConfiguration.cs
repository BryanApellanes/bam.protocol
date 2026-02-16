namespace Bam.Protocol.Server;

/// <summary>
/// Defines configuration for group-based access control, mapping group names to access levels.
/// </summary>
public interface IGroupAccessConfiguration
{
    /// <summary>
    /// Gets or sets the default access level for authenticated users not in any configured group.
    /// </summary>
    BamAccess DefaultAuthenticatedAccess { get; set; }

    /// <summary>
    /// Sets the access level for the specified group.
    /// </summary>
    /// <param name="groupName">The group name.</param>
    /// <param name="access">The access level to assign.</param>
    void SetGroupAccess(string groupName, BamAccess access);

    /// <summary>
    /// Gets the access level for the specified group.
    /// </summary>
    /// <param name="groupName">The group name.</param>
    /// <returns>The access level for the group, or <see cref="BamAccess.Denied"/> if not configured.</returns>
    BamAccess GetGroupAccess(string groupName);
}
