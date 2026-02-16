namespace Bam.Protocol;

/// <summary>
/// Defines a typed initializable component that raises events during initialization.
/// </summary>
/// <typeparam name="T">The type of the initialization context.</typeparam>
public interface IInitialize<T>: IInitialize
{
    /// <summary>
    /// Occurs before initialization begins.
    /// </summary>
    event Action<T> Initializing;

    /// <summary>
    /// Occurs after initialization completes.
    /// </summary>
    event Action<T> Initialized;
}