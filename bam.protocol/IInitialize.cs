using Bam.Logging;

namespace Bam.Protocol;

public interface IInitialize: ILoggable
{
    bool IsInitialized { get; }

    void Initialize();
}