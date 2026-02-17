using Bam.Logging;

namespace Bam.Protocol.Server;

/// <summary>
/// Resolves commands from BAM requests by parsing method invocation request content.
/// </summary>
public class CommandResolver : Loggable, ICommandResolver
{
    /// <summary>
    /// Resolves the command from the request content by deserializing it as a method invocation request.
    /// </summary>
    /// <param name="request">The BAM request to resolve a command from.</param>
    /// <returns>The resolved command, or null if the content is empty or cannot be parsed.</returns>
    public ICommand ResolveCommand(IBamRequest request)
    {
        string content = request.Content;
        if (string.IsNullOrEmpty(content))
        {
            return null!;
        }

        try
        {
            MethodInvocationRequest invocation = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodInvocationRequest>(content)!;
            if (invocation == null || string.IsNullOrEmpty(invocation.OperationIdentifier))
            {
                return null!;
            }

            string[] parts = invocation.OperationIdentifier.Split('+', ',');
            return new Command
            {
                TypeName = parts[0].Trim(),
                MethodName = parts[1].Trim(),
                Arguments = invocation.Arguments?.Select(a => a.Value?.ToString()!).ToArray()! ?? Array.Empty<string>()
            };
        }
        catch (Exception ex)
        {
            Log.Warn("Failed to resolve command from request content: {0}", ex.Message);
            return null!;
        }
    }
}
