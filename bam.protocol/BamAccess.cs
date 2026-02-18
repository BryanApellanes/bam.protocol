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
    /// Execute access is granted.
    /// </summary>
    Execute,
    /// <summary>
    /// Read and write access is granted.
    /// </summary>
    Write,
}