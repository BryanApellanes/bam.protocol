using System.Reflection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// A method an invocation request resolved to, together with the access rules its attributes declare.
/// </summary>
/// <remarks>
/// Produced only by an <see cref="IInvocationTargetResolver"/>, so holding one means the declaring type is in the
/// dispatch allow-list and the method is a public, non-special member that type declares itself. The access values are read
/// once here so the handler never reflects over attributes itself: <see cref="RequiredAccess"/> follows the native
/// pipeline's <c>CommandAttributeResolver</c> (method attribute, else type attribute, else
/// <see cref="BamAccess.Denied"/>), <see cref="ExplicitProof"/> is a <see cref="RequireRequestProofAttribute"/>
/// on the method or type when present, and <see cref="IsAnonymous"/> reflects an
/// <see cref="AnonymousAccessAttribute"/> that allows anonymous callers. Unlike the native resolver, attributes are
/// read without inheritance: an override that carries no attribute takes its own declaring type's level, and a
/// derived type without <see cref="RequiredAccessAttribute"/> is <see cref="BamAccess.Denied"/> whatever its base
/// declares, so "the method or the type that declares it, else Denied" is the literal rule.
/// </remarks>
/// <param name="DeclaringType">The allow-listed type the operation named (a class or an interface key in the dispatch registry), which declares <paramref name="Method"/>.</param>
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

        RequiredAccessAttribute? required = method.GetCustomAttribute<RequiredAccessAttribute>(inherit: false) ?? declaringType.GetCustomAttribute<RequiredAccessAttribute>(inherit: false);
        RequireRequestProofAttribute? proof = method.GetCustomAttribute<RequireRequestProofAttribute>(inherit: false) ?? declaringType.GetCustomAttribute<RequireRequestProofAttribute>(inherit: false);
        AnonymousAccessAttribute? anonymous = method.GetCustomAttribute<AnonymousAccessAttribute>(inherit: false) ?? declaringType.GetCustomAttribute<AnonymousAccessAttribute>(inherit: false);

        return new InvocationTarget(
            declaringType,
            method,
            required?.Access ?? BamAccess.Denied,
            proof?.Required,
            anonymous?.AllowAnonymous ?? false);
    }
}
