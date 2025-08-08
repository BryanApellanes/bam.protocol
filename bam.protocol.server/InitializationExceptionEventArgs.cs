namespace Bam.Protocol.Server;

public class InitializationExceptionEventArgs : EventArgs
{
    public InitializationExceptionEventArgs(Exception ex, BamServerContextInitialization initialization)
    {
        this.Exception = ex;
        this.Initialization = initialization;
    }
    
    public Exception Exception { get; set; }
    
    public BamServerContextInitialization Initialization { get; set; }
    
}