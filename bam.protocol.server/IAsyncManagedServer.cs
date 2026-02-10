using System.Threading.Tasks;

namespace Bam.Server
{
    public interface IAsyncManagedServer : IManagedServer
    {
        Task StartAsync();
        Task StopAsync();
        Task TryStopAsync();
    }
}