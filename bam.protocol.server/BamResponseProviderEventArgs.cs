namespace Bam.Protocol.Server;

/// <summary>
/// Provides event data for response provider events.
/// </summary>
public class BamResponseProviderEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponseProviderEventArgs"/> class.
    /// </summary>
    public BamResponseProviderEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponseProviderEventArgs"/> class with the specified provider.
    /// </summary>
    /// <param name="provider">The response provider associated with this event.</param>
    public BamResponseProviderEventArgs(BamResponseProvider provider)
    {
        this.ResponseProvider = provider;
    }
    
    /// <summary>
    /// Gets or sets the response provider associated with this event.
    /// </summary>
    public BamResponseProvider ResponseProvider { get; set; }
}