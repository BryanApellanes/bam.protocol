using System.Collections.Concurrent;
using System.Reflection;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides shared attribute-resolution logic for commands, with cached type lookup.
/// </summary>
public static class CommandAttributeResolver
{
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    /// <summary>
    /// Resolves a type by its fully qualified name, searching all loaded assemblies. Results are cached.
    /// </summary>
    /// <param name="typeName">The fully qualified type name.</param>
    /// <returns>The resolved type, or null if not found.</returns>
    public static Type ResolveType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null!;
        }

        return TypeCache.GetOrAdd(typeName, name =>
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(name)!;
                if (type != null)
                {
                    return type;
                }
            }
            return null!;
        })!;
    }

    /// <summary>
    /// Gets an attribute from the command's method (if present) or class (fallback).
    /// </summary>
    /// <typeparam name="T">The attribute type to retrieve.</typeparam>
    /// <param name="command">The command to inspect.</param>
    /// <returns>The attribute instance, or null if not found.</returns>
    public static T? GetAttribute<T>(ICommand command) where T : Attribute
    {
        if (command == null)
        {
            return null;
        }

        Type type = ResolveType(command.TypeName);
        if (type == null)
        {
            return null;
        }

        MethodInfo method = type.GetMethod(command.MethodName)!;
        if (method != null)
        {
            T? methodAttr = method.GetCustomAttribute<T>();
            if (methodAttr != null)
            {
                return methodAttr;
            }
        }

        return type.GetCustomAttribute<T>();
    }

    /// <summary>
    /// Gets the required access level for the specified command.
    /// </summary>
    /// <param name="command">The command to inspect.</param>
    /// <returns>The required access level, or <see cref="BamAccess.Denied"/> if no attribute is found.</returns>
    public static BamAccess GetRequiredAccess(ICommand command)
    {
        RequiredAccessAttribute? attr = GetAttribute<RequiredAccessAttribute>(command);
        return attr?.Access ?? BamAccess.Denied;
    }

    /// <summary>
    /// Determines whether anonymous access is allowed for the specified command.
    /// </summary>
    /// <param name="command">The command to inspect.</param>
    /// <returns>True if anonymous access is allowed; otherwise false.</returns>
    public static bool IsAnonymousAccessAllowed(ICommand command)
    {
        AnonymousAccessAttribute? attr = GetAttribute<AnonymousAccessAttribute>(command);
        return attr?.AllowAnonymous ?? false;
    }

    /// <summary>
    /// Determines whether encryption is required for the specified command.
    /// </summary>
    /// <param name="command">The command to inspect.</param>
    /// <returns>True if encryption is required; otherwise false.</returns>
    public static bool IsEncryptionRequired(ICommand command)
    {
        AnonymousAccessAttribute? attr = GetAttribute<AnonymousAccessAttribute>(command);
        return attr?.EncryptionRequired ?? false;
    }
}
