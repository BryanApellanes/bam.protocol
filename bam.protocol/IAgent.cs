namespace Bam.Protocol.Data.Profile;

public interface IAgent : IActor
{
    IPerson Person { get; set; }
    IDevice Device { get; set; }
}