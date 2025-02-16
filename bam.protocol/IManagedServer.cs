namespace Bam.Server
{
    public interface IManagedServer
    {
        string ServerName { get; }
        HostBinding HttpHostBinding { get; }
        void Start();
        void Stop();

        void TryStop();
    }
}