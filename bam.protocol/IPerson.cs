namespace Bam.Protocol;

public interface IPerson: IActor
{
    string Phone { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string MiddleName { get; set; }
}