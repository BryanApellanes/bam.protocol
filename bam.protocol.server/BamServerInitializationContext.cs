namespace Bam.Protocol.Server;

public abstract class BamServerInitializationContext
{
    public BamServerInitializationContext()
    {
        CanContinue = true;
    }
    public BamServer Server { get; set; }
    public IBamServerContext ServerContext { get; set; }
    public bool CanContinue { get; set; }
    public BamServerEventArgs EventArgs { get; set; }
    public InitializationStatus Status { get; set; }
    public string Message { get; set; }
}