using System.Reflection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// A method an invocation request resolved to, together with the access rules its attributes declare.
/// </summary>
/// <remarks>
/// Produced only by an <see cref="IInvocationTargetResolver"/>, so holding one means the declaring type was
/// registered for dispatch and the method is a public, non-special member of that type. The access values are read
/// once here so the handler never reflects over attributes itself: <see cref="RequiredAccess"/> follows the native
/// pipeline's <c>CommandAttributeResolver</c> (method attribute, else type attribute, else
/// <see cref="BamAccess.Denied"/>), <see cref="ExplicitProof"/> is a <see cref="RequireRequestProofAttribute"/>
/// on the method or type when present, and <see cref="IsAnonymous"/> reflects an
/// <see cref="AnonymousAccessAttribute"/> that allows anonymous callers.
/// </remarks>
/// <param name="DeclaringType">The registered type the operation named (a class or an interface key in the dispatch registry).</param>
/// <param name="Method">The public method to invoke.</param>
/// <param name="RequiredAccess">The access the caller must hold.</param>
/// <param name="ExplicitProof">The explicit body-proof requirement, or null to derive it from <paramref name="RequiredAccess"/>.</param>
/// <param name="IsAnonymous">True when the method or its type allows anonymous callers.</param>
public sealed record InvocationTarget(Type DeclaringType, MethodInfo Method, BamAccess RequiredAccess, bool? ExplicitProof, bool IsAnonymous)
{
    /// <summary>
    /// Reads the access rules off <paramref name="method"/> and <paramref name="declaringType"/>.
    /// </summary>
    /// <param name="declaringType">The registered type the operation named.</param>
    /// <param name="method">The resolved method.</param>
    /// <returns>The target with its rules populated.</returns>
    public static InvocationTarget For(Type declaringType, MethodInfo method)
    {
        Args.ThrowIfNull(declaringType, nameof(declaringType));
        Args.ThrowIfNull(method, nameof(method));

        RequiredAccessAttribute? required = method.GetCustomAttribute<RequiredAccessAttribute>() ?? declaringType.GetCustomAttribute<RequiredAccessAttribute>();
        RequireRequestProofAttribute? proof = method.GetCustomAttribute<RequireRequestProofAttribute>() ?? declaringType.GetCustomAttribute<RequireRequestProofAttribute>();
        AnonymousAccessAttribute? anonymous = method.GetCustomAttribute<AnonymousAccessAttribute>() ?? declaringType.GetCustomAttribute<AnonymousAccessAttribute>();

        return new InvocationTarget(
            declaringType,
            method,
            required?.Access ?? BamAccess.Denied,
            proof?.Required,
            anonymous?.AllowAnonymous ?? false);
    }
}
