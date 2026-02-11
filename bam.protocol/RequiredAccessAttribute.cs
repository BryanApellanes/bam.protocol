namespace Bam.Protocol;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiredAccessAttribute : Attribute
{
    public RequiredAccessAttribute(BamAccess access)
    {
        Access = access;
    }

    public BamAccess Access { get; }
}
