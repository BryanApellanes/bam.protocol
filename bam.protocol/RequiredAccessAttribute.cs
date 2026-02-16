namespace Bam.Protocol;

/// <summary>
/// Specifies the minimum access level required to invoke the decorated class or method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiredAccessAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredAccessAttribute"/> class with the specified access level.
    /// </summary>
    /// <param name="access">The minimum access level required.</param>
    public RequiredAccessAttribute(BamAccess access)
    {
        Access = access;
    }

    /// <summary>
    /// Gets the minimum access level required.
    /// </summary>
    public BamAccess Access { get; }
}
