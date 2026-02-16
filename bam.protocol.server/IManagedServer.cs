namespace Bam.Server
{
    /// <summary>
    /// Defines a server that can be started, stopped, and identified by name and host binding.
    /// </summary>
    public interface IManagedServer
    {
        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the HTTP host binding for the server.
        /// </summary>
        HostBinding HttpHostBinding { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Attempts to stop the server, swallowing any exceptions.
        /// </summary>
        void TryStop();
    }
}