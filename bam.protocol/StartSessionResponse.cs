using Bam.Encryption;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Represents the server's response to a start session request, containing the session ID, nonce, and server's ECC public key.
/// </summary>
public class StartSessionResponse : BamResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartSessionResponse"/> class with the specified nonce, server public key, and output stream.
    /// </summary>
    /// <param name="nonce">The nonce value for key derivation.</param>
    /// <param name="serverPublicKey">The server's ECC public key.</param>
    /// <param name="outputStream">The stream to write the response to.</param>
    /// <param name="statusCode">The HTTP status code, defaults to 404.</param>
    public StartSessionResponse(string nonce, EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404) : base(outputStream, statusCode)
    {
        Nonce = nonce;
        ServerPublicKey = serverPublicKey;
    }

    /// <summary>
    /// Gets or sets the unique session identifier.
    /// </summary>
    public string SessionId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the nonce value used for key derivation.
    /// </summary>
    public string Nonce { get; set; }

    /// <summary>
    /// Gets or sets the server's ECC public key.
    /// </summary>
    public EccPublicKey ServerPublicKey { get; set; }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void Send(byte[] responseEntity)
    {
        StatusCode = StatusCode == 404 ? 200 : StatusCode;
        OutputStream.Write(responseEntity, 0, responseEntity.Length);
        OutputStream.Flush();
    }
}