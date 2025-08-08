namespace Bam.Protocol.Profile;

public class PenOidProvider : OidProvider
{
    /// <summary>
    /// Creates an instance of PenOidProvider.
    /// </summary>
    /// <param name="pen">The private enterprise number assigned by IANA.</param>
    public PenOidProvider(string pen)
    {
        this.Pen = pen;    
    }
    
    private string Pen { get; set; }
    
    public override string GetPen()
    {
        return Pen;
    }
}