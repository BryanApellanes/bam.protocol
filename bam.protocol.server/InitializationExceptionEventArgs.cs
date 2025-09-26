namespace Bam.Protocol.Server;

public class InitializationExceptionEventArgs : EventArgs
{
    public InitializationExceptionEventArgs(Exception ex, BamServerInitializationContext initialization)
    {
        this.Exception = ex;
        this.Initialization = initialization;
    }
    
    public Exception Exception { get; set; }
    
    public BamServerInitializationContext Initialization { get; set; }
    
}