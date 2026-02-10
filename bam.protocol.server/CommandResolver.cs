using Bam.Logging;

namespace Bam.Protocol.Server;

public class CommandResolver : Loggable, ICommandResolver
{
    public ICommand ResolveCommand(IBamRequest request)
    {
        string content = request.Content;
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        try
        {
            MethodInvocationRequest invocation = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodInvocationRequest>(content);
            if (invocation == null || string.IsNullOrEmpty(invocation.OperationIdentifier))
            {
                return null;
            }

            string[] parts = invocation.OperationIdentifier.Split('+', ',');
            return new Command
            {
                TypeName = parts[0].Trim(),
                MethodName = parts[1].Trim(),
                Arguments = invocation.Arguments?.Select(a => a.Value?.ToString()).ToArray() ?? Array.Empty<string>()
            };
        }
        catch (Exception ex)
        {
            Log.Warn("Failed to resolve command from request content: {0}", ex.Message);
            return null;
        }
    }
}
