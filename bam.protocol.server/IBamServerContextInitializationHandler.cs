namespace Bam.Protocol.Server;

public interface IBamServerContextInitializationHandler
{
    BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization);
}