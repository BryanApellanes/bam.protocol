namespace Bam.Protocol.Tests;

[RequiredAccess(BamAccess.Read)]
public class TestEchoService
{
    public static string Echo(string message)
    {
        return $"Echo: {message}";
    }
}
