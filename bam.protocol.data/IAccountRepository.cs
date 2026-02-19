using Bam.Protocol.Data.Server;

namespace Bam.Protocol;

public interface IAccountRepository
{
    ServerAccountData Save(ServerAccountData data);
}
