namespace Bam.Protocol;

public interface IGroup
{
    string Name { get; set; }
    string Description { get; set; }
    List<IPerson> Persons { get; set; }
}