using Bam.DependencyInjection;
using Bam.Services;

namespace Bam.Protocol.Server;

public class BamServerOptions<T> : BamServerOptions where T: CommunicationHandler, new()
{
    public BamServerOptions()
    {
        this.ComponentRegistry = new ServiceRegistry();
        this.Initialize();
    }

    private ICommunicationHandler? _communicationHandler;
    public override ICommunicationHandler? GetCommunicationHandler(bool reinit = false)
    {
        if (_communicationHandler == null || reinit)
        {
            T bamCommunicationHandler = ComponentRegistry.Get<T>();
            _communicationHandler = bamCommunicationHandler;
        }

        return _communicationHandler;
    }
}