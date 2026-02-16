namespace Bam.Protocol.Client;

/// <summary>
/// Represents a strongly-typed response received from a Bam server.
/// </summary>
/// <typeparam name="T">The type of the response content.</typeparam>
public interface IBamClientResponse<T> : IBamClientResponse
{

}