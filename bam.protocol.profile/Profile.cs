using Bam.Encryption;

namespace Bam.Protocol.Profile;

public class Profile
{   
    public string Email { get; set; }
    public string Phone { get; set; }
    
    public IActor Actor { get; set; }
    
    /// <summary>
    /// Gets or sets this profiles public key representing the identity.
    /// </summary>
    public IPublicKey PublicKey { get; set; }
    

    
    public AesKey GetSymmetricKeyPair()
    {
        throw new NotImplementedException();
    }
}