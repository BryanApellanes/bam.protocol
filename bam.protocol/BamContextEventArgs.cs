namespace Bam.Protocol;

/// <summary>
/// Provides event data containing a <see cref="BamContext"/> instance.
/// </summary>
public class BamContextEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the Bam context associated with the event.
    /// </summary>
    public BamContext Context { get; set; } = null!;
}