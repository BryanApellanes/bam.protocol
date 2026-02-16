namespace Bam.Protocol.Server;

/// <summary>
/// Provides event data when an exception occurs during server context initialization.
/// </summary>
public class InitializationExceptionEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="initialization">The initialization context at the time of the exception.</param>
    public InitializationExceptionEventArgs(Exception ex, BamServerInitializationContext initialization)
    {
        this.Exception = ex;
        this.Initialization = initialization;
    }
    
    /// <summary>
    /// Gets or sets the exception that occurred during initialization.
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// Gets or sets the initialization context at the time of the exception.
    /// </summary>
    public BamServerInitializationContext Initialization { get; set; }
    
}