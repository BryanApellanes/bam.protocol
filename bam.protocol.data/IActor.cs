using System.Text.Json.Serialization;
using Bam.Data.Repositories;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol;

[JsonConverter(typeof(InterfaceTypeConverter<IActor, ActorData>))]
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