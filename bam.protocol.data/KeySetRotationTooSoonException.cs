namespace Bam.Protocol.Data;

/// <summary>
/// Thrown when a key-set rotation is proven but comes sooner than the host's minimum interval after the handle's
/// current keys were set (bam.protocol#89). The registered key set is left unchanged. It derives from
/// <see cref="InvalidKeySetRotationException"/>, so a handler that refuses every rejected rotation still refuses this
/// one; a handler that can say when to retry catches it first and reads <see cref="RetryAfter"/>.
/// </summary>
public class KeySetRotationTooSoonException : InvalidKeySetRotationException
{
    /// <summary>Initializes a new instance of the <see cref="KeySetRotationTooSoonException"/> class.</summary>
    /// <param name="keySetHandle">The handle whose rotation was refused.</param>
    /// <param name="retryAfter">How long until the handle may rotate.</param>
    public KeySetRotationTooSoonException(string keySetHandle, TimeSpan retryAfter)
        : base(keySetHandle, $"Handle '{keySetHandle}' rotated too recently; it may rotate again in {Math.Ceiling(retryAfter.TotalSeconds)} seconds.")
    {
        RetryAfter = retryAfter;
    }

    /// <summary>Gets how long until the handle may rotate.</summary>
    public TimeSpan RetryAfter { get; }
}
