using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class PersonRegistrationData : RepoData
{
    public string Name { get; internal set; }
    public string Handle { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
}