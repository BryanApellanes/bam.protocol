using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
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
/// Pins the in-memory, bounded key-set store and named-key storage (bam.protocol#119): capacity, expiry, removal,
/// the registrar's admission rules, thread safety, and a running host that registers, issues a token and serves a
/// proof-signed call on these stores alone.
/// </summary>
[UnitTestMenu("Bounded Memory Key Set Store Should", Selector = "bmks")]
public class BoundedMemoryKeySetStoreShould : UnitTestMenuContainer
{
    private const string Audience = "https://host.test";
    private const string JsonType = "application/json";
    private static readonly DateTimeOffset Start = new DateTimeOffset(2026, 11, 27, 12, 0, 0, TimeSpan.Zero);

    [UnitTest]
    public void RegisterAnEccOnlyKeySetAndResolveItByHandleAndKey()
    {
        When.A<BoundedMemoryKeySetStoreShould>("registers an ECC-only key set and resolves it",
            (test) =>
            {
                EccKeyPair pair = TestKeys.NewEcc();
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(null, new FixedClock(Start));
                PublicKeySetData registered = store.Register(new PublicKeySetData { KeySetHandle = "ada", PublicRsaKey = string.Empty, PublicEccKey = pair.PublicPem, RevokedUtc = Start.UtcDateTime, Created = DateTime.MinValue });
                registered.PublicEccKey = "changed by the caller";
                PublicKeySetData? byHandle = store.ResolveByHandle("ada");
                PublicKeySetData? byKey = store.ResolveByKeyMaterial(pair.PublicPem);
                return new RegisterOutcome(byHandle, byKey, store.ResolveByHandle("Ada"), PublicKeyFingerprint.Of(pair.PublicPem), pair.PublicPem, store.Count);
            })
            .TheTest
            .ShouldPass<RegisterOutcome>((because, outcome) =>
            {
                because.ItsTrue("the handle resolves to the registered key", outcome.ByHandle?.PublicEccKey == outcome.Pem);
                because.ItsTrue("the key resolves to the handle", outcome.ByKey?.KeySetHandle == "ada");
                because.ItsTrue("the ECC fingerprint is stamped with the canonical fingerprint /actor/token returns", outcome.ByHandle?.PublicEccKeyFingerprint == outcome.Fingerprint && outcome.Fingerprint != null);
                because.ItsTrue("a caller-supplied revocation and creation time are replaced", outcome.ByHandle?.RevokedUtc == null && outcome.ByHandle?.Created == Start.UtcDateTime);
                because.ItsTrue("handles are case-sensitive", outcome.OtherCase == null);
                because.ItsTrue("the store holds one key set", outcome.Count == 1);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseWhatTheRegistrarRefuses()
    {
        When.A<BoundedMemoryKeySetStoreShould>("registers a taken handle, a claimed key, bad keys and a blank handle",
            (test) =>
            {
                EccKeyPair pair = TestKeys.NewEcc();
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(null, new FixedClock(Start));
                store.Register(Ecc("ada", pair));
                List<string> refusals = new List<string>
                {
                    Refusal(() => store.Register(Ecc("ada", TestKeys.NewEcc()))),
                    Refusal(() => store.Register(Ecc("bob", pair))),
                    Refusal(() => store.Register(new PublicKeySetData { KeySetHandle = "cy", PublicRsaKey = string.Empty, PublicEccKey = string.Empty })),
                    Refusal(() => store.Register(new PublicKeySetData { KeySetHandle = "cy", PublicRsaKey = string.Empty, PublicEccKey = "not a key" })),
                    Refusal(() => store.Register(new PublicKeySetData { KeySetHandle = "cy", PublicRsaKey = TestKeys.NewEcc().PublicPem, PublicEccKey = string.Empty })),
                    Refusal(() => store.Register(Ecc(" ", TestKeys.NewEcc()))),
                    Refusal(() => store.Rotate(Ecc("ada", TestKeys.NewEcc()), Array.Empty<byte>())),
                };
                return new RefusalOutcome(refusals, store.ResolveByHandle("ada")?.PublicEccKey == pair.PublicPem, store.Count);
            })
            .TheTest
            .ShouldPass<RefusalOutcome>((because, outcome) =>
            {
                because.ItsTrue("refusals: " + string.Join(", ", outcome.Refusals), outcome.Refusals.SequenceEqual(new[]
                {
                    nameof(PublicKeySetConflictException),
                    nameof(PublicKeySetKeyMaterialConflictException),
                    nameof(InvalidPublicKeySetException),
                    nameof(InvalidPublicKeySetException),
                    nameof(InvalidPublicKeySetException),
                    nameof(ArgumentException),
                    nameof(InvalidKeySetRotationException),
                }));
                because.ItsTrue("the first registration is unchanged", outcome.FirstUnchanged);
                because.ItsTrue("nothing else was stored", outcome.Count == 1);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefusePastCapacityUntilAKeySetGoes()
    {
        When.A<BoundedMemoryKeySetStoreShould>("fills a store of two, then removes one",
            (test) =>
            {
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(new BoundedMemoryKeySetStoreOptions { Capacity = 2 }, new FixedClock(Start));
                store.Register(Ecc("a", TestKeys.NewEcc()));
                store.Register(Ecc("b", TestKeys.NewEcc()));
                string full = Refusal(() => store.Register(Ecc("c", TestKeys.NewEcc())));
                int capacity = 0;
                try
                {
                    store.Register(Ecc("c", TestKeys.NewEcc()));
                }
                catch (KeySetCapacityExceededException exception)
                {
                    capacity = exception.Capacity;
                }
                bool removed = store.Remove("a");
                string afterRemove = Refusal(() => store.Register(Ecc("c", TestKeys.NewEcc())));
                return new CapacityOutcome(full, capacity, removed, afterRemove, store.Count);
            })
            .TheTest
            .ShouldPass<CapacityOutcome>((because, outcome) =>
            {
                because.ItsTrue("the third registration is refused for capacity: " + outcome.Full, outcome.Full == nameof(KeySetCapacityExceededException));
                because.ItsTrue("the refusal is a registration refusal the endpoint already catches", typeof(KeySetRegistrationException).IsAssignableFrom(typeof(KeySetCapacityExceededException)));
                because.ItsTrue("the refusal carries the capacity", outcome.Capacity == 2);
                because.ItsTrue("removing one makes room: " + outcome.AfterRemove, outcome.Removed && outcome.AfterRemove == "registered" && outcome.Count == 2);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ForgetAKeySetOnceItsLifetimeHasPassed()
    {
        When.A<BoundedMemoryKeySetStoreShould>("registers a key set and moves the clock past its lifetime",
            (test) =>
            {
                FixedClock clock = new FixedClock(Start);
                EccKeyPair pair = TestKeys.NewEcc();
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(new BoundedMemoryKeySetStoreOptions { Capacity = 1, KeySetLifetime = TimeSpan.FromMinutes(15) }, clock);
                store.Register(Ecc("ada", pair));
                clock.Now = Start.AddMinutes(15).AddTicks(-1);
                bool liveJustBefore = store.ResolveByHandle("ada") != null;
                string fullJustBefore = Refusal(() => store.Register(Ecc("bob", TestKeys.NewEcc())));
                clock.Now = Start.AddMinutes(15);
                bool handleGone = store.ResolveByHandle("ada") == null;
                bool keyGone = store.ResolveByKeyMaterial(pair.PublicPem) == null;
                bool claimsGone = !store.FindKeyMaterialClaims(Ecc("other", pair)).Any();
                int countAfter = store.Count;
                string again = Refusal(() => store.Register(Ecc("ada", pair)));
                return new ExpiryOutcome(liveJustBefore, fullJustBefore, handleGone, keyGone, claimsGone, countAfter, again);
            })
            .TheTest
            .ShouldPass<ExpiryOutcome>((because, outcome) =>
            {
                because.ItsTrue("it resolves until the last tick of its lifetime", outcome.LiveJustBefore);
                because.ItsTrue("and still holds its place: " + outcome.FullJustBefore, outcome.FullJustBefore == nameof(KeySetCapacityExceededException));
                because.ItsTrue("at the end of its lifetime neither its handle, its key nor a material claim resolves", outcome.HandleGone && outcome.KeyGone && outcome.ClaimsGone);
                because.ItsTrue("and it no longer counts", outcome.CountAfter == 0);
                because.ItsTrue("the handle and key can register again: " + outcome.Again, outcome.Again == "registered");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void NeverResolveARemovedKeySet()
    {
        When.A<BoundedMemoryKeySetStoreShould>("removes a key set, twice",
            (test) =>
            {
                EccKeyPair pair = TestKeys.NewEcc();
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(null, new FixedClock(Start));
                store.Register(Ecc("ada", pair));
                bool first = store.Remove("ada");
                bool second = store.Remove("ada");
                bool unknown = store.Remove("nobody") || store.Remove(string.Empty);
                bool gone = store.ResolveByHandle("ada") == null && store.Resolve("ada") == null && store.ResolveByKeyMaterial(pair.PublicPem) == null;
                string again = Refusal(() => store.Register(Ecc("bob", pair)));
                return new RemoveOutcome(first, second, unknown, gone, again);
            })
            .TheTest
            .ShouldPass<RemoveOutcome>((because, outcome) =>
            {
                because.ItsTrue("the first remove removes it, a second finds nothing", outcome.First && !outcome.Second && !outcome.Unknown);
                because.ItsTrue("a removed key set never resolves", outcome.Gone);
                because.ItsTrue("its key may register under another handle: " + outcome.Again, outcome.Again == "registered");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void AdmitExactlyCapacityUnderConcurrentRegistration()
    {
        When.A<BoundedMemoryKeySetStoreShould>("registers 200 handles from many threads into a store of 50 while resolving",
            (test) =>
            {
                BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(new BoundedMemoryKeySetStoreOptions { Capacity = 50 }, new FixedClock(Start));
                List<EccKeyPair> pairs = Enumerable.Range(0, 200).Select(_ => TestKeys.NewEcc()).ToList();
                int registered = 0;
                int full = 0;
                int other = 0;
                int resolvedWrong = 0;
                Parallel.For(0, 200, new ParallelOptions { MaxDegreeOfParallelism = 16 }, index =>
                {
                    try
                    {
                        store.Register(Ecc("h" + index, pairs[index]));
                        Interlocked.Increment(ref registered);
                    }
                    catch (KeySetCapacityExceededException)
                    {
                        Interlocked.Increment(ref full);
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref other);
                    }

                    PublicKeySetData? resolved = store.ResolveByHandle("h" + index);
                    if (resolved != null && resolved.PublicEccKey != pairs[index].PublicPem)
                    {
                        Interlocked.Increment(ref resolvedWrong);
                    }
                });
                return new ConcurrencyOutcome(registered, full, other, resolvedWrong, store.Count);
            })
            .TheTest
            .ShouldPass<ConcurrencyOutcome>((because, outcome) =>
            {
                because.ItsTrue($"exactly 50 were admitted and 150 refused for capacity ({outcome.Registered} admitted, {outcome.Full} full, {outcome.Other} other)", outcome.Registered == 50 && outcome.Full == 150 && outcome.Other == 0);
                because.ItsTrue("no resolve returned another handle's key", outcome.ResolvedWrong == 0);
                because.ItsTrue("the store holds 50", outcome.Count == 50);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseBoundsThatCannotHold()
    {
        When.A<BoundedMemoryKeySetStoreShould>("creates stores with a capacity of 0 and a lifetime of 0",
            (test) => new List<string>
            {
                Refusal(() => new BoundedMemoryKeySetStore(new BoundedMemoryKeySetStoreOptions { Capacity = 0 })),
                Refusal(() => new BoundedMemoryKeySetStore(new BoundedMemoryKeySetStoreOptions { KeySetLifetime = TimeSpan.Zero })),
                Refusal(() => new BoundedMemoryKeySetStore()),
            })
            .TheTest
            .ShouldPass<List<string>>((because, refusals) =>
            {
                because.ItsTrue("bad bounds are refused and the defaults are accepted: " + string.Join(", ", refusals), refusals.SequenceEqual(new[] { nameof(ArgumentOutOfRangeException), nameof(ArgumentOutOfRangeException), "registered" }));
                because.ItsTrue("the defaults are 1000 and 30 minutes", BoundedMemoryKeySetStoreOptions.DefaultCapacity == 1000 && BoundedMemoryKeySetStoreOptions.DefaultKeySetLifetime == TimeSpan.FromMinutes(30));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void KeepNamedKeysApartFromTheCallersArrays()
    {
        When.A<BoundedMemoryKeySetStoreShould>("saves a key, clears the caller's array, and changes the returned one",
            (test) =>
            {
                MemoryNamedKeyStorage keys = new MemoryNamedKeyStorage();
                byte[] saved = new byte[] { 1, 2, 3 };
                bool accepted = keys.SaveNamedKey("k", saved);
                Array.Clear(saved);
                byte[]? first = keys.GetNamedKey("k");
                first![0] = 9;
                byte[]? second = keys.GetNamedKey("k");
                string blankName = Refusal(() => keys.SaveNamedKey(" ", new byte[] { 1 }));
                return new NamedKeyOutcome(accepted, second, keys.GetNamedKey("missing") == null && keys.GetNamedKey(string.Empty) == null, blankName);
            })
            .TheTest
            .ShouldPass<NamedKeyOutcome>((because, outcome) =>
            {
                because.ItsTrue("the stored key is unaffected by either array", outcome.Accepted && outcome.Stored != null && outcome.Stored.SequenceEqual(new byte[] { 1, 2, 3 }));
                because.ItsTrue("a missing name gives null", outcome.MissingIsNull);
                because.ItsTrue("a blank name is refused: " + outcome.BlankName, outcome.BlankName == nameof(ArgumentException));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ServeRegistrationTokenAndAProvedCallOnTheInMemoryStoresAlone()
    {
        When.A<BoundedMemoryKeySetStoreShould>("runs a host on the in-memory stores: register ECC-only, token, proved call, then a taken handle and a full store",
            (test) => RunAsync(async (host) =>
            {
                EccKeyPair pair = TestKeys.NewEcc();
                string body = "{\"KeySetHandle\":\"ada\",\"PublicRsaKeyPem\":\"\",\"PublicEccKeyPem\":" + Quote(pair.PublicPem) + "}";
                Answer registered = await host.PostAsync("/actor/register", body, null);
                Answer issued = await host.PostAsync("/actor/token", "{\"ClientToken\":" + Quote(ClientToken(pair, "ada")) + "}", null);
                string token = Regex.Match(issued.Body, "\"token\":\"([^\"]+)\"").Groups[1].Value;
                string fingerprint = Regex.Match(issued.Body, "\"keyFingerprint\":\"([^\"]+)\"").Groups[1].Value;
                Answer proved = await host.SendAsync("/things", "{}", token, pair);
                Answer taken = await host.PostAsync("/actor/register", "{\"KeySetHandle\":\"ada\",\"PublicRsaKeyPem\":\"\",\"PublicEccKeyPem\":" + Quote(TestKeys.NewEcc().PublicPem) + "}", null);
                Answer second = await host.PostAsync("/actor/register", "{\"KeySetHandle\":\"bob\",\"PublicRsaKeyPem\":\"\",\"PublicEccKeyPem\":" + Quote(TestKeys.NewEcc().PublicPem) + "}", null);
                Answer full = await host.PostAsync("/actor/register", "{\"KeySetHandle\":\"cy\",\"PublicRsaKeyPem\":\"\",\"PublicEccKeyPem\":" + Quote(TestKeys.NewEcc().PublicPem) + "}", null);
                return new PipelineOutcome(registered, issued, fingerprint, PublicKeyFingerprint.Of(pair.PublicPem) ?? string.Empty, proved, taken, second, full);
            }))
            .TheTest
            .ShouldPass<PipelineOutcome>((because, outcome) =>
            {
                because.ItsTrue("an ECC-only registration is accepted: " + outcome.Registered, outcome.Registered.Status == 200);
                because.ItsTrue("a token is issued without confirmation: " + outcome.Issued, outcome.Issued.Status == 200);
                because.ItsTrue("the token's fingerprint is the key's canonical fingerprint", outcome.TokenFingerprint == outcome.KeyFingerprint && outcome.KeyFingerprint.Length == 64);
                because.ItsTrue("the proof-signed call is served: " + outcome.Proved, outcome.Proved.Status == 200 && outcome.Proved.Body == "done");
                because.ItsTrue("a taken handle is the generic 400: " + outcome.Taken, outcome.Taken.Status == 400 && outcome.Taken.Body.Contains("Registration was not accepted.", StringComparison.Ordinal));
                because.ItsTrue("the second handle fills the store of two: " + outcome.Second, outcome.Second.Status == 200);
                because.ItsTrue("a full store is 429 with the capacity message: " + outcome.Full,
                    outcome.Full.Status == 429 && outcome.Full.ContentType.StartsWith(JsonType, StringComparison.Ordinal) && outcome.Full.Body.Contains(ActorEnrollmentEndpoints.CapacityMessage, StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static PublicKeySetData Ecc(string handle, EccKeyPair pair)
    {
        return new PublicKeySetData { KeySetHandle = handle, PublicRsaKey = string.Empty, PublicEccKey = pair.PublicPem };
    }

    private static string Refusal(Action action)
    {
        try
        {
            action();
            return "registered";
        }
        catch (Exception exception)
        {
            return exception.GetType().Name;
        }
    }

    private static string Quote(string value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value);
    }

    private static string ClientToken(EccKeyPair pair, string handle)
    {
        BamJwtToken token = new BamJwtToken(Guid.NewGuid().ToString("N"), handle, "client", TimeSpan.FromMinutes(2)) { Audience = Audience };
        return token.Encode(pair.PrivateKey.Value);
    }

    private static T RunAsync<T>(Func<PipelineHost, Task<T>> calls)
    {
        return RunCoreAsync(calls).GetAwaiter().GetResult();
    }

    private static async Task<T> RunCoreAsync<T>(Func<PipelineHost, Task<T>> calls)
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions { Enabled = true, OpenEnrollment = true, ClientTokenAudience = Audience };
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddInMemoryActorStores(options, new BoundedMemoryKeySetStoreOptions { Capacity = 2, KeySetLifetime = TimeSpan.FromMinutes(15) });
        builder.Services.AddSingleton<IAccountConfirmation>(new FakeAccountConfirmation(Array.Empty<byte>()));
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
            return await calls(new PipelineHost(client));
        }
        finally
        {
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }

    private sealed record Answer(int Status, string ContentType, string Body);

    private sealed record RegisterOutcome(PublicKeySetData? ByHandle, PublicKeySetData? ByKey, PublicKeySetData? OtherCase, string? Fingerprint, string Pem, int Count);

    private sealed record RefusalOutcome(List<string> Refusals, bool FirstUnchanged, int Count);

    private sealed record CapacityOutcome(string Full, int Capacity, bool Removed, string AfterRemove, int Count);

    private sealed record ExpiryOutcome(bool LiveJustBefore, string FullJustBefore, bool HandleGone, bool KeyGone, bool ClaimsGone, int CountAfter, string Again);

    private sealed record RemoveOutcome(bool First, bool Second, bool Unknown, bool Gone, string Again);

    private sealed record ConcurrencyOutcome(int Registered, int Full, int Other, int ResolvedWrong, int Count);

    private sealed record NamedKeyOutcome(bool Accepted, byte[]? Stored, bool MissingIsNull, string BlankName);

    private sealed record PipelineOutcome(Answer Registered, Answer Issued, string TokenFingerprint, string KeyFingerprint, Answer Proved, Answer Taken, Answer Second, Answer Full);

    private sealed class PipelineHost
    {
        private readonly HttpClient _client;

        internal PipelineHost(HttpClient client)
        {
            _client = client;
        }

        internal Task<Answer> PostAsync(string path, string body, string? bearer)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body)) };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonType);
            if (bearer is not null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearer);
            }
            return ReadAsync(request);
        }

        internal Task<Answer> SendAsync(string path, string body, string bearer, EccKeyPair prover)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new ByteArrayContent(bytes) };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonType);
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearer);
            RequestProofSigner.Sign(new PrivateKeyProvider(prover.PrivateKey.Value), "POST", new Uri(_client.BaseAddress!, path), bytes, DateTimeOffset.UtcNow).ApplyTo(request);
            return ReadAsync(request);
        }

        private async Task<Answer> ReadAsync(HttpRequestMessage request)
        {
            using HttpResponseMessage response = await _client.SendAsync(request);
            return new Answer((int)response.StatusCode, response.Content.Headers.ContentType?.ToString() ?? string.Empty, await response.Content.ReadAsStringAsync());
        }
    }
}
