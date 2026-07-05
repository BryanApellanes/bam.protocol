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
    /// <param name="encryptionRequired">Whether the request body must be encrypted. Defaults to false.</param>
    public AnonymousAccessAttribute(bool allowAnonymous = true, bool encryptionRequired = false)
    {
        AllowAnonymous = allowAnonymous;
        EncryptionRequired = encryptionRequired;
    }

    /// <summary>
    /// Gets a value indicating whether anonymous access is allowed.
    /// </summary>
    public bool AllowAnonymous { get; }

    /// <summary>
    /// Gets a value indicating whether encryption is required for the anonymous request.
    /// </summary>
    public bool EncryptionRequired { get; }
}
