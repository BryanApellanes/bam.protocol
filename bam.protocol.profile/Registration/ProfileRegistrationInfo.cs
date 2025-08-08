namespace Bam.Protocol.Profile.Registration;

public class ProfileRegistrationInfo : IPerson
{
    public string Handle { get; }
    public string Name { get; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
}