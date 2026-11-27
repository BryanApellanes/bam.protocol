using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore.Tests;

/// <summary>
/// Pins what a running host answers on the actor-auth endpoints: the status, the content type and the body, for
/// the answers the handlers write and for the ones ASP.NET's request binding writes before a handler runs. These
/// are the answers <c>spec/actor-auth.md</c> lists (bam.protocol#110), so a change here is a change to the wire.
/// The host listens on a loopback port the system picks, and runs as Production whatever the machine's
/// environment variables say, because a Development host writes exception text into some of these answers.
/// </summary>
[UnitTestMenu("Actor Endpoint Answers Should", Selector = "aea")]
public class ActorEndpointAnswersShould : UnitTestMenuContainer
{
    private const string Audience = "https://host.test";
    private const string JsonType = "application/json";

    [UnitTest]
    public void AnswerRegistrationAsTheSpecificationSays()
    {
        When.A<ActorEndpointAnswersShould>("posts good and bad registrations to a running host",
            (test) => RunAsync(async (host) =>
            {
                string good = "{\"KeySetHandle\":\"ada\",\"PublicRsaKeyPem\":\"\",\"PublicEccKeyPem\":" + Quote(host.Actor.PublicPem) + "}";
                host.Registrar.RefuseWith = null;
                Answer accepted = await host.PostAsync("/actor/register", good, JsonType);
                host.Registrar.Refuse = true;
                Answer refused = await host.PostAsync("/actor/register", good, JsonType);
                host.Registrar.Refuse = false;
                Answer missing = await host.PostAsync("/actor/register", "{\"KeySetHandle\":\"ada\",\"PublicEccKeyPem\":\"x\"}", JsonType);
                Answer notJson = await host.PostAsync("/actor/register", "not json", JsonType);
                Answer plainText = await host.PostAsync("/actor/register", good, "text/plain");
                Answer noType = await host.PostAsync("/actor/register", good, null);
                return new RegistrationAnswers(accepted, refused, missing, notJson, plainText, noType);
            }, "Production"))
            .TheTest
            .ShouldPass<RegistrationAnswers>((because, answers) =>
            {
                because.ItsTrue("accepted: " + answers.Accepted, answers.Accepted == new Answer(200, "application/json; charset=utf-8", "{\"keySetHandle\":\"ada\"}"));
                because.ItsTrue("refused by the registrar: " + answers.Refused, answers.Refused == new Answer(400, "application/json; charset=utf-8", "{\"messages\":[\"Registration was not accepted.\"]}"));
                because.ItsTrue("a missing property: " + answers.Missing, answers.Missing == new Answer(400, "", ""));
                because.ItsTrue("a body that is not JSON: " + answers.NotJson, answers.NotJson == new Answer(400, "", ""));
                // bam.protocol#113: this should be 415. Routing matches a wrong content type to its own endpoint, which is not anonymous.
                because.ItsTrue("a text/plain body is answered as an unauthenticated request (bam.protocol#113): " + answers.PlainText, answers.PlainText == new Answer(401, "text/plain", "Authentication failed."));
                because.ItsTrue("no content type: " + answers.NoType, answers.NoType == new Answer(415, "", ""));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void AnswerTheTokenEndpointAsTheSpecificationSays()
    {
        When.A<ActorEndpointAnswersShould>("posts good and bad client tokens to a running host",
            (test) => RunAsync(async (host) =>
            {
                string good = "{\"ClientToken\":" + Quote(host.ClientToken(host.Actor, Audience)) + "}";
                Answer issued = await host.PostAsync("/actor/token", good, JsonType);
                Answer wrongKey = await host.PostAsync("/actor/token", "{\"ClientToken\":" + Quote(host.ClientToken(TestKeys.NewEcc(), Audience)) + "}", JsonType);
                Answer wrongAudience = await host.PostAsync("/actor/token", "{\"ClientToken\":" + Quote(host.ClientToken(host.Actor, "https://elsewhere.test")) + "}", JsonType);
                Answer garbage = await host.PostAsync("/actor/token", "{\"ClientToken\":\"garbage\"}", JsonType);
                Answer missing = await host.PostAsync("/actor/token", "{}", JsonType);
                Answer plainText = await host.PostAsync("/actor/token", good, "text/plain");
                Answer notJson = await host.PostAsync("/actor/token", "not json", JsonType);
                Answer noType = await host.PostAsync("/actor/token", good, null);
                string token = Regex.Match(issued.Body, "\"token\":\"([^\"]+)\"").Groups[1].Value;
                Answer plainTextWithToken = await host.PostAsync("/actor/token", good, "text/plain", token);
                return new TokenAnswers(issued, wrongKey, wrongAudience, garbage, missing, plainText, notJson, noType, plainTextWithToken);
            }, "Production"))
            .TheTest
            .ShouldPass<TokenAnswers>((because, answers) =>
            {
                Answer failure = new Answer(401, "application/json; charset=utf-8", "{\"messages\":[\"Authentication failed.\"]}");
                because.ItsTrue("issued: " + answers.Issued, answers.Issued.Status == 200 && answers.Issued.ContentType == "application/json; charset=utf-8");
                because.ItsTrue("the issued body has the token, an expiry with a UTC offset and the fingerprint: " + answers.Issued.Body,
                    Regex.IsMatch(answers.Issued.Body, "^\\{\"token\":\"[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+\",\"expiresUtc\":\"\\d{4}-\\d\\d-\\d\\dT\\d\\d:\\d\\d:\\d\\d(\\.\\d{1,7})?\\+00:00\",\"keyFingerprint\":\"[0-9a-f]{64}\"\\}$"));
                because.ItsTrue("another key: " + answers.WrongKey, answers.WrongKey == failure);
                because.ItsTrue("another audience: " + answers.WrongAudience, answers.WrongAudience == failure);
                because.ItsTrue("not a token: " + answers.Garbage, answers.Garbage == failure);
                because.ItsTrue("a missing property: " + answers.Missing, answers.Missing == new Answer(400, "", ""));
                // bam.protocol#113: this should be 415. Routing matches a wrong content type to its own endpoint, which is not anonymous.
                because.ItsTrue("a text/plain body is answered as an unauthenticated request (bam.protocol#113): " + answers.PlainText, answers.PlainText == new Answer(401, "text/plain", "Authentication failed."));
                because.ItsTrue("a body that is not JSON: " + answers.NotJson, answers.NotJson == new Answer(400, "", ""));
                because.ItsTrue("no content type: " + answers.NoType, answers.NoType == new Answer(415, "", ""));
                because.ItsTrue("a text/plain body from a caller with a valid token gets the 415: " + answers.PlainTextWithToken, answers.PlainTextWithToken == new Answer(415, "", ""));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void WriteExceptionTextIntoBindingAnswersOnlyInDevelopment()
    {
        When.A<ActorEndpointAnswersShould>("posts a registration without a required property to a Development host",
            (test) => RunAsync(async (host) =>
            {
                Answer missing = await host.PostAsync("/actor/register", "{\"KeySetHandle\":\"ada\",\"PublicEccKeyPem\":\"x\"}", JsonType);
                Answer noType = await host.PostAsync("/actor/register", "{}", null);
                return new DevelopmentAnswers(missing, noType);
            }, "Development"))
            .TheTest
            .ShouldPass<DevelopmentAnswers>((because, answers) =>
            {
                because.ItsTrue("a missing property is still 400, with exception text: " + answers.Missing.Status + " " + answers.Missing.ContentType,
                    answers.Missing.Status == 400 && answers.Missing.ContentType == "text/plain; charset=utf-8" && answers.Missing.Body.Contains("BadHttpRequestException", StringComparison.Ordinal));
                because.ItsTrue("no content type is still 415, with exception text: " + answers.NoType.Status + " " + answers.NoType.ContentType,
                    answers.NoType.Status == 415 && answers.NoType.ContentType == "text/plain; charset=utf-8" && answers.NoType.Body.Contains("BadHttpRequestException", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void AnswerAProtectedEndpointAsTheSpecificationSays()
    {
        When.A<ActorEndpointAnswersShould>("calls a protected endpoint with and without a token and a proof",
            (test) => RunAsync(async (host) =>
            {
                Answer issued = await host.PostAsync("/actor/token", "{\"ClientToken\":" + Quote(host.ClientToken(host.Actor, Audience)) + "}", JsonType);
                string token = Regex.Match(issued.Body, "\"token\":\"([^\"]+)\"").Groups[1].Value;
                Answer noToken = await host.SendAsync("/things", "{}", null, null);
                Answer badToken = await host.SendAsync("/things", "{}", "garbage", null);
                Answer noProof = await host.SendAsync("/things", "{}", token, null);
                Answer proved = await host.SendAsync("/things", "{}", token, host.Actor);
                Answer wrongProof = await host.SendAsync("/things", "{}", token, TestKeys.NewEcc());
                return new ProtectedAnswers(noToken, badToken, noProof, proved, wrongProof);
            }, "Production"))
            .TheTest
            .ShouldPass<ProtectedAnswers>((because, answers) =>
            {
                Answer failure = new Answer(401, "text/plain", "Authentication failed.");
                because.ItsTrue("no token: " + answers.NoToken, answers.NoToken == failure);
                because.ItsTrue("not a token: " + answers.BadToken, answers.BadToken == failure);
                because.ItsTrue("a token and a proof: " + answers.Proved, answers.Proved == new Answer(200, "text/plain; charset=utf-8", "done"));
                because.ItsTrue("no proof is a 401 problem document: " + answers.NoProof,
                    answers.NoProof.Status == 401 && answers.NoProof.ContentType == "application/problem+json" && answers.NoProof.Body.Contains("\"detail\":\"Request proof required", StringComparison.Ordinal));
                because.ItsTrue("a proof by another key is a 401 problem document: " + answers.WrongProof,
                    answers.WrongProof.Status == 401 && answers.WrongProof.ContentType == "application/problem+json" && answers.WrongProof.Body.Contains("\"detail\":\"Request proof failed", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static string Quote(string value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value);
    }

    private static T RunAsync<T>(Func<AnswerHost, Task<T>> calls, string environment)
    {
        return RunCoreAsync(calls, environment).GetAwaiter().GetResult();
    }

    private static async Task<T> RunCoreAsync<T>(Func<AnswerHost, Task<T>> calls, string environment)
    {
        EccKeyPair actor = TestKeys.NewEcc();
        ActorAuthenticationOptions options = new ActorAuthenticationOptions { Enabled = true, OpenEnrollment = true, ClientTokenAudience = Audience };
        InMemoryNamedKeyStorage storage = new InMemoryNamedKeyStorage();
        storage.SaveNamedKey(options.ServerKeyName, TestKeys.PairPem(TestKeys.NewEcc()));
        FakeKeySetRegistrar registrar = new FakeKeySetRegistrar();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = environment });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<IPublicKeySetResolver>(new FakeKeySetResolver().Add(TestKeys.KeySet("ada", actor)));
        builder.Services.AddSingleton<IPublicKeySetRegistrar>(registrar);
        builder.Services.AddSingleton<IAccountConfirmation>(new FakeAccountConfirmation(Array.Empty<byte>()));
        builder.Services.AddSingleton<INamedKeyStorage>(storage);
        builder.Services.AddActorAuthentication(options);
        WebApplication app = builder.Build();
        app.UseRouting();
        app.UseActorAuthentication();
        app.MapActorEnrollment();
        app.MapActorToken();
        app.MapPost("/things", () => Results.Text("done"))
            .WithMetadata(new RequiredAccessAttribute(BamAccess.Execute))
            .AddEndpointFilter<RequestProofEndpointFilter>();
        await app.StartAsync();
        try
        {
            using HttpClient client = new HttpClient { BaseAddress = new Uri(app.Urls.First()) };
            return await calls(new AnswerHost(client, actor, registrar));
        }
        finally
        {
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }

    private sealed record Answer(int Status, string ContentType, string Body);

    private sealed record RegistrationAnswers(Answer Accepted, Answer Refused, Answer Missing, Answer NotJson, Answer PlainText, Answer NoType);

    private sealed record TokenAnswers(Answer Issued, Answer WrongKey, Answer WrongAudience, Answer Garbage, Answer Missing, Answer PlainText, Answer NotJson, Answer NoType, Answer PlainTextWithToken);

    private sealed record DevelopmentAnswers(Answer Missing, Answer NoType);

    private sealed record ProtectedAnswers(Answer NoToken, Answer BadToken, Answer NoProof, Answer Proved, Answer WrongProof);

    private sealed class AnswerHost
    {
        private readonly HttpClient _client;

        internal AnswerHost(HttpClient client, EccKeyPair actor, FakeKeySetRegistrar registrar)
        {
            _client = client;
            Actor = actor;
            Registrar = registrar;
        }

        internal EccKeyPair Actor { get; }

        internal FakeKeySetRegistrar Registrar { get; }

        internal string ClientToken(EccKeyPair pair, string audience)
        {
            BamJwtToken token = new BamJwtToken(Guid.NewGuid().ToString("N"), "ada", "client", TimeSpan.FromMinutes(2)) { Audience = audience };
            return token.Encode(pair.PrivateKey.Value);
        }

        internal Task<Answer> PostAsync(string path, string body, string? contentType, string? bearer = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body)) };
            if (bearer is not null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearer);
            }

            if (contentType is not null)
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }

            return ReadAsync(request);
        }

        internal Task<Answer> SendAsync(string path, string body, string? bearer, EccKeyPair? prover)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new ByteArrayContent(bytes) };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonType);
            if (bearer is not null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearer);
            }

            if (prover is not null)
            {
                RequestProofSigner.Sign(new PrivateKeyProvider(prover.PrivateKey.Value), "POST", new Uri(_client.BaseAddress!, path), bytes, DateTimeOffset.UtcNow).ApplyTo(request);
            }

            return ReadAsync(request);
        }

        private async Task<Answer> ReadAsync(HttpRequestMessage request)
        {
            using HttpResponseMessage response = await _client.SendAsync(request);
            return new Answer((int)response.StatusCode, response.Content.Headers.ContentType?.ToString() ?? string.Empty, await response.Content.ReadAsStringAsync());
        }
    }
}
