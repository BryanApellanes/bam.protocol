using Bam.Protocol.Data;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Represents the result of an authorization calculation for a server request.
/// </summary>
public class AuthorizationCalculation : IAuthorizationCalculation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationCalculation"/> class.
    /// </summary>
    /// <param name="context">The server context for the current request.</param>
    /// <param name="access">The calculated access level.</param>
    public AuthorizationCalculation(IBamServerContext context, BamAccess access)
    {
        this.Context = context;
        this.Access = access;
    }
    
    private IBamServerContext Context { get; set; }
    
    /// <summary>
    /// Gets or sets the messages associated with the authorization calculation.
    /// </summary>
    public string[] Messages { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the calculated access level.
    /// </summary>
    public BamAccess Access { get; internal set; }

    /// <summary>
    /// Gets the request associated with this authorization calculation.
    /// </summary>
    public IBamRequest Request => Context.BamRequest;

    /// <summary>
    /// Gets the response associated with this authorization calculation.
    /// </summary>
    public IBamResponse Response => Context.BamResponse;

    /// <summary>
    /// Gets the actor associated with this authorization calculation.
    /// </summary>
    public IActor Actor => Context.Actor;
}