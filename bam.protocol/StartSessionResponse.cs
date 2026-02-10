using Bam.Encryption;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class StartSessionResponse : BamResponse
{
    public StartSessionResponse(string nonce, EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404) : base(outputStream, statusCode)
    {
        Nonce = nonce;
        ServerPublicKey = serverPublicKey;
    }

    public string SessionId { get; set; }
    public string Nonce { get; set; }
    public EccPublicKey ServerPublicKey { get; set; }

    public override void Send()
    {
        var responseData = new
        {
            SessionId,
            Nonce,
            ServerPublicKey = ServerPublicKey?.Pem
        };
        string json = System.Text.Json.JsonSerializer.Serialize(responseData);
        Send(System.Text.Encoding.UTF8.GetBytes(json));
    }

    public override void Send(byte[] responseEntity)
    {
        StatusCode = StatusCode == 404 ? 200 : StatusCode;
        OutputStream.Write(responseEntity, 0, responseEntity.Length);
        OutputStream.Flush();
    }
}