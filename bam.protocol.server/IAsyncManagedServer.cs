using System.Threading.Tasks;

namespace Bam.Server
{
    /// <summary>
    /// Extends <see cref="IManagedServer"/> with asynchronous start and stop operations.
    /// </summary>
    public interface IAsyncManagedServer : IManagedServer
    {
        /// <summary>
        /// Starts the server asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous start operation.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the server asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous stop operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Attempts to stop the server asynchronously, swallowing any exceptions.
        /// </summary>
        /// <returns>A task representing the asynchronous try-stop operation.</returns>
        Task TryStopAsync();
    }
}