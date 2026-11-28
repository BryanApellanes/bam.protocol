namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a bounded key-set store already holds as many live key sets as it allows, so a new registration
/// is refused until one expires or is removed. Unlike the other registration refusals it says nothing about the
/// handle or its key material, so a host can answer it with 429 Too Many Requests.
/// </summary>
public class KeySetCapacityExceededException : KeySetRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetCapacityExceededException"/> class.
    /// </summary>
    /// <param name="keySetHandle">The handle whose registration was refused.</param>
    /// <param name="capacity">The store's limit on live key sets.</param>
    public KeySetCapacityExceededException(string keySetHandle, int capacity)
        : base(keySetHandle, $"The key-set store is full ({capacity} live key sets); try again once one expires.")
    {
        Capacity = capacity;
    }

    /// <summary>
    /// Gets the store's limit on live key sets.
    /// </summary>
    public int Capacity { get; }
}
