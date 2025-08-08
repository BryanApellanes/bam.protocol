namespace Bam.Protocol;

public interface IOrganization
{
    string Handle { get; set; }
    string Name { get; set; }
    List<IPerson> Persons { get; set; }
}