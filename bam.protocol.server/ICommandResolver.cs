namespace Bam.Protocol.Server;

public interface ICommandResolver
{
    ICommand ResolveCommand(IBamRequest request);
}