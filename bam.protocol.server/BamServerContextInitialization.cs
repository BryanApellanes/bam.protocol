namespace Bam.Protocol.Server;

public abstract class BamServerContextInitialization
{
    public BamServerContextInitialization()
    {
        CanContinue = true;
    }
    public BamServer Server { get; set; }
    public IBamServerContext ServerContext { get; internal set; }
    public bool CanContinue { get; internal set; }
    public BamServerEventArgs EventArgs { get; internal set; }
    public InitializationStatus Status { get; internal set; }
    public string Message { get; internal set; }
}