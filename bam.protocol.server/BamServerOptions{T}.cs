using Bam.DependencyInjection;
using Bam.Services;

namespace Bam.Protocol.Server;

/// <summary>
/// Typed server options that resolve the communication handler from the component registry using the specified type.
/// </summary>
/// <typeparam name="T">The communication handler type to use.</typeparam>
public class BamServerOptions<T> : BamServerOptions where T: CommunicationHandler, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerOptions{T}"/> class.
    /// </summary>
    public BamServerOptions()
    {
        this.ComponentRegistry = new ServiceRegistry();
        this.Initialize();
    }

    private ICommunicationHandler? _communicationHandler;
    /// <summary>
    /// Gets the communication handler, resolving it from the component registry as type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reinit">If true, forces re-resolution of the communication handler.</param>
    /// <returns>The communication handler instance, or null if not resolved.</returns>
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