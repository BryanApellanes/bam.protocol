using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using Microsoft.AspNetCore.Http;

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

                clock.Now = Now + TimeSpan.FromMinutes(6);
                bool reuseAfterExpiry = Passes(evaluator, Requests.WithEndpoint().At("GET", "/actor-api/hello").WithProof(proof), pair);
                return new FreshnessOutcome(firstUse, reuse, expired, fromTheFuture, insideWindow, reuseAfterExpiry);
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
                noNonce.Request.Headers.Remove(Headers.Nonce);
                DefaultHttpContext shortNonce = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                shortNonce.Request.Headers[Headers.Nonce] = "abc";
                DefaultHttpContext newlineNonce = Requests.WithEndpoint().At("GET", "/x").Proved(pair, Now);
                newlineNonce.Request.Headers[Headers.Nonce] = "0123456789abcdef\nGET";
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
                because.ItsTrue("a missing nonce fails and names the header", !outcome.NoNonce.Success && outcome.NoNonce.Reason!.Contains(Headers.Nonce, StringComparison.Ordinal));
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

    private sealed record BindingOutcome(bool Original, bool OtherMethod, bool OtherPath, bool OtherQuery, bool OtherBody);

    private sealed record FreshnessOutcome(bool FirstUse, bool Reuse, bool Expired, bool FromTheFuture, bool InsideWindow, bool ReuseAfterExpiry);

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
    public void RefuseNewNoncesWhenFullUntilEntriesExpire()
    {
        When.A<MemoryRequestProofNonceStore>("keeps every unexpired nonce and fails closed at capacity",
            () => new MemoryRequestProofNonceStore(new ActorAuthenticationOptions { RequestProofNonceCapacity = 2 }),
            (store) =>
            {
                bool first = store.TryClaim("key-a", "nonce-1", Now.AddMinutes(5), Now);
                bool second = store.TryClaim("key-b", "nonce-1", Now.AddMinutes(1), Now);
                bool repeat = store.TryClaim("key-a", "nonce-1", Now.AddMinutes(5), Now);
                bool full = store.TryClaim("key-a", "nonce-2", Now.AddMinutes(5), Now);
                bool afterExpiry = store.TryClaim("key-a", "nonce-2", Now.AddMinutes(5), Now.AddMinutes(2));
                bool stillRemembered = store.TryClaim("key-a", "nonce-1", Now.AddMinutes(5), Now.AddMinutes(2));
                return new StoreOutcome(first, second, repeat, full, afterExpiry, stillRemembered, store.Count, Refusal(0));
            })
            .TheTest
            .ShouldPass<StoreOutcome>((because, outcome) =>
            {
                because.ItsTrue("a fresh nonce is claimed", outcome.First);
                because.ItsTrue("the same nonce under another key is separate", outcome.SecondScope);
                because.ItsTrue("a repeated nonce is refused", !outcome.Repeat);
                because.ItsTrue("a new nonce is refused while the store is full of unexpired ones", !outcome.Full);
                because.ItsTrue("an expired entry frees room", outcome.AfterExpiry);
                because.ItsTrue("an unexpired nonce is still refused", !outcome.StillRemembered);
                because.ItsTrue("the store holds exactly its capacity", outcome.Count == 2);
                because.ItsTrue("a zero capacity is refused at construction", outcome.ZeroCapacityRefused);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static bool Refusal(int capacity)
    {
        try
        {
            new MemoryRequestProofNonceStore(new ActorAuthenticationOptions { RequestProofNonceCapacity = capacity });
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    private sealed record StoreOutcome(bool First, bool SecondScope, bool Repeat, bool Full, bool AfterExpiry, bool StillRemembered, int Count, bool ZeroCapacityRefused);
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

    private sealed record AudienceOutcome(bool Matching, bool OtherHost, bool Missing, bool Unchecked, string? RoundTrip);
}
