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
/// <c>+</c>) resolve too. A type matches when a type in the frozen allow-list
/// (<see cref="BamInvocationDispatchRegistry.InvocableTypes"/>) has the same full name and its assembly has the
/// same simple name; the assembly's version, culture and token are ignored, which is what lets a client built
/// against one build of a contract assembly call a host running another. Refused: an identifier without all three
/// parts, a type outside the allow-list, a method that is not public, is a special-name accessor, is an open generic
/// method, is overloaded (ambiguous), or is not declared by the named type itself. Inherited members are never
/// reachable through a derived type's name, so <see cref="object"/> members and anything a service inherits from a
/// base (a logging helper, say) stay unreachable; a base type's own methods are invocable only under the base type's
/// name, with the base type's own rules, and only when the base is in the allow-list. That costs the real client
/// nothing: <see cref="OperationIdentifier.For(MethodInfo)"/> writes the declaring type. Static methods are allowed.
/// For an interface-keyed registration the interface's members and attributes are what resolve; the implementation's
/// attributes are not consulted. Closed generic types are not resolvable (their full names contain commas). Nothing
/// here loads or enumerates assemblies, and nothing here reads the registry's live type list.
/// </remarks>
public sealed class RegisteredTypeInvocationTargetResolver : IInvocationTargetResolver
{
    private const BindingFlags PublicMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    private readonly BamInvocationDispatchRegistry _targets;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisteredTypeInvocationTargetResolver"/> class.
    /// </summary>
    /// <param name="targets">The dispatch registry whose frozen <see cref="BamInvocationDispatchRegistry.InvocableTypes"/> is the allow-list.</param>
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

        Type? declaringType = _targets.InvocableTypes.FirstOrDefault(type =>
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

        if (method.DeclaringType != declaringType)
        {
            return null;
        }

        return InvocationTarget.For(declaringType, method);
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
