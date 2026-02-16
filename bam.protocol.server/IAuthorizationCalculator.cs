namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that calculates the authorization level for a server request context.
/// </summary>
public interface IAuthorizationCalculator
{
    /// <summary>
    /// Calculates the authorization for the specified server context.
    /// </summary>
    /// <param name="serverContext">The server context to authorize.</param>
    /// <returns>The authorization calculation result.</returns>
    IAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext);
}