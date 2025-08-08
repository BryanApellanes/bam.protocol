using Bam.Data.Repositories;

namespace Bam.Protocol;

public interface IActor
{
    
    /// <summary>
    /// Gets the actor handle, a user defined unique identifier.
    /// </summary>
    [CompositeKey]
    string Handle { get; }
    
    /// <summary>
    /// The display name of the actor.
    /// </summary>
    [CompositeKey]
    string Name { get; }
}