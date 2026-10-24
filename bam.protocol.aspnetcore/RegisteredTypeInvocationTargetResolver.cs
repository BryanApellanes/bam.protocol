using System.Reflection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Resolves operation identifiers strictly against the types registered in the
/// <see cref="BamInvocationDispatchRegistry"/>.
/// </summary>
/// <remarks>
/// The identifier format is the one <see cref="OperationIdentifier.For(MethodInfo)"/> writes:
/// <c>{Type.FullName}+{Method}, {AssemblyName}[, Version=..., Culture=..., PublicKeyToken=...]</c>. The last
/// <c>+</c> before the first comma separates type from method, so nested types (whose full names contain
/// <c>+</c>) resolve too. A type matches when a registered key has the same full name and its assembly has the
/// same simple name; the assembly's version, culture and token are ignored, which is what lets a client built
/// against one build of a contract assembly call a host running another. Refused: an identifier without all three
/// parts, a type that is not registered, a method that is not public, is a special-name accessor, is an open generic
/// method, is overloaded (ambiguous), or is inherited from a base type that is not itself registered (so
/// <see cref="object"/> members, and anything a registered service inherits from an unregistered base such as a
/// logging helper, stay unreachable). Static methods are allowed because their declaring type is registered. For an
/// interface-keyed registration the interface's members and attributes are what resolve; the implementation's
/// attributes are not consulted. Closed generic types are not resolvable (their full names contain commas). Nothing
/// here loads or enumerates assemblies.
/// </remarks>
public sealed class RegisteredTypeInvocationTargetResolver : IInvocationTargetResolver
{
    private const BindingFlags PublicMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    private readonly BamInvocationDispatchRegistry _targets;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisteredTypeInvocationTargetResolver"/> class.
    /// </summary>
    /// <param name="targets">The dispatch registry whose keys are the allow-list.</param>
    public RegisteredTypeInvocationTargetResolver(BamInvocationDispatchRegistry targets)
    {
        Args.ThrowIfNull(targets, nameof(targets));
        _targets = targets;
    }

    /// <inheritdoc />
    public InvocationTarget? Resolve(string? operationIdentifier)
    {
        if (!TryParse(operationIdentifier, out string typeName, out string methodName, out string assemblyName))
        {
            return null;
        }

        Type? declaringType = _targets.Registry.MappedTypes.FirstOrDefault(type =>
            string.Equals(type.FullName, typeName, StringComparison.Ordinal)
            && string.Equals(type.Assembly.GetName().Name, assemblyName, StringComparison.Ordinal));
        if (declaringType is null)
        {
            return null;
        }

        MethodInfo? method;
        try
        {
            method = declaringType.GetMethod(methodName, PublicMembers);
        }
        catch (AmbiguousMatchException)
        {
            return null;
        }

        if (method is null || method.IsSpecialName || method.ContainsGenericParameters)
        {
            return null;
        }

        if (method.DeclaringType != declaringType && !IsRegistered(method.DeclaringType))
        {
            return null;
        }

        return InvocationTarget.For(declaringType, method);
    }

    private bool IsRegistered(Type? type)
    {
        return type is not null && _targets.Registry.MappedTypes.Contains(type);
    }

    /// <summary>
    /// Splits an identifier into its type, method and assembly simple name without resolving anything.
    /// </summary>
    /// <param name="operationIdentifier">The identifier text.</param>
    /// <param name="typeName">The type's full name.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="assemblyName">The assembly's simple name.</param>
    /// <returns>True when all three parts are present and non-empty.</returns>
    public static bool TryParse(string? operationIdentifier, out string typeName, out string methodName, out string assemblyName)
    {
        typeName = string.Empty;
        methodName = string.Empty;
        assemblyName = string.Empty;
        if (string.IsNullOrWhiteSpace(operationIdentifier))
        {
            return false;
        }

        int firstComma = operationIdentifier.IndexOf(',');
        if (firstComma <= 0)
        {
            return false;
        }

        string typeAndMethod = operationIdentifier.Substring(0, firstComma);
        int methodSeparator = typeAndMethod.LastIndexOf('+');
        if (methodSeparator <= 0 || methodSeparator == typeAndMethod.Length - 1)
        {
            return false;
        }

        string[] assemblyParts = operationIdentifier.Substring(firstComma + 1).Split(',');
        string simpleName = assemblyParts[0].Trim();
        if (simpleName.Length == 0)
        {
            return false;
        }

        typeName = typeAndMethod.Substring(0, methodSeparator).Trim();
        methodName = typeAndMethod.Substring(methodSeparator + 1).Trim();
        assemblyName = simpleName;
        return typeName.Length > 0 && methodName.Length > 0;
    }
}
