using Bam.Protocol.Data.Profile.Dao;
using Bam.Protocol.Data.Profile.Dao.Repository;
using DaoPersonData = Bam.Protocol.Data.Profile.Dao.PersonData;

namespace Bam.Protocol.Server;

/// <summary>
/// Determines access levels based on the actor's group memberships and a group access configuration.
/// </summary>
public class GroupAccessLevelProvider : IAccessLevelProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupAccessLevelProvider"/> class.
    /// </summary>
    /// <param name="profileSchemaRepository">The profile schema repository for looking up person data.</param>
    /// <param name="groupAccessConfiguration">The group access configuration defining group-to-access mappings.</param>
    public GroupAccessLevelProvider(ProfileSchemaRepository profileSchemaRepository, IGroupAccessConfiguration groupAccessConfiguration)
    {
        ProfileSchemaRepository = profileSchemaRepository;
        GroupAccessConfiguration = groupAccessConfiguration;
    }

    private ProfileSchemaRepository ProfileSchemaRepository { get; }
    private IGroupAccessConfiguration GroupAccessConfiguration { get; }

    /// <summary>
    /// Gets the access level for the specified server context based on the actor's group memberships.
    /// </summary>
    /// <param name="context">The server context to evaluate.</param>
    /// <returns>The highest access level across all matching groups, or the default authenticated access.</returns>
    public BamAccess GetAccessLevel(IBamServerContext context)
    {
        if (context.Authentication == null || !context.Authentication.Success)
        {
            return BamAccess.Denied;
        }

        string actorHandle = context.Actor?.Handle!;
        if (string.IsNullOrEmpty(actorHandle))
        {
            return BamAccess.Denied;
        }

        DaoPersonData personData = DaoPersonData.FirstOneWhere(
            c => c.Handle == actorHandle,
            database: ProfileSchemaRepository.Database
        );

        if (personData == null)
        {
            return GroupAccessConfiguration.DefaultAuthenticatedAccess;
        }

        BamAccess highest = BamAccess.Denied;
        bool hasGroupMatch = false;

        foreach (var group in personData.GroupDatas)
        {
            BamAccess groupAccess = GroupAccessConfiguration.GetGroupAccess(group.Name);
            if (groupAccess > highest)
            {
                highest = groupAccess;
                hasGroupMatch = true;
            }
        }

        return hasGroupMatch ? highest : GroupAccessConfiguration.DefaultAuthenticatedAccess;
    }
}
