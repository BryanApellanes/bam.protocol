namespace Bam.Protocol.Profile;

public interface IHandleProvider
{
    string CreateHandle(IPerson person);
    string CreateHandle(IActor actor);
    string CreateHandle(IDevice device);
    string CreateHandle(IKeySet keySet);
}