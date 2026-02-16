using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents an authenticated identity with contact information, extending <see cref="IActor"/>.
/// </summary>
public interface IIdentity : IActor
{
    /// <summary>
    /// Gets or sets the phone number of the identity.
    /// </summary>
    string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the email address of the identity.
    /// </summary>
    string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this identity has been authenticated.
    /// </summary>
    bool IsAuthenticated { get; set; }
}