namespace Bam.Protocol;

/// <summary>
/// Defines access levels for authorization in the Bam protocol.
/// </summary>
public enum BamAccess
{
    /// <summary>
    /// Access is denied.
    /// </summary>
    Denied,
    /// <summary>
    /// Read-only access is granted.
    /// </summary>
    Read,
    /// <summary>
    /// Read and write access is granted.
    /// </summary>
    Write,
}