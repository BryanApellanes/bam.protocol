namespace Bam.Protocol.Server;

public interface IBamServerContextInitializationHandler
{
    BamServerContextInitialization HandleInitialization(BamServerContextInitialization initialization);
}