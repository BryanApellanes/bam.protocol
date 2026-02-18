namespace Bam.Protocol;

/// <summary>
/// Specifies that the decorated class or method allows anonymous (unauthenticated) access.
/// Method-level attributes override class-level attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AnonymousAccessAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousAccessAttribute"/> class.
    /// </summary>
    /// <param name="allowAnonymous">Whether anonymous access is allowed. Defaults to true.</param>
    public AnonymousAccessAttribute(bool allowAnonymous = true)
    {
        AllowAnonymous = allowAnonymous;
    }

    /// <summary>
    /// Gets a value indicating whether anonymous access is allowed.
    /// </summary>
    public bool AllowAnonymous { get; }
}
