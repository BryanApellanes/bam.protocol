namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Turns the operation identifier carried by a <see cref="MethodInvocationRequest"/> into an
/// <see cref="InvocationTarget"/>, or refuses it.
/// </summary>
/// <remarks>
/// This is the trust boundary of the <c>/invoke</c> bridge. The identifier is caller-controlled text
/// (<c>Type+Method, Assembly, ...</c>), so an implementation must decide what it names by looking only at types the
/// host registered for dispatch. It must never load an assembly, scan loaded assemblies, or construct a type on the
/// strength of the identifier; <see cref="OperationIdentifier.ToMethod"/> does exactly that and is not used here.
/// </remarks>
public interface IInvocationTargetResolver
{
    /// <summary>
    /// Resolves <paramref name="operationIdentifier"/> against the registered dispatch targets.
    /// </summary>
    /// <param name="operationIdentifier">The identifier from the request envelope; may be malformed.</param>
    /// <returns>The target, or null when the identifier does not name a public method on a registered type.</returns>
    InvocationTarget? Resolve(string? operationIdentifier);
}
