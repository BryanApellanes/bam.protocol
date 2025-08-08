namespace Bam.Protocol.Server;

public interface IIdentity : IActor
{
    string PhoneNumber { get; set; }
    string EmailAddress { get; set; }
    bool IsAuthenticated { get; set; }
}