using Bam.DependencyInjection;
using Bam.Services;

namespace Bam.Protocol.Server;

public class BamServerOptions<T> :BamServerOptions where T: BamCommunicationHandler, new()
{
    public BamServerOptions()
    {
        this.ComponentRegistry = new ServiceRegistry();
        this.Initialize();
    }

    private IBamCommunicationHandler? _communicationHandler;
    public override IBamCommunicationHandler? GetCommunicationHandler(bool reinit = false)
    {
        if (_communicationHandler == null || reinit)
        {
            T bamCommunicationHandler = ComponentRegistry.Get<T>();
            _communicationHandler = bamCommunicationHandler;
        }

        return _communicationHandler;
    }
}