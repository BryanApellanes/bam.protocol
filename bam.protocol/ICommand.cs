namespace Bam.Protocol;

public interface ICommand
{
    string TypeName { get; set; }
    string MethodName { get; set; }
    string[] Arguments { get; set; }
}