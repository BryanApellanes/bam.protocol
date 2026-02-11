using Bam.Protocol.Data.Profile.Dao;
using Bam.Protocol.Data.Profile.Dao.Repository;
using DaoPersonData = Bam.Protocol.Data.Profile.Dao.PersonData;

namespace Bam.Protocol.Server;

public class GroupAccessLevelProvider : IAccessLevelProvider
{
    public GroupAccessLevelProvider(ProfileSchemaRepository profileSchemaRepository, IGroupAccessConfiguration groupAccessConfiguration)
    {
        ProfileSchemaRepository = profileSchemaRepository;
        GroupAccessConfiguration = groupAccessConfiguration;
    }

    private ProfileSchemaRepository ProfileSchemaRepository { get; }
    private IGroupAccessConfiguration GroupAccessConfiguration { get; }

    public BamAccess GetAccessLevel(IBamServerContext context)
    {
        if (context.Authentication == null || !context.Authentication.Success)
        {
            return BamAccess.Denied;
        }

        string actorHandle = context.Actor?.Handle;
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
