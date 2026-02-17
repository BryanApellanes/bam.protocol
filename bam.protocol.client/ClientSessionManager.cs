using System.Text;
using Bam.Encryption;
using Bam.Server;

namespace Bam.Protocol.Client;

public class ClientSessionManager : IClientSessionManager
{
    public ClientSessionManager(HttpClient httpClient, HostBinding hostBinding)
    {
        this.HttpClient = httpClient;
        this.HostBinding = hostBinding;
    }

    protected HttpClient HttpClient { get; }
    protected HostBinding HostBinding { get; }

    public async Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request)
    {
        string url = $"http://{HostBinding.HostName}:{HostBinding.Port}{BamSessionPaths.Create}";
        string json = System.Text.Json.JsonSerializer.Serialize(new
        {
            ClientPublicKey = request.ClientPublicKey?.Pem
        });
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage httpResponse = await HttpClient.PostAsync(url, content);
        string responseJson = await httpResponse.Content.ReadAsStringAsync();

        var doc = System.Text.Json.JsonDocument.Parse(responseJson);
        string sessionId = doc.RootElement.GetProperty("SessionId").GetString()!;
        string nonce = doc.RootElement.GetProperty("Nonce").GetString()!;
        string serverPublicKeyPem = doc.RootElement.GetProperty("ServerPublicKey").GetString()!;

        EccPublicKey serverPublicKey = new EccPublicKey(serverPublicKeyPem);
        StartSessionResponse response = new StartSessionResponse(nonce, serverPublicKey, Stream.Null, (int)httpResponse.StatusCode);
        response.SessionId = sessionId;
        return response;
    }

    public async Task<IClientSessionState> StartSessionAsync()
    {
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicKey clientPublicKey = clientKeyPair.GetEccPublicKey();

        StartSessionRequest request = new StartSessionRequest { ClientPublicKey = clientPublicKey };
        StartSessionResponse response = await StartSessionAsync(request);

        return new ClientSessionState(
            response.SessionId,
            response.Nonce,
            response.ServerPublicKey,
            clientKeyPair
        );
    }
}
