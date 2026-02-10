namespace Bam.Protocol;

public class Command : ICommand
{
    public string TypeName { get; set; }
    public string MethodName { get; set; }
    public string[] Arguments { get; set; }
}
