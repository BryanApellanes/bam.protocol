using System.Text;
using Bam.DependencyInjection;
using Bam.Protocol.Server;
using Bam.Services;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("Request reader should")]
public class RequestReaderShould : UnitTestMenuContainer
{
    public RequestReaderShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void ReadLineFromStream()
    {
        string firstLine = "this is the first line";
        string secondLine = "this is the second line";
        string multiLineValue = $@"{firstLine}
{secondLine}";

        When.A<TestBamRequestReader>("reads a line from stream",
            () => new TestBamRequestReader(new BamRequestReaderOptions(new BamServerOptions())),
            (reader) =>
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(Encoding.ASCII.GetBytes(multiLineValue), 0, multiLineValue.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return reader.ReadStringLineAccessor(stream).Trim();
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.As<string>("read line equals first line", result => firstLine.Equals(result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReadRequestFromStream()
    {
        string requestBody = @"This is the request body";
        string requestStream = $@"POST bam://test.com/file/path BAM/2.0
Content-Type: {MediaTypes.BamPipeline}
Accept: {MediaTypes.Json}
X-Bam-Test: another header value

{requestBody}
";

        When.A<TestBamRequestReader>("reads a request from stream",
            () => new TestBamRequestReader(new BamRequestReaderOptions(new BamServerOptions())),
            (reader) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(requestStream));
                return reader.ReadRequest(stream);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IBamRequest>("Content equals request body", r => requestBody.Equals(r?.Content))
                .As<IBamRequest>("Headers contain content-type", r => r?.Headers.ContainsKey("content-type"))
                .As<IBamRequest>("Headers contain accept", r => r?.Headers.ContainsKey("accept"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

}
