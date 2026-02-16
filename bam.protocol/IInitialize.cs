using Bam.Logging;

namespace Bam.Protocol;

/// <summary>
/// Defines an initializable component with initialization state tracking.
/// </summary>
public interface IInitialize: ILoggable
{
    /// <summary>
    /// Gets a value indicating whether this instance has been initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    void Initialize();
}