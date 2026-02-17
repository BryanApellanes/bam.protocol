namespace Bam.Protocol.Server;

/// <summary>
/// Abstract base class representing the state of a server context initialization pipeline.
/// </summary>
public abstract class BamServerInitializationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerInitializationContext"/> class.
    /// </summary>
    public BamServerInitializationContext()
    {
        CanContinue = true;
    }
    /// <summary>
    /// Gets or sets the server processing this request.
    /// </summary>
    public BamServer Server { get; set; } = null!;

    /// <summary>
    /// Gets or sets the server context being initialized.
    /// </summary>
    public IBamServerContext ServerContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the initialization pipeline can continue to the next step.
    /// </summary>
    public bool CanContinue { get; set; }

    /// <summary>
    /// Gets or sets the event arguments associated with this initialization.
    /// </summary>
    public BamServerEventArgs EventArgs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current initialization status.
    /// </summary>
    public InitializationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets a message describing the initialization result or failure reason.
    /// </summary>
    public string Message { get; set; } = null!;
}