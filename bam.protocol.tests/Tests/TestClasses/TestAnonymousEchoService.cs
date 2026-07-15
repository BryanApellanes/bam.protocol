namespace Bam.Protocol.Tests;

[AnonymousAccess]
public class TestAnonymousEchoService
{
    public static string Echo(string message)
    {
        return $"Echo: {message}";
    }
}
