namespace Bam.Protocol.Tests;

public class TestClass
{
    public string Name { get; set; }

    public string TestMethod(string argument1, string argument2)
    {
        return $"name = {Name}, argument1 = {argument1}, argument2 = {argument2}";
    }
}