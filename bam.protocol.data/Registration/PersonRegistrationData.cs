using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class PersonRegistrationData : RepoData
{
    public string Name { get; internal set; } = null!;
    public string Handle { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
}