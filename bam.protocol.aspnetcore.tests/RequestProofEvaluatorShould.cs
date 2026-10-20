using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Server;
using Bam.Test;
using System.Net;
using System.Text;
using Bam.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("RequestProofEvaluator Should", Selector = "rpe")]
public class RequestProofEvaluatorShould : UnitTestMenuContainer
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    private static bool Passes(RequestProofEvaluator evaluator, DefaultHttpContext context, EccKeyPair pair)
    {
        return evaluator.EvaluateAsync(context.Request, pair.PublicPem).GetAwaiter().GetResult().Success;
    }

    private static DefaultHttpContext Replay(RequestProofHeaders proof, string method, string path, string query, string body)
    {
        return Requests.WithEndpoint().WithBody(body).At(method, path, query).WithProof(proof);
    }

    [UnitTest]
    public void RejectAProofOnAnyMethodPathQueryOrBodyOtherThanTheOneSigned()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        DefaultHttpContext signed = Requests.WithEndpoint().WithBody("{}").At("POST", "/actor-api/hello", "?lang=en");
        RequestProofHeaders proof = Requests.Proof(signed, pair, Now);

        When.A<RequestProofEvaluator>("verifies a proof only on the request it was made for",
            () => Evaluators.Create(new FixedClock(Now)),
            (_) => new BindingOutcome(
                Passes(Evaluators.Create(new FixedClock(Now)), Replay(proof, "POST", "/actor-api/hello", "?lang=en", "{}"), pair),
                Passes(Evaluators.Create(new FixedClock(Now)), Replay(proof, "DELETE", "/actor-api/hello", "?lang=en", "{}"), pair),
                Passes(Evaluators.Create(new FixedClock(Now)), Replay(proof, "POST", "/actor-api/time", "?lang=en", "{}"), pair),
                Passes(Evaluators.Create(new FixedClock(Now)), Replay(proof, "POST", "/actor-api/hello", "?lang=fr", "{}"), pair),
                Passes(Evaluators.Create(new FixedClock(Now)), Replay(proof, "POST", "/actor-api/hello", "?lang=en", "{ }"), pair)))
            .TheTest
            .ShouldPass<BindingOutcome>((because, outcome) =>
            {
                because.ItsTrue("the signed request verifies", outcome.Original);
                because.ItsTrue("another method fails", !outcome.OtherMethod);
                because.ItsTrue("another path fails", !outcome.OtherPath);
                because.ItsTrue("another query fails", !outcome.OtherQuery);
                because.ItsTrue("another body fails", !outcome.OtherBody);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RejectReusedExpiredAndFutureProofs()
    {
        EccKeyPair pair = TestKeys.NewEcc();

        When.A<FixedClock>("accepts a proof once and only inside the time window",
            () => new FixedClock(Now),
            (clock) =>
            {
                RequestProofEvaluator evaluator = Evaluators.Create(clock);
                DefaultHttpContext first = Requests.WithEndpoint().At("GET", "/actor-api/hello");
                RequestProofHeaders proof = Requests.Proof(first, pair, Now);
                first.WithProof(proof);
                bool firstUse = Passes(evaluator, first, pair);
                bool reuse = Passes(evaluator, Requests.WithEndpoint().At("GET", "/actor-api/hello").WithProof(proof), pair);

                DefaultHttpContext stale = Requests.WithEndpoint().At("GET", "/actor-api/hello").Proved(pair, Now - TimeSpan.FromMinutes(6));
                DefaultHttpContext future = Requests.WithEndpoint().At("GET", "/actor-api/hello").Proved(pair, Now + TimeSpan.FromMinutes(6));
                DefaultHttpContext edge = Requests.WithEndpoint().At("GET", "/actor-api/hello").Proved(pair, Now - TimeSpan.FromMinutes(4));
                bool expired = Passes(evaluator, stale, pair);
                bool fromTheFuture = Passes(evaluator, future, pair);
                bool insideWindow = Passes(evaluator, edge, pair);

                RequestProofHeaders boundary = Requests.Proof(Requests.WithEndpoint().At("GET", "/actor-api/hello"), pair, Now - TimeSpan.FromMinutes(5));
                bool boundaryFirst = Passes(evaluator, Requests.WithEndpoint().At("GET", "/actor-api/hello").WithProof(boundary), pair);
                bool boundaryReplay = Passes(evaluator, Requests.WithEndpoint().At("GET", "/actor-api/hello").WithProof(boundary), pair);

                clock.Now = Now + TimeSpan.FromMinutes(6);
                bool reuseAfterExpiry = Passes(evaluator, Requests.WithEndpoint().At("GET", "/actor-api/hello").WithProof(proof), pair);
                return new FreshnessOutcome(firstUse, reuse, expired, fromTheFuture, insideWindow, reuseAfterExpiry, boundaryFirst, boundaryReplay);
            })
            .TheTest
            .ShouldPass<FreshnessOutcome>((because, outcome) =>
            {
                because.ItsTrue("the first use verifies", outcome.FirstUse);
                because.ItsTrue("the same proof a second time fails", !outcome.Reuse);
                because.ItsTrue("a proof older than the window fails", !outcome.Expired);
                because.ItsTrue("a proof from beyond the window ahead fails", !outcome.FromTheFuture);
                because.ItsTrue("a proof inside the window verifies", outcome.InsideWindow);
                because.ItsTrue("the proof still fails once its timestamp has left the window", !outcome.ReuseAfterExpiry);
                because.ItsTrue("a proof exactly at the edge of the window verifies", outcome.BoundaryFirst);
                because.ItsTrue("and its replay at that same instant is refused", !outcome.BoundaryReplay);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void NotShareASignatureBetweenBodiesThatDifferOnlyInInvalidUtf8()
    {
        // Decoded as UTF-8, both bodies read as "a�b", so a proof over the decoded text would cover
        // both; the proof covers the raw bytes, so it covers only the one signed.
        EccKeyPair pair = TestKeys.NewEcc();
        byte[] signedBody = new byte[] { 0x61, 0xFF, 0x62 };
        byte[] otherBody = new byte[] { 0x61, 0xFE, 0x62 };
        DefaultHttpContext signed = Requests.WithEndpoint().WithBodyBytes(signedBody).At("POST", "/actor-api/hello");
        RequestProofHeaders proof = Requests.Proof(signed, pair, Now);

        When.A<RequestProofEvaluator>("verifies the raw bytes that were signed and nothing else",
            () => Evaluators.Create(new FixedClock(Now)),
            (evaluator) => new RawBytesOutcome(
                System.Text.Encoding.UTF8.GetString(signedBody) == System.Text.Encoding.UTF8.GetString(otherBody),
                Passes(Evaluators.Create(new FixedClock(Now)), Requests.WithEndpoint().WithBodyBytes(otherBody).At("POST", "/actor-api/hello").WithProof(proof), pair),
                Passes(evaluator, Requests.WithEndpoint().WithBodyBytes(signedBody).At("POST", "/actor-api/hello").WithProof(proof), pair)))
            .TheTest
            .ShouldPass<RawBytesOutcome>((because, outcome) =>
            {
                because.ItsTrue("the two bodies decode to the same text", outcome.SameDecodedText);
                because.ItsTrue("the other body fails", !outcome.OtherBody);
                because.ItsTrue("the signed body verifies", outcome.SignedBody);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAMissingOrMalformedTimestampOrNonce()
    {
        EccKeyPair pair = TestKeys.NewEcc();

        When.A<RequestProofEvaluator>("fails a proof without a usable timestamp or nonce",
            () => Evaluators.Create(new FixedClock(Now)),
            (evaluator) =>
            {
                DefaultHttpContext noTimestamp = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                noTimestamp.Request.Headers.Remove(CanonicalRequest.TimestampHeader);
                DefaultHttpContext textTimestamp = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                textTimestamp.Request.Headers[CanonicalRequest.TimestampHeader] = "yesterday";
                DefaultHttpContext noNonce = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                noNonce.Request.Headers.Remove(CanonicalRequest.NonceHeader);
                DefaultHttpContext shortNonce = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                shortNonce.Request.Headers[CanonicalRequest.NonceHeader] = "abc";
                DefaultHttpContext newlineNonce = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                newlineNonce.Request.Headers[CanonicalRequest.NonceHeader] = "0123456789abcdef\nGET";
                return new HeaderOutcome(
                    evaluator.EvaluateAsync(noTimestamp.Request, pair.PublicPem).GetAwaiter().GetResult(),
                    Passes(evaluator, textTimestamp, pair),
                    evaluator.EvaluateAsync(noNonce.Request, pair.PublicPem).GetAwaiter().GetResult(),
                    Passes(evaluator, shortNonce, pair),
                    Passes(evaluator, newlineNonce, pair));
            })
            .TheTest
            .ShouldPass<HeaderOutcome>((because, outcome) =>
            {
                because.ItsTrue("a missing timestamp fails and names the header", !outcome.NoTimestamp.Success && outcome.NoTimestamp.Reason!.Contains(CanonicalRequest.TimestampHeader, StringComparison.Ordinal));
                because.ItsTrue("a non-numeric timestamp fails", !outcome.TextTimestamp);
                because.ItsTrue("a missing nonce fails and names the header", !outcome.NoNonce.Success && outcome.NoNonce.Reason!.Contains(CanonicalRequest.NonceHeader, StringComparison.Ordinal));
                because.ItsTrue("a short nonce fails", !outcome.ShortNonce);
                because.ItsTrue("a nonce with a line break fails", !outcome.NewlineNonce);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RecordANonceOnlyAfterTheSignatureVerifies()
    {
        // A forged request must not burn the nonce of the genuine one, or anyone who sees a proof in
        // flight could deny it.
        EccKeyPair pair = TestKeys.NewEcc();
        DefaultHttpContext genuine = Requests.WithEndpoint().At("POST", "/actor-api/hello");
        RequestProofHeaders proof = Requests.Proof(genuine, pair, Now);
        genuine.WithProof(proof);

        When.A<RequestProofEvaluator>("leaves the nonce unused when a forgery carries it",
            () => Evaluators.Create(new FixedClock(Now)),
            (evaluator) =>
            {
                DefaultHttpContext forgery = Requests.WithEndpoint().At("DELETE", "/actor-api/hello").WithProof(proof);
                bool forged = Passes(evaluator, forgery, pair);
                bool genuineAfter = Passes(evaluator, genuine, pair);
                return new NonceOrderOutcome(forged, genuineAfter);
            })
            .TheTest
            .ShouldPass<NonceOrderOutcome>((because, outcome) =>
            {
                because.ItsTrue("the forgery fails", !outcome.Forged);
                because.ItsTrue("the genuine request still verifies afterwards", outcome.GenuineAfter);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ComposeOneLinePerFieldAndRefuseLineBreaks()
    {
        When.A<string>("builds the canonical text",
            () => "rpe",
            (_) => new ComposeOutcome(
                CanonicalRequest.Compose("post", "/a", "?b=1", 1760529600, "0123456789abcdef", "00ff"),
                Refusal(() => CanonicalRequest.Compose("GET", "/a\n", string.Empty, 1, "0123456789abcdef", "00")),
                Refusal(() => CanonicalRequest.Compose("GET", "/a", "?b\r", 1, "0123456789abcdef", "00")),
                CanonicalRequest.IsWellFormedNonce("0123456789abcdef"),
                CanonicalRequest.IsWellFormedNonce(new string('a', 129)),
                CanonicalRequest.IsWellFormedNonce("0123456789abcde!")))
            .TheTest
            .ShouldPass<ComposeOutcome>((because, outcome) =>
            {
                because.ItsTrue("the text is the version and six fields, one per line, method upper-cased",
                    outcome.Canonical == "BAM-REQUEST-PROOF-1\nPOST\n/a\n?b=1\n1760529600\n0123456789abcdef\n00ff");
                because.ItsTrue("a line break in the path is refused", outcome.PathBreak);
                because.ItsTrue("a line break in the query is refused", outcome.QueryBreak);
                because.ItsTrue("a 16-character nonce is well formed", outcome.SixteenOk);
                because.ItsTrue("a 129-character nonce is not", !outcome.TooLong);
                because.ItsTrue("punctuation outside - and _ is not", !outcome.Punctuation);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAbsoluteFormAndFragmentTargets()
    {
        // #51 round-2 security condition 1: Kestrel reduces http://h?x=1/evil to path "/" and query "?x=1/evil", so a
        // proof signed for /evil must not verify on it. Only origin-form targets carry a proof.
        EccKeyPair pair = TestKeys.NewEcc();

        When.A<RequestProofEvaluator>("evaluates a proof for /evil delivered under other request targets",
            () => Evaluators.Create(new FixedClock(Now)),
            (evaluator) =>
            {
                RequestProofHeaders proof = Requests.Proof(Requests.WithEndpoint().At("GET", "/evil"), pair, Now);
                return new TargetOutcome(
                    Under(evaluator, pair, proof, "http://h:5000?x=1/evil"),
                    Under(evaluator, pair, proof, "http://h:5000/evil"),
                    Under(evaluator, pair, proof, "/evil#frag"),
                    Under(evaluator, pair, proof, "*"),
                    Under(evaluator, pair, proof, "/evil"),
                    Under(evaluator, pair, proof, "/evil\nX"),
                    CanonicalRequest.IsOriginForm("/a?b"),
                    SplitRefuses("http://h?x=1/evil"));
            })
            .TheTest
            .ShouldPass<TargetOutcome>((because, outcome) =>
            {
                because.ItsTrue("an absolute-form target with a query before the path is refused", outcome.AuthorityQuery?.Contains("origin-form", StringComparison.Ordinal) == true);
                because.ItsTrue("any absolute-form target is refused", outcome.Absolute?.Contains("origin-form", StringComparison.Ordinal) == true);
                because.ItsTrue("a target with a fragment is refused", outcome.Fragment?.Contains("origin-form", StringComparison.Ordinal) == true);
                because.ItsTrue("an asterisk-form target is refused", outcome.Asterisk?.Contains("origin-form", StringComparison.Ordinal) == true);
                because.ItsTrue("the origin-form target the proof was made for verifies", outcome.Origin is null);
                because.ItsTrue("a target that cannot be put in canonical form is refused with 401, not a 500", outcome.LineBreak?.Contains("canonical form", StringComparison.Ordinal) == true);
                because.ItsTrue("origin form is recognised", outcome.OriginRecognised);
                because.ItsTrue("SplitTarget refuses a non-origin target", outcome.SplitRefused);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static string? Under(RequestProofEvaluator evaluator, EccKeyPair pair, RequestProofHeaders proof, string rawTarget)
    {
        DefaultHttpContext context = Requests.WithEndpoint().At("GET", "/evil").WithProof(proof);
        context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpRequestFeature>()!.RawTarget = rawTarget;
        RequestProofResult result = evaluator.EvaluateAsync(context.Request, pair.PublicPem).GetAwaiter().GetResult();
        return result.Success ? null : result.Reason;
    }

    private static bool SplitRefuses(string target)
    {
        try
        {
            CanonicalRequest.SplitTarget(target);
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    [UnitTest]
    public void WarnAboutCapacityOncePerKeyPerMinuteNamingTheKeyAndTheBound()
    {
        EccKeyPair keyA = TestKeys.NewEcc();
        EccKeyPair keyB = TestKeys.NewEcc();
        EccKeyPair keyC = TestKeys.NewEcc();
        ActorAuthenticationOptions tight = new ActorAuthenticationOptions { RequestProofNonceCapacity = 2, RequestProofNoncesPerKey = 1 };

        When.A<FixedClock>("refuses proofs at the per-key and total bounds over a minute and more",
            () => new FixedClock(Now),
            (clock) =>
            {
                CapturingLogger<RequestProofEvaluator> logger = new CapturingLogger<RequestProofEvaluator>();
                RequestProofEvaluator evaluator = Evaluators.Create(clock, tight, logger: logger);
                bool Prove(EccKeyPair pair) => evaluator.EvaluateAsync(Requests.WithEndpoint().At("GET", "/x").Proved(pair, clock.Now).Request, pair.PublicPem).GetAwaiter().GetResult().Success;
                int Warnings() => logger.Entries.Count(entry => entry.Key == Microsoft.Extensions.Logging.LogLevel.Warning);

                bool firstA = Prove(keyA);
                Prove(keyA);
                Prove(keyA);
                int afterTwoRefusals = Warnings();
                clock.Now = Now + TimeSpan.FromSeconds(59);
                Prove(keyA);
                int after59Seconds = Warnings();
                clock.Now = Now + TimeSpan.FromSeconds(60);
                Prove(keyA);
                int after60Seconds = Warnings();
                bool firstB = Prove(keyB);
                bool keyCRefused = !Prove(keyC);
                List<string> messages = logger.Entries.Where(entry => entry.Key == Microsoft.Extensions.Logging.LogLevel.Warning).Select(entry => entry.Value).ToList();
                return new WarningOutcome(firstA, afterTwoRefusals, after59Seconds, after60Seconds, firstB, keyCRefused, messages,
                    Bam.Protocol.Profile.PublicKeyFingerprint.Of(keyA.PublicPem)!, Bam.Protocol.Profile.PublicKeyFingerprint.Of(keyC.PublicPem)!);
            })
            .TheTest
            .ShouldPass<WarningOutcome>((because, outcome) =>
            {
                because.ItsTrue("the first proof for key A verifies", outcome.FirstA);
                because.ItsTrue("two refusals at the same instant log one warning", outcome.AfterTwoRefusals == 1);
                because.ItsTrue("a refusal 59 s later logs none", outcome.After59Seconds == 1);
                because.ItsTrue("one at 60 s logs a second", outcome.After60Seconds == 2);
                because.ItsTrue("the per-key warning names the bound and key A", outcome.Messages[0].Contains("per-key", StringComparison.Ordinal) && outcome.Messages[0].Contains(outcome.FingerprintA, StringComparison.Ordinal));
                because.ItsTrue("key B fits in the total", outcome.FirstB);
                because.ItsTrue("key C is refused at the total bound", outcome.KeyCRefused);
                because.ItsTrue("and that warning names the total bound and key C", outcome.Messages.Count == 3 && outcome.Messages[2].Contains("total", StringComparison.Ordinal) && outcome.Messages[2].Contains(outcome.FingerprintC, StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ReportACapacityRefusalAsLoadNotAsAReplay()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        ActorAuthenticationOptions tight = new ActorAuthenticationOptions { RequestProofNonceCapacity = 1, RequestProofNoncesPerKey = 1 };

        When.A<RequestProofEvaluator>("verifies two proofs from one key against a store that holds one",
            () => Evaluators.Create(new FixedClock(Now), tight),
            (evaluator) =>
            {
                RequestProofResult first = evaluator.EvaluateAsync(Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now).Request, pair.PublicPem).GetAwaiter().GetResult();
                RequestProofResult second = evaluator.EvaluateAsync(Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now).Request, pair.PublicPem).GetAwaiter().GetResult();
                return new CapacityOutcome(first.Success, second.Success, second.Reason);
            })
            .TheTest
            .ShouldPass<CapacityOutcome>((because, outcome) =>
            {
                because.ItsTrue("the first proof verifies", outcome.First);
                because.ItsTrue("the second is refused", !outcome.Second);
                because.ItsTrue("with a capacity reason, not a replay reason", outcome.Reason?.Contains("cannot record", StringComparison.Ordinal) == true && outcome.Reason.Contains("already used", StringComparison.Ordinal) == false);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void VerifyARealRequestWithABoundBodyAPathBaseAndAnEscapedPath()
    {
        EccKeyPair pair = TestKeys.NewEcc();

        When.A<string>("posts signed and tampered JSON through Kestrel",
            () => "rpe",
            (_) => PipelineAsync(pair).GetAwaiter().GetResult())
            .TheTest
            .ShouldPass<PipelineOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a signed request reaches the handler ({outcome.SignedStatus})", outcome.SignedStatus == HttpStatusCode.OK);
                because.ItsTrue($"the handler saw the bound body and the decoded route value ({outcome.SignedBody})", outcome.SignedBody.Contains("a:b|widget", StringComparison.Ordinal));
                because.ItsTrue($"a body with one byte changed is refused ({outcome.TamperedStatus})", outcome.TamperedStatus == HttpStatusCode.Unauthorized);
                because.ItsTrue($"a replay of the signed request is refused ({outcome.ReplayStatus})", outcome.ReplayStatus == HttpStatusCode.Unauthorized);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static async Task<PipelineOutcome> PipelineAsync(EccKeyPair pair)
    {
        ActorAuthenticationOptions options = new ActorAuthenticationOptions();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IRequestProof>(new BodySignatureProofVerifier(options));
        builder.Services.AddSingleton<IRequestProofNonceStore>(new MemoryRequestProofNonceStore(options));
        builder.Services.AddSingleton(provider => new RequestProofEvaluator(provider.GetRequiredService<IRequestProof>(), provider.GetRequiredService<IRequestProofNonceStore>(), options));
        WebApplication app = builder.Build();
        app.UsePathBase("/base");
        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering();
            context.SetActor(new ActorData { Handle = "alice", Name = "alice" }, pair.PublicPem);
            await next(context);
        });
        app.MapPost("/items/{id}", (string id, PipelinePayload payload) => Results.Text(id + "|" + payload.Name))
            .WithMetadata(new RequiredAccessAttribute(BamAccess.Execute))
            .AddEndpointFilter<RequestProofEndpointFilter>();
        await app.StartAsync();
        try
        {
            Uri address = new Uri(app.Urls.First());
            Uri target = new Uri(address, "/base/items/a%3Ab?x=1");
            byte[] body = Encoding.UTF8.GetBytes("{\"name\":\"widget\"}");
            using HttpClient client = new HttpClient();
            RequestProofHeaders proof = RequestProofSigner.Sign(new PrivateKeyProvider(pair.PrivateKey.Value), "POST", target, body, DateTimeOffset.UtcNow);

            using HttpResponseMessage signed = await client.SendAsync(Post(target, body, proof));
            string signedBody = await signed.Content.ReadAsStringAsync();
            byte[] tampered = (byte[])body.Clone();
            tampered[10] = (byte)'W';
            RequestProofHeaders tamperedProof = RequestProofSigner.Sign(new PrivateKeyProvider(pair.PrivateKey.Value), "POST", target, body, DateTimeOffset.UtcNow);
            using HttpResponseMessage tamperedResponse = await client.SendAsync(Post(target, tampered, tamperedProof));
            using HttpResponseMessage replay = await client.SendAsync(Post(target, body, proof));
            return new PipelineOutcome(signed.StatusCode, signedBody, tamperedResponse.StatusCode, replay.StatusCode);
        }
        finally
        {
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }

    private static HttpRequestMessage Post(Uri target, byte[] body, RequestProofHeaders proof)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, target) { Content = new ByteArrayContent(body) };
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        proof.ApplyTo(request);
        return request;
    }

    private static bool Refusal(Func<string> compose)
    {
        try
        {
            compose();
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private sealed record TargetOutcome(string? AuthorityQuery, string? Absolute, string? Fragment, string? Asterisk, string? Origin, string? LineBreak, bool OriginRecognised, bool SplitRefused);

    private sealed record WarningOutcome(bool FirstA, int AfterTwoRefusals, int After59Seconds, int After60Seconds, bool FirstB, bool KeyCRefused, List<string> Messages, string FingerprintA, string FingerprintC);

    private sealed record CapacityOutcome(bool First, bool Second, string? Reason);

    private sealed record PipelineOutcome(HttpStatusCode SignedStatus, string SignedBody, HttpStatusCode TamperedStatus, HttpStatusCode ReplayStatus);

    /// <summary>The JSON body the pipeline test binds.</summary>
    public sealed record PipelinePayload(string Name);

    private sealed record BindingOutcome(bool Original, bool OtherMethod, bool OtherPath, bool OtherQuery, bool OtherBody);

    private sealed record FreshnessOutcome(bool FirstUse, bool Reuse, bool Expired, bool FromTheFuture, bool InsideWindow, bool ReuseAfterExpiry, bool BoundaryFirst, bool BoundaryReplay);

    private sealed record RawBytesOutcome(bool SameDecodedText, bool OtherBody, bool SignedBody);

    private sealed record HeaderOutcome(RequestProofResult NoTimestamp, bool TextTimestamp, RequestProofResult NoNonce, bool ShortNonce, bool NewlineNonce);

    private sealed record NonceOrderOutcome(bool Forged, bool GenuineAfter);

    private sealed record ComposeOutcome(string Canonical, bool PathBreak, bool QueryBreak, bool SixteenOk, bool TooLong, bool Punctuation);
}

[UnitTestMenu("MemoryRequestProofNonceStore Should", Selector = "mrpns")]
public class MemoryRequestProofNonceStoreShould : UnitTestMenuContainer
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    [UnitTest]
    public void BoundEachKeySoOneKeyCannotLockOutAnother()
    {
        When.A<MemoryRequestProofNonceStore>("fills one key to its limit, then claims for another key",
            () => new MemoryRequestProofNonceStore(new ActorAuthenticationOptions { RequestProofNonceCapacity = 5, RequestProofNoncesPerKey = 2 }),
            (store) =>
            {
                NonceClaimResult first = store.TryClaim("key-a", "nonce-1", Now.AddMinutes(5), Now);
                NonceClaimResult repeat = store.TryClaim("key-a", "nonce-1", Now.AddMinutes(5), Now);
                NonceClaimResult second = store.TryClaim("key-a", "nonce-2", Now.AddMinutes(5), Now);
                NonceClaimResult keyFull = store.TryClaim("key-a", "nonce-3", Now.AddMinutes(5), Now);
                NonceClaimResult otherKey = store.TryClaim("key-b", "nonce-1", Now.AddMinutes(5), Now);
                NonceClaimResult afterExpiry = store.TryClaim("key-a", "nonce-3", Now.AddMinutes(5), Now.AddMinutes(6));
                return new PerKeyOutcome(first, repeat, second, keyFull, otherKey, afterExpiry);
            })
            .TheTest
            .ShouldPass<PerKeyOutcome>((because, outcome) =>
            {
                because.ItsTrue("a fresh nonce is claimed", outcome.First == NonceClaimResult.Claimed);
                because.ItsTrue("a repeated nonce is a replay, not a capacity refusal", outcome.Repeat == NonceClaimResult.AlreadyUsed);
                because.ItsTrue("a second nonce for the key is claimed", outcome.Second == NonceClaimResult.Claimed);
                because.ItsTrue("the key at its limit is refused for its own capacity", outcome.KeyFull == NonceClaimResult.KeyFull);
                because.ItsTrue("another key is unaffected", outcome.OtherKey == NonceClaimResult.Claimed);
                because.ItsTrue("expired nonces free the key's room", outcome.AfterExpiry == NonceClaimResult.Claimed);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void KeepTheTotalBoundAsABackstopAndDropOnlyAfterExpiry()
    {
        When.A<MemoryRequestProofNonceStore>("fills the whole store across keys",
            () => new MemoryRequestProofNonceStore(new ActorAuthenticationOptions { RequestProofNonceCapacity = 2, RequestProofNoncesPerKey = 2 }),
            (store) =>
            {
                store.TryClaim("key-a", "nonce-1", Now, Now);
                store.TryClaim("key-b", "nonce-1", Now.AddMinutes(5), Now);
                NonceClaimResult full = store.TryClaim("key-c", "nonce-1", Now.AddMinutes(5), Now);
                NonceClaimResult atExpiry = store.TryClaim("key-a", "nonce-1", Now, Now);
                NonceClaimResult afterExpiry = store.TryClaim("key-a", "nonce-1", Now, Now.AddTicks(1));
                return new TotalOutcome(full, atExpiry, afterExpiry, Refusal(0, 0), Refusal(5, 6), Refusal(5, 0));
            })
            .TheTest
            .ShouldPass<TotalOutcome>((because, outcome) =>
            {
                because.ItsTrue("a full store refuses a new key for capacity", outcome.Full == NonceClaimResult.StoreFull);
                because.ItsTrue("a nonce is still remembered at its expiry instant", outcome.AtExpiry == NonceClaimResult.AlreadyUsed);
                because.ItsTrue("and dropped one tick later", outcome.AfterExpiry == NonceClaimResult.Claimed);
                because.ItsTrue("a zero capacity is refused at construction", outcome.ZeroRefused);
                because.ItsTrue("a per-key limit above the total is refused", outcome.PerKeyAboveTotalRefused);
                because.ItsTrue("a zero per-key limit is refused", outcome.ZeroPerKeyRefused);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static bool Refusal(int capacity, int perKey)
    {
        try
        {
            new MemoryRequestProofNonceStore(new ActorAuthenticationOptions { RequestProofNonceCapacity = capacity, RequestProofNoncesPerKey = perKey });
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    private sealed record PerKeyOutcome(NonceClaimResult First, NonceClaimResult Repeat, NonceClaimResult Second, NonceClaimResult KeyFull, NonceClaimResult OtherKey, NonceClaimResult AfterExpiry);

    private sealed record TotalOutcome(NonceClaimResult Full, NonceClaimResult AtExpiry, NonceClaimResult AfterExpiry, bool ZeroRefused, bool PerKeyAboveTotalRefused, bool ZeroPerKeyRefused);
}

[UnitTestMenu("Client token audience Should", Selector = "aud")]
public class ClientTokenAudienceShould : UnitTestMenuContainer
{
    private static string Token(EccKeyPair pair, string? audience)
    {
        BamJwtToken token = new BamJwtToken(Guid.NewGuid().ToString("N"), "alice", "client", TimeSpan.FromMinutes(2)) { Audience = audience };
        return token.Encode(pair.PrivateKey.Value);
    }

    [UnitTest]
    public void AcceptAClientTokenOnlyForThisHostsAudience()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));

        When.A<SignedActorTokenVerifier>("checks aud when the host sets ClientTokenAudience",
            () => new SignedActorTokenVerifier(resolver, new ActorAuthenticationOptions { ClientTokenAudience = "https://gateway.example" }),
            (verifier) =>
            {
                SignedActorTokenVerifier unbound = new SignedActorTokenVerifier(resolver, new ActorAuthenticationOptions());
                return new AudienceOutcome(
                    verifier.Verify(Token(pair, "https://gateway.example")).Success,
                    verifier.Verify(Token(pair, "https://other.example")).Success,
                    verifier.Verify(Token(pair, null)).Success,
                    unbound.Verify(Token(pair, null)).Success,
                    BamJwtToken.Decode(Token(pair, "https://gateway.example")).Audience);
            })
            .TheTest
            .ShouldPass<AudienceOutcome>((because, outcome) =>
            {
                because.ItsTrue("a token for this host verifies", outcome.Matching);
                because.ItsTrue("a token for another host fails", !outcome.OtherHost);
                because.ItsTrue("a token with no audience fails", !outcome.Missing);
                because.ItsTrue("a host without ClientTokenAudience checks none", outcome.Unchecked);
                because.ItsTrue("aud round-trips through encode and decode", outcome.RoundTrip == "https://gateway.example");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseToStartAnEnabledHostWithoutAnAudienceUnlessOptedOut()
    {
        When.A<string>("verifies prerequisites with and without ClientTokenAudience",
            () => "aud",
            (_) => new StartupOutcome(
                StartupFailure(new ActorAuthenticationOptions { Enabled = true, Mode = ActorAuthenticationMode.ClientSignedOnly }),
                StartupFailure(new ActorAuthenticationOptions { Enabled = true, Mode = ActorAuthenticationMode.ClientSignedOnly, ClientTokenAudience = "   " }),
                StartupFailure(new ActorAuthenticationOptions { Enabled = true, Mode = ActorAuthenticationMode.ClientSignedOnly, ClientTokenAudience = "https://gateway.example" }),
                StartupFailure(new ActorAuthenticationOptions { Enabled = true, Mode = ActorAuthenticationMode.ClientSignedOnly, AllowMissingClientTokenAudience = true })))
            .TheTest
            .ShouldPass<StartupOutcome>((because, outcome) =>
            {
                because.ItsTrue("a missing audience refuses start-up and names the setting", outcome.Missing?.Contains("ClientTokenAudience", StringComparison.Ordinal) == true);
                because.ItsTrue("a blank audience counts as missing", outcome.Blank?.Contains("ClientTokenAudience", StringComparison.Ordinal) == true);
                because.ItsTrue("a set audience starts", outcome.Set is null);
                because.ItsTrue("the explicit opt-out starts", outcome.OptedOut is null);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void TreatABlankAudienceAsUnsetWhenVerifying()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", pair));

        When.A<SignedActorTokenVerifier>("verifies a token without aud under a blank configured audience",
            () => new SignedActorTokenVerifier(resolver, new ActorAuthenticationOptions { ClientTokenAudience = " " }),
            (verifier) => verifier.Verify(Token(pair, null)).Success)
            .TheTest
            .ShouldPass<bool>((because, verified) =>
            {
                because.ItsTrue("a blank audience does not demand aud == \" \"", verified);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAnEnabledServiceRegistryHostWithoutAnAudience()
    {
        When.A<string>("registers into a ServiceRegistry with and without an audience",
            () => "aud",
            (_) =>
            {
                string? missing = null;
                try
                {
                    Bam.DependencyInjection.ServiceRegistry.Create().AddActorAuthentication(new ActorAuthenticationOptions { Enabled = true });
                }
                catch (InvalidOperationException failure)
                {
                    missing = failure.Message;
                }

                Bam.DependencyInjection.ServiceRegistry complete = Bam.DependencyInjection.ServiceRegistry.Create();
                complete.For<Bam.Protocol.Data.IPublicKeySetResolver>().UseSingleton<Bam.Protocol.Data.IPublicKeySetResolver>(new FakeKeySetResolver());
                complete.For<Bam.Encryption.INamedKeyStorage>().UseSingleton<Bam.Encryption.INamedKeyStorage>(new InMemoryNamedKeyStorage());
                complete.AddActorAuthentication(new ActorAuthenticationOptions { Enabled = true, ClientTokenAudience = "https://host.example" });
                return missing;
            })
            .TheTest
            .ShouldPass<string?>((because, message) =>
            {
                because.ItsTrue("the ServiceRegistry path refuses an enabled host without an audience", message?.Contains("ClientTokenAudience", StringComparison.Ordinal) == true);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static string? StartupFailure(ActorAuthenticationOptions options)
    {
        Microsoft.Extensions.DependencyInjection.ServiceProvider services = HostServicesForAudience().AddActorAuthentication(options).BuildServiceProvider();
        try
        {
            ActorAuthenticationRegistration.VerifyPrerequisites(services, options);
            return null;
        }
        catch (InvalidOperationException failure)
        {
            return failure.Message;
        }
    }

    private static Microsoft.Extensions.DependencyInjection.IServiceCollection HostServicesForAudience()
    {
        return ActorAuthenticationRegistrationShould.HostServices(new FakeKeySetResolver(), new InMemoryNamedKeyStorage());
    }

    private sealed record StartupOutcome(string? Missing, string? Blank, string? Set, string? OptedOut);

    private sealed record AudienceOutcome(bool Matching, bool OtherHost, bool Missing, bool Unchecked, string? RoundTrip);
}
