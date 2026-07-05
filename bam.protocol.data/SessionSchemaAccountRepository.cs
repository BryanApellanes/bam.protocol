using Bam.Protocol;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;

namespace Bam.Protocol.Data;

public class SessionSchemaAccountRepository : IAccountRepository
{
    private readonly ServerSessionSchemaRepository _repository;

    public SessionSchemaAccountRepository(ServerSessionSchemaRepository repository)
    {
        _repository = repository;
    }

    public ServerAccountData Save(ServerAccountData data)
    {
        return (ServerAccountData)_repository.Save((object)data)!;
    }
}
