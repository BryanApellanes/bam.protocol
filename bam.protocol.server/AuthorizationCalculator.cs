using System.Collections.Concurrent;
using System.Reflection;

namespace Bam.Protocol.Server;

/// <summary>
/// Calculates authorization by comparing the actor's access level against the required access for the resolved command.
/// </summary>
public class AuthorizationCalculator : IAuthorizationCalculator
{
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationCalculator"/> class.
    /// </summary>
    /// <param name="accessLevelProvider">The access level provider for determining actor access.</param>
    public AuthorizationCalculator(IAccessLevelProvider accessLevelProvider)
    {
        AccessLevelProvider = accessLevelProvider;
    }

    private IAccessLevelProvider AccessLevelProvider { get; }

    /// <summary>
    /// Calculates the authorization for the specified server context by comparing the actor's access level against the command's required access.
    /// </summary>
    /// <param name="serverContext">The server context to authorize.</param>
    /// <returns>The authorization calculation result.</returns>
    public IAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext)
    {
        ICommand command = serverContext.Command;
        if (command == null)
        {
            return Denied(serverContext, "No command resolved");
        }

        BamAccess requiredAccess = GetRequiredAccess(command);
        BamAccess actorAccess = AccessLevelProvider.GetAccessLevel(serverContext);

        if (actorAccess >= requiredAccess)
        {
            return new AuthorizationCalculation(serverContext, requiredAccess);
        }

        return Denied(serverContext, $"Actor has {actorAccess} access but {command.TypeName}.{command.MethodName} requires {requiredAccess}");
    }

    private static BamAccess GetRequiredAccess(ICommand command)
    {
        Type type = ResolveType(command.TypeName);
        if (type == null)
        {
            return BamAccess.Denied;
        }

        MethodInfo method = type.GetMethod(command.MethodName)!;
        if (method != null)
        {
            RequiredAccessAttribute methodAttr = method.GetCustomAttribute<RequiredAccessAttribute>()!;
            if (methodAttr != null)
            {
                return methodAttr.Access;
            }
        }

        RequiredAccessAttribute classAttr = type.GetCustomAttribute<RequiredAccessAttribute>()!;
        if (classAttr != null)
        {
            return classAttr.Access;
        }

        return BamAccess.Denied;
    }

    private static Type ResolveType(string typeName)
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

    private static IAuthorizationCalculation Denied(IBamServerContext serverContext, string message)
    {
        return new AuthorizationCalculation(serverContext, BamAccess.Denied)
        {
            Messages = new[] { message }
        };
    }
}
