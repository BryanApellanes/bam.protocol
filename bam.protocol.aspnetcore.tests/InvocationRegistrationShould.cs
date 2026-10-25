using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Test;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("BamInvocationRegistration Should", Selector = "bir")]
public class BamInvocationRegistrationShould : UnitTestMenuContainer
{
    private static readonly ActorAuthenticationOptions ClientSigned = new ActorAuthenticationOptions
    {
        Enabled = true,
        Mode = ActorAuthenticationMode.ClientSignedOnly,
        OpenEnrollment = true,
        EnrolledActorAccess = BamAccess.Execute,
        AllowMissingClientTokenAudience = true,
    };

    private static readonly Type[] Invocable = { typeof(TestInvocationService) };

    private static BamInvocationOptions Options()
    {
        return new BamInvocationOptions { InvocableTypes = Invocable };
    }

    private static ServiceRegistry DispatchRegistry()
    {
        ServiceRegistry registry = new ServiceRegistry();
        registry.For<TestInvocationService>().Use<TestInvocationService>();
        return registry;
    }

    private static WebApplicationBuilder HostBuilder(FakeKeySetResolver resolver, ActorAuthenticationOptions options)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<IPublicKeySetResolver>(resolver);
        builder.Services.AddSingleton<IPublicKeySetRegistrar>(new FakeKeySetRegistrar());
        builder.Services.AddSingleton<IAccountConfirmation>(new FakeAccountConfirmation(Array.Empty<byte>()));
        builder.Services.AddSingleton<INamedKeyStorage>(new InMemoryNamedKeyStorage());
        builder.Services.AddActorAuthentication(options);
        return builder;
    }

    [UnitTest]
    public void RegisterTheBridgeAndShareTheAmbientContextWithTheDispatchRegistry()
    {
        When.A<ServiceRegistry>("adds the bridge to a host",
            DispatchRegistry,
            (registry) =>
            {
                ServiceCollection services = new ServiceCollection();
                services.AddActorAuthentication(ClientSigned);
                services.AddBamInvocation(registry, new BamInvocationOptions { Pattern = "/rpc", MaxBodyBytes = 2048, InvocableTypes = Invocable });
                ServiceProvider provider = services.BuildServiceProvider();

                ServiceRegistry preset = DispatchRegistry();
                AmbientActorContext own = new AmbientActorContext();
                preset.For<IActorContext>().Use(own);
                new ServiceCollection().AddBamInvocation(preset, Options());

                return new RegistrationOutcome(
                    provider.GetService<BamInvocationHandler>() is not null,
                    provider.GetService<IInvocationTargetResolver>() is RegisteredTypeInvocationTargetResolver,
                    provider.GetService<BamInvocationOptions>()?.Pattern,
                    provider.GetService<BamInvocationOptions>()?.MaxBodyBytes,
                    ReferenceEquals(provider.GetService<BamInvocationDispatchRegistry>()?.Registry, registry),
                    registry.Get<IActorContext>() is AmbientActorContext,
                    ReferenceEquals(provider.GetService<IActorContext>(), registry.Get<IActorContext>()),
                    ReferenceEquals(preset.Get<IActorContext>(), own));
            })
            .TheTest
            .ShouldPass<RegistrationOutcome>((because, outcome) =>
            {
                because.ItsTrue("the handler is registered", outcome.Handler);
                because.ItsTrue("the registered-type resolver is the default", outcome.Resolver);
                because.ItsTrue("explicit options are kept", outcome.Pattern == "/rpc" && outcome.MaxBodyBytes == 2048);
                because.ItsTrue("the dispatch registry wraps the host's ServiceRegistry", outcome.SameRegistry);
                because.ItsTrue("IActorContext is registered in the dispatch registry for invoked services", outcome.AmbientInRegistry);
                because.ItsTrue("the host container and the dispatch registry share one IActorContext", outcome.SharedAmbient);
                because.ItsTrue("a host's own IActorContext registration is left alone", outcome.PresetKept);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void MapANonAnonymousEndpointAndFailFastWithoutItsPrerequisites()
    {
        When.A<ServiceRegistry>("maps /invoke and refuses hosts missing AddBamInvocation or AddActorAuthentication",
            DispatchRegistry,
            (registry) =>
            {
                WebApplicationBuilder builder = WebApplication.CreateBuilder();
                builder.Services.AddActorAuthentication(ClientSigned);
                builder.Services.AddBamInvocation(registry, Options());
                WebApplication app = builder.Build();
                app.MapBamInvocation();
                app.MapBamInvocation("/custom");
                IReadOnlyList<Endpoint> endpoints = ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).ToList();
                Endpoint invoke = endpoints.Single(endpoint => endpoint.DisplayName!.Contains("/invoke", StringComparison.Ordinal));
                Endpoint custom = endpoints.Single(endpoint => endpoint.DisplayName!.Contains("/custom", StringComparison.Ordinal));
                string? methods = string.Join(",", invoke.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? Array.Empty<string>());

                WebApplicationBuilder noBridge = WebApplication.CreateBuilder();
                noBridge.Services.AddActorAuthentication(ClientSigned);
                string? bridgeRefusal = Refusal(() => noBridge.Build().MapBamInvocation());

                WebApplicationBuilder noAuth = WebApplication.CreateBuilder();
                noAuth.Services.AddBamInvocation(DispatchRegistry(), Options());
                string? authRefusal = Refusal(() => noAuth.Build().MapBamInvocation());

                WebApplicationBuilder disabled = WebApplication.CreateBuilder();
                disabled.Services.AddActorAuthentication(new ActorAuthenticationOptions { Enabled = false });
                disabled.Services.AddBamInvocation(DispatchRegistry(), Options());
                string? disabledRefusal = Refusal(() => disabled.Build().MapBamInvocation());

                return new MappingOutcome(
                    invoke.Metadata.GetMetadata<AnonymousAccessAttribute>()?.AllowAnonymous,
                    custom.Metadata.GetMetadata<AnonymousAccessAttribute>()?.AllowAnonymous,
                    methods,
                    bridgeRefusal,
                    authRefusal,
                    disabledRefusal);
            })
            .TheTest
            .ShouldPass<MappingOutcome>((because, outcome) =>
            {
                because.ItsTrue("/invoke is explicitly not anonymous", outcome.InvokeAllowsAnonymous == false);
                because.ItsTrue("a custom pattern is explicitly not anonymous", outcome.CustomAllowsAnonymous == false);
                because.ItsTrue($"only POST is mapped ({outcome.Methods})", outcome.Methods == "POST");
                because.ItsTrue("mapping without AddBamInvocation fails fast", outcome.BridgeRefusal?.Contains("AddBamInvocation", StringComparison.Ordinal) == true);
                because.ItsTrue("mapping without AddActorAuthentication fails fast", outcome.AuthRefusal?.Contains("AddActorAuthentication", StringComparison.Ordinal) == true);
                because.ItsTrue("mapping with actor authentication disabled fails fast", outcome.DisabledRefusal?.Contains("Enabled", StringComparison.Ordinal) == true);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ServeAnInvocationEndToEndOverHttpBehindActorAuthentication()
    {
        EccKeyPair alice = TestKeys.NewEcc();
        EccKeyPair stranger = TestKeys.NewEcc();
        FakeKeySetResolver resolver = new FakeKeySetResolver().Add(TestKeys.KeySet("alice", alice));

        When.A<ServiceRegistry>("hosts /invoke on Kestrel and calls it with and without credentials",
            DispatchRegistry,
            (registry) =>
            {
                WebApplicationBuilder builder = HostBuilder(resolver, ClientSigned);
                builder.WebHost.UseUrls("http://127.0.0.1:0");
                builder.Services.AddBamInvocation(registry, Options());
                WebApplication app = builder.Build();
                app.UseActorAuthentication();
                app.MapBamInvocation();
                app.StartAsync().GetAwaiter().GetResult();
                try
                {
                    Uri baseAddress = new Uri(app.Urls.First());
                    using HttpClient client = new HttpClient { BaseAddress = baseAddress };

                    string echo = Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "over http")));
                    string whoAmI = Invocations.Envelope<TestInvocationService>("WhoAmI");
                    string secret = Invocations.Envelope<UnregisteredService>("Secret");

                    return new RoundtripOutcome(
                        Send(client, echo, TestKeys.ClientToken(alice, "alice"), alice),
                        Send(client, whoAmI, TestKeys.ClientToken(alice, "alice"), alice),
                        Send(client, echo, TestKeys.ClientToken(alice, "alice"), null),
                        Send(client, echo, null, alice),
                        Send(client, echo, TestKeys.ClientToken(stranger, "alice"), stranger),
                        Send(client, secret, TestKeys.ClientToken(alice, "alice"), alice));
                }
                finally
                {
                    app.StopAsync().GetAwaiter().GetResult();
                }
            })
            .TheTest
            .ShouldPass<RoundtripOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a token plus proof invokes the service ({outcome.Echo})", outcome.Echo == "200:\"Echo: over http\"");
                because.ItsTrue($"the service sees the actor the middleware verified ({outcome.WhoAmI})", outcome.WhoAmI.StartsWith("200:\"alice@", StringComparison.Ordinal));
                because.ItsTrue($"a token without proof is 401 ({outcome.NoProof})", outcome.NoProof.StartsWith("401:", StringComparison.Ordinal));
                because.ItsTrue($"no token is the middleware's 401 ({outcome.NoToken})", outcome.NoToken.StartsWith("401:", StringComparison.Ordinal));
                because.ItsTrue($"a token signed by an unknown key is 401 ({outcome.Forged})", outcome.Forged.StartsWith("401:", StringComparison.Ordinal));
                because.ItsTrue($"an unregistered type is 461 ({outcome.Unregistered})", outcome.Unregistered.StartsWith("461:", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private static string Send(HttpClient client, string body, string? token, EccKeyPair? prover)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/invoke");
        request.Content = new ByteArrayContent(bytes);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (prover is not null)
        {
            RequestProofHeaders proof = RequestProofSigner.Sign(new PrivateKeyProvider(prover.PrivateKey.Value), "POST", new Uri("/invoke", UriKind.Relative), bytes, DateTimeOffset.UtcNow);
            foreach (KeyValuePair<string, string> header in proof.ToHeaders())
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        using HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
        string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return (int)response.StatusCode + ":" + content;
    }

    private static string? Refusal(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }

    private sealed record RegistrationOutcome(bool Handler, bool Resolver, string? Pattern, long? MaxBodyBytes, bool SameRegistry, bool AmbientInRegistry, bool SharedAmbient, bool PresetKept);

    private sealed record MappingOutcome(bool? InvokeAllowsAnonymous, bool? CustomAllowsAnonymous, string? Methods, string? BridgeRefusal, string? AuthRefusal, string? DisabledRefusal);

    private sealed record RoundtripOutcome(string Echo, string WhoAmI, string NoProof, string NoToken, string Forged, string Unregistered);
}
