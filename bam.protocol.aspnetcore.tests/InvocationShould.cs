using System.Reflection;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Test;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Bam.Protocol.AspNetCore.Tests;

[UnitTestMenu("RegisteredTypeInvocationTargetResolver Should", Selector = "itr")]
public class RegisteredTypeInvocationTargetResolverShould : UnitTestMenuContainer
{
    private static RegisteredTypeInvocationTargetResolver Resolver()
    {
        ServiceRegistry registry = new ServiceRegistry();
        registry.For<TestInvocationService>().Use<TestInvocationService>();
        registry.For<TestInvocationService.Nested>().Use<TestInvocationService.Nested>();
        registry.For<UnattributedService>().Use<UnattributedService>();
        registry.For<DerivedService>().Use<DerivedService>();
        registry.For<IKeyedService>().Use<KeyedService>();
        return new RegisteredTypeInvocationTargetResolver(new BamInvocationDispatchRegistry(registry));
    }

    [UnitTest]
    public void KeepInheritedGenericAndInterfaceMembersInsideTheRegisteredSurface()
    {
        When.A<RegisteredTypeInvocationTargetResolver>("resolves only members the named type declares itself",
            Resolver,
            (resolver) =>
            {
                ServiceRegistry withBase = new ServiceRegistry();
                withBase.For<BaseService>().Use<BaseService>();
                withBase.For<DerivedService>().Use<DerivedService>();
                RegisteredTypeInvocationTargetResolver baseRegistered = new RegisteredTypeInvocationTargetResolver(new BamInvocationDispatchRegistry(withBase));

                return new SurfaceOutcome(
                    resolver.Resolve(Invocations.Operation<DerivedService>("Own")),
                    resolver.Resolve(Invocations.Operation<DerivedService>("FromBase")),
                    baseRegistered.Resolve(Invocations.Operation<DerivedService>("FromBase")),
                    baseRegistered.Resolve(Invocations.Operation<BaseService>("FromBase")),
                    resolver.Resolve(Invocations.Operation<DerivedService>("Generic")),
                    resolver.Resolve(Invocations.Operation<UnattributedService>("Hi")),
                    resolver.Resolve(Invocations.Operation<IKeyedService>("Granted")),
                    resolver.Resolve(Invocations.Operation<IKeyedService>("Ungranted")),
                    resolver.Resolve(Invocations.Operation<KeyedService>("Granted")));
            })
            .TheTest
            .ShouldPass<SurfaceOutcome>((because, outcome) =>
            {
                because.ItsTrue("a method the registered type declares resolves", outcome.Own?.Method.Name == "Own");
                because.ItsTrue("a method inherited from an unregistered base is refused", outcome.InheritedFromUnregistered is null);
                because.ItsTrue("an inherited method is refused under the derived name even when the base is registered", outcome.InheritedFromRegistered is null);
                because.ItsTrue("the base's own method resolves under the base's name with the base's rules (Denied, it has no attribute)", outcome.BaseByOwnName?.Method.DeclaringType == typeof(BaseService) && outcome.BaseByOwnName.RequiredAccess == BamAccess.Denied);
                because.ItsTrue("an open generic method is refused", outcome.Generic is null);
                because.ItsTrue("a type with no access attribute defaults to Denied", outcome.Unattributed?.RequiredAccess == BamAccess.Denied);
                because.ItsTrue("an interface method's own attribute applies", outcome.InterfaceGranted?.RequiredAccess == BamAccess.Execute);
                because.ItsTrue("an interface method without an attribute is Denied whatever the implementation says", outcome.InterfaceUngranted?.RequiredAccess == BamAccess.Denied);
                because.ItsTrue("the implementation class is not reachable by its own name when only the interface is registered", outcome.ImplementationByName is null);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void FreezeTheAllowListWhenConstructed()
    {
        When.A<ServiceRegistry>("keeps types the registry constructs later out of the allow-list",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.For<IKeyedService>().Use<KeyedService>();
                registry.For<NeedsHelper>().Use<NeedsHelper>();
                return registry;
            },
            (registry) =>
            {
                RegisteredTypeInvocationTargetResolver resolver = new RegisteredTypeInvocationTargetResolver(new BamInvocationDispatchRegistry(registry));
                InvocationTarget? implementationBefore = resolver.Resolve(Invocations.Operation<KeyedService>("Granted"));

                registry.Get<IKeyedService>();
                registry.Get<NeedsHelper>();
                bool registryGrew = registry.MappedTypes.Contains(typeof(KeyedService)) && registry.MappedTypes.Contains(typeof(Helper));

                InvocationTarget? implementationAfter = resolver.Resolve(Invocations.Operation<KeyedService>("Granted"));
                InvocationTarget? helperAfter = resolver.Resolve(Invocations.Operation<Helper>("Leak"));
                InvocationTarget? interfaceStill = resolver.Resolve(Invocations.Operation<IKeyedService>("Granted"));

                RegisteredTypeInvocationTargetResolver explicitList = new RegisteredTypeInvocationTargetResolver(new BamInvocationDispatchRegistry(registry, new[] { typeof(IKeyedService) }));
                InvocationTarget? listed = explicitList.Resolve(Invocations.Operation<IKeyedService>("Granted"));
                InvocationTarget? unlisted = explicitList.Resolve(Invocations.Operation<NeedsHelper>("Do"));

                string? unregisteredListing;
                try
                {
                    new BamInvocationDispatchRegistry(registry, new[] { typeof(UnregisteredService) });
                    unregisteredListing = null;
                }
                catch (ArgumentException ex)
                {
                    unregisteredListing = ex.Message;
                }

                return new FrozenOutcome(implementationBefore, registryGrew, implementationAfter, helperAfter, interfaceStill, listed, unlisted, unregisteredListing);
            })
            .TheTest
            .ShouldPass<FrozenOutcome>((because, outcome) =>
            {
                because.ItsTrue("the implementation behind an interface key is not resolvable by its own name before any call", outcome.ImplementationBefore is null);
                because.ItsTrue("constructing the services makes the registry cache the implementation and the unregistered constructor dependency", outcome.RegistryGrew);
                because.ItsTrue("the implementation is still not resolvable afterwards", outcome.ImplementationAfter is null);
                because.ItsTrue("the constructed constructor dependency is not resolvable either", outcome.HelperAfter is null);
                because.ItsTrue("the interface key still resolves", outcome.InterfaceStill?.Method.Name == "Granted");
                because.ItsTrue("an explicit list admits exactly what it names", outcome.Listed?.Method.Name == "Granted" && outcome.Unlisted is null);
                because.ItsTrue($"listing a type the registry does not hold is refused at construction ({outcome.UnregisteredListing})", outcome.UnregisteredListing?.Contains("UnregisteredService", StringComparison.Ordinal) == true);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveRegisteredPublicMethodsAndReadTheirRules()
    {
        When.A<RegisteredTypeInvocationTargetResolver>("resolves instance, static, nested and attributed methods",
            Resolver,
            (resolver) => new ResolvedOutcome(
                resolver.Resolve(Invocations.Operation<TestInvocationService>("Echo")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("Ping")),
                resolver.Resolve(Invocations.Operation<TestInvocationService.Nested>("Hello")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("Privileged")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("Open")),
                resolver.Resolve(OperationIdentifier.For(typeof(TestInvocationService).GetMethod("Add")!))))
            .TheTest
            .ShouldPass<ResolvedOutcome>((because, outcome) =>
            {
                because.ItsTrue("a public instance method on a registered type resolves", outcome.Echo?.Method.Name == "Echo");
                because.ItsTrue("the type-level RequiredAccess is read", outcome.Echo?.RequiredAccess == BamAccess.Execute);
                because.ItsTrue("no explicit proof attribute means null", outcome.Echo?.ExplicitProof is null);
                because.ItsTrue("a public static method resolves", outcome.Ping?.Method.IsStatic == true);
                because.ItsTrue("a nested registered type resolves through the last '+'", outcome.Nested?.Method.Name == "Hello");
                because.ItsTrue("a method-level RequiredAccess overrides the type's", outcome.Privileged?.RequiredAccess == BamAccess.Write);
                because.ItsTrue("an AnonymousAccess method is marked anonymous", outcome.Open?.IsAnonymous == true);
                because.ItsTrue("the identifier OperationIdentifier.For writes resolves too", outcome.FromFor?.Method.Name == "Add");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseEverythingOutsideTheRegisteredSurface()
    {
        When.A<RegisteredTypeInvocationTargetResolver>("refuses unregistered, malformed, object, special-name, ambiguous and private targets",
            Resolver,
            (resolver) => new RefusedOutcome(
                resolver.Resolve(Invocations.Operation<UnregisteredService>("Secret")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("ToString")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("GetHashCode")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("Overloaded")),
                resolver.Resolve(Invocations.Operation<TestInvocationService>("NoSuchMethod")),
                resolver.Resolve($"{typeof(TestInvocationService).FullName}+Echo, some.other.assembly"),
                resolver.Resolve(Invocations.Operation<string>("Trim")),
                resolver.Resolve(null),
                resolver.Resolve(string.Empty),
                resolver.Resolve("NoCommaHere+Echo"),
                resolver.Resolve("TypeOnly, assembly"),
                resolver.Resolve("+Echo, assembly"),
                resolver.Resolve($"{typeof(TestInvocationService).FullName}+, {typeof(TestInvocationService).Assembly.GetName().Name}"),
                resolver.Resolve("System.Reflection.Assembly+Load, System.Private.CoreLib")))
            .TheTest
            .ShouldPass<RefusedOutcome>((because, outcome) =>
            {
                because.ItsTrue("a loaded but unregistered type is refused", outcome.Unregistered is null);
                because.ItsTrue("object.ToString is refused", outcome.ToStringMember is null);
                because.ItsTrue("object.GetHashCode is refused", outcome.GetHashCodeMember is null);
                because.ItsTrue("an overloaded method is refused as ambiguous", outcome.Ambiguous is null);
                because.ItsTrue("a missing method is refused", outcome.Missing is null);
                because.ItsTrue("a registered type named with the wrong assembly is refused", outcome.WrongAssembly is null);
                because.ItsTrue("a BCL type is refused", outcome.Bcl is null);
                because.ItsTrue("null is refused", outcome.Null is null);
                because.ItsTrue("empty is refused", outcome.Empty is null);
                because.ItsTrue("an identifier without an assembly is refused", outcome.NoAssembly is null);
                because.ItsTrue("an identifier without a method separator is refused", outcome.NoMethod is null);
                because.ItsTrue("an identifier with an empty type is refused", outcome.EmptyType is null);
                because.ItsTrue("an identifier with an empty method is refused", outcome.EmptyMethod is null);
                because.ItsTrue("Assembly.Load is not reachable", outcome.AssemblyLoad is null);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void ParseIdentifiersWithoutResolvingAnything()
    {
        When.A<RegisteredTypeInvocationTargetResolver>("splits type, method and assembly simple name",
            Resolver,
            (ignored) =>
            {
                bool full = RegisteredTypeInvocationTargetResolver.TryParse("My.Ns.Outer+Inner+Method, my.assembly, Version=1.0.0.0, Culture=neutral", out string type, out string method, out string assembly);
                bool bare = RegisteredTypeInvocationTargetResolver.TryParse("My.Ns.Type+Do, my.assembly", out string bareType, out string bareMethod, out string bareAssembly);
                bool noAssembly = RegisteredTypeInvocationTargetResolver.TryParse("My.Ns.Type+Do,  ", out _, out _, out _);
                return new ParsedOutcome(full, type, method, assembly, bare, bareType, bareMethod, bareAssembly, noAssembly);
            })
            .TheTest
            .ShouldPass<ParsedOutcome>((because, outcome) =>
            {
                because.ItsTrue("a fully qualified identifier parses", outcome.Full);
                because.ItsTrue("the nested type keeps its inner '+'", outcome.Type == "My.Ns.Outer+Inner");
                because.ItsTrue("the method is the segment after the last '+'", outcome.Method == "Method");
                because.ItsTrue("only the assembly simple name is kept", outcome.Assembly == "my.assembly");
                because.ItsTrue("a bare identifier parses", outcome.Bare && outcome.BareType == "My.Ns.Type" && outcome.BareMethod == "Do" && outcome.BareAssembly == "my.assembly");
                because.ItsTrue("a blank assembly name is refused", !outcome.NoAssembly);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record SurfaceOutcome(InvocationTarget? Own, InvocationTarget? InheritedFromUnregistered, InvocationTarget? InheritedFromRegistered, InvocationTarget? BaseByOwnName, InvocationTarget? Generic, InvocationTarget? Unattributed, InvocationTarget? InterfaceGranted, InvocationTarget? InterfaceUngranted, InvocationTarget? ImplementationByName);

    private sealed record FrozenOutcome(InvocationTarget? ImplementationBefore, bool RegistryGrew, InvocationTarget? ImplementationAfter, InvocationTarget? HelperAfter, InvocationTarget? InterfaceStill, InvocationTarget? Listed, InvocationTarget? Unlisted, string? UnregisteredListing);

    private sealed record ResolvedOutcome(InvocationTarget? Echo, InvocationTarget? Ping, InvocationTarget? Nested, InvocationTarget? Privileged, InvocationTarget? Open, InvocationTarget? FromFor);

    private sealed record RefusedOutcome(InvocationTarget? Unregistered, InvocationTarget? ToStringMember, InvocationTarget? GetHashCodeMember, InvocationTarget? Ambiguous, InvocationTarget? Missing, InvocationTarget? WrongAssembly, InvocationTarget? Bcl, InvocationTarget? Null, InvocationTarget? Empty, InvocationTarget? NoAssembly, InvocationTarget? NoMethod, InvocationTarget? EmptyType, InvocationTarget? EmptyMethod, InvocationTarget? AssemblyLoad);

    private sealed record ParsedOutcome(bool Full, string Type, string Method, string Assembly, bool Bare, string BareType, string BareMethod, string BareAssembly, bool NoAssembly);
}

[UnitTestMenu("InvocationAccessRule Should", Selector = "iar")]
public class InvocationAccessRuleShould : UnitTestMenuContainer
{
    private static InvocationTarget Target(BamAccess required, bool? explicitProof)
    {
        MethodInfo method = typeof(TestInvocationService).GetMethod("Echo")!;
        return new InvocationTarget(typeof(TestInvocationService), method, required, explicitProof, false);
    }

    [UnitTest]
    public void GrantWhenHeldReachesRequiredAndNeverForDenied()
    {
        When.A<ActorAuthenticationOptions>("compares held against required",
            () => new ActorAuthenticationOptions(),
            (_) => new AccessOutcome(
                InvocationAccessRule.IsAllowed(BamAccess.Execute, BamAccess.Execute),
                InvocationAccessRule.IsAllowed(BamAccess.Write, BamAccess.Read),
                InvocationAccessRule.IsAllowed(BamAccess.Read, BamAccess.Execute),
                InvocationAccessRule.IsAllowed(BamAccess.Denied, BamAccess.Read),
                InvocationAccessRule.IsAllowed(BamAccess.Write, BamAccess.Denied)))
            .TheTest
            .ShouldPass<AccessOutcome>((because, outcome) =>
            {
                because.ItsTrue("equal levels pass", outcome.Equal);
                because.ItsTrue("a higher held level passes", outcome.Higher);
                because.ItsTrue("a lower held level fails", !outcome.Lower);
                because.ItsTrue("a Denied actor fails", !outcome.DeniedActor);
                because.ItsTrue("a Denied requirement fails even for Write", !outcome.DeniedTarget);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RequireProofFromTheExplicitAttributeElseTheThreshold()
    {
        When.A<ActorAuthenticationOptions>("derives the proof requirement",
            () => new ActorAuthenticationOptions { ProofRequiredAtOrAbove = BamAccess.Execute },
            (options) => new ProofOutcome(
                InvocationAccessRule.RequiresProof(Target(BamAccess.Execute, null), options),
                InvocationAccessRule.RequiresProof(Target(BamAccess.Read, null), options),
                InvocationAccessRule.RequiresProof(Target(BamAccess.Read, true), options),
                InvocationAccessRule.RequiresProof(Target(BamAccess.Write, false), options)))
            .TheTest
            .ShouldPass<ProofOutcome>((because, outcome) =>
            {
                because.ItsTrue("Execute at an Execute threshold requires proof", outcome.AtThreshold);
                because.ItsTrue("Read below the threshold does not", !outcome.BelowThreshold);
                because.ItsTrue("an explicit true forces proof below the threshold", outcome.ExplicitOn);
                because.ItsTrue("an explicit false waives proof above the threshold", !outcome.ExplicitOff);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record AccessOutcome(bool Equal, bool Higher, bool Lower, bool DeniedActor, bool DeniedTarget);

    private sealed record ProofOutcome(bool AtThreshold, bool BelowThreshold, bool ExplicitOn, bool ExplicitOff);
}

[UnitTestMenu("AmbientActorContext Should", Selector = "aac")]
public class AmbientActorContextShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ScopeTheCallerToTheAsyncFlowAndRestoreOnDispose()
    {
        When.A<AmbientActorContext>("enters, reads, nests and restores",
            () => new AmbientActorContext(),
            (ambient) =>
            {
                bool activeBefore = ambient.IsActive;
                string? refusal = null;
                try
                {
                    _ = ambient.Actor;
                }
                catch (InvalidOperationException ex)
                {
                    refusal = ex.Message;
                }

                string outer;
                string inner;
                string afterInner;
                using (AmbientActorContext.Enter(new ActorInvocationContext(new TestActor("alice"), "fp-alice", "10.0.0.1")))
                {
                    outer = ambient.Actor.Handle + "/" + ambient.KeyFingerprint + "/" + ambient.ClientIp;
                    using (AmbientActorContext.Enter(new ActorInvocationContext(new TestActor("bob"), null, null)))
                    {
                        inner = ambient.Actor.Handle + "/" + (ambient.KeyFingerprint ?? "-") + "/" + (ambient.ClientIp ?? "-");
                    }

                    afterInner = ambient.Actor.Handle;
                }

                return new ScopeOutcome(activeBefore, refusal, outer, inner, afterInner, ambient.IsActive);
            })
            .TheTest
            .ShouldPass<ScopeOutcome>((because, outcome) =>
            {
                because.ItsTrue("nothing is active before Enter", !outcome.ActiveBefore);
                because.ItsTrue("reading without an invocation throws InvalidOperationException", outcome.Refusal?.Contains("No actor invocation", StringComparison.Ordinal) == true);
                because.ItsTrue("the entered caller is visible", outcome.Outer == "alice/fp-alice/10.0.0.1");
                because.ItsTrue("a nested Enter shadows it", outcome.Inner == "bob/-/-");
                because.ItsTrue("disposing the nested scope restores the outer caller", outcome.AfterInner == "alice");
                because.ItsTrue("disposing the last scope clears the flow", !outcome.ActiveAfter);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void IsolateConcurrentInvocations()
    {
        When.A<AmbientActorContext>("runs two invocations on parallel async flows",
            () => new AmbientActorContext(),
            (ambient) =>
            {
                async Task<string> RunAsync(string handle)
                {
                    using (AmbientActorContext.Enter(new ActorInvocationContext(new TestActor(handle), null, null)))
                    {
                        await Task.Delay(20).ConfigureAwait(false);
                        string seen = ambient.Actor.Handle;
                        await Task.Yield();
                        return seen + "|" + ambient.Actor.Handle;
                    }
                }

                Task<string> alice = Task.Run(() => RunAsync("alice"));
                Task<string> bob = Task.Run(() => RunAsync("bob"));
                Task.WaitAll(alice, bob);
                return new ConcurrencyOutcome(alice.Result, bob.Result, ambient.IsActive);
            })
            .TheTest
            .ShouldPass<ConcurrencyOutcome>((because, outcome) =>
            {
                because.ItsTrue("alice's flow only ever sees alice", outcome.Alice == "alice|alice");
                because.ItsTrue("bob's flow only ever sees bob", outcome.Bob == "bob|bob");
                because.ItsTrue("the test's own flow was never entered", !outcome.CallerActive);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record ScopeOutcome(bool ActiveBefore, string? Refusal, string Outer, string Inner, string AfterInner, bool ActiveAfter);

    private sealed record ConcurrencyOutcome(string Alice, string Bob, bool CallerActive);
}

[UnitTestMenu("BamInvocationHandler Should", Selector = "bih")]
public class BamInvocationHandlerShould : UnitTestMenuContainer
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 24, 12, 0, 0, TimeSpan.Zero);

    /// <summary>Proof is only demanded at Write, so the Execute-level fixture methods run without a body signature.</summary>
    private static readonly ActorAuthenticationOptions ProofAtWrite = new ActorAuthenticationOptions { ProofRequiredAtOrAbove = BamAccess.Write };

    private static ServiceRegistry Registry()
    {
        ServiceRegistry registry = new ServiceRegistry();
        registry.For<IActorContext>().Use(new AmbientActorContext());
        registry.For<TestInvocationService>().Use<TestInvocationService>();
        registry.For<UnattributedService>().Use<UnattributedService>();
        return registry;
    }

    private static BamInvocationHandler Handler(BamAccess held, ActorAuthenticationOptions? authentication = null, BamInvocationOptions? options = null, CapturingLogger<BamInvocationHandler>? logger = null, ServiceRegistry? registry = null)
    {
        BamInvocationDispatchRegistry targets = new BamInvocationDispatchRegistry(registry ?? Registry());
        ActorAuthenticationOptions effective = authentication ?? ProofAtWrite;
        return new BamInvocationHandler(
            new RegisteredTypeInvocationTargetResolver(targets),
            targets,
            new FixedAccessPolicy(held),
            Evaluators.Create(new FixedClock(Now), effective),
            effective,
            options ?? new BamInvocationOptions(),
            logger);
    }

    private static DefaultHttpContext Request(string body, IActor? actor, string? pem = null)
    {
        DefaultHttpContext context = new DefaultHttpContext().At("POST", "/invoke").WithBody(body);
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.1.2.3");
        if (actor is not null)
        {
            context.SetActor(actor, pem);
        }

        return context;
    }

    private static string Run(BamInvocationHandler handler, DefaultHttpContext context)
    {
        IResult result = handler.HandleAsync(context).GetAwaiter().GetResult();
        return Invocations.StatusOf(result) + ":" + Invocations.BodyOf(result);
    }

    [UnitTest]
    public void InvokeARegisteredMethodForAnAuthorizedActorAndReturnBareJson()
    {
        TestActor alice = new TestActor("alice");
        When.A<BamInvocationHandler>("runs Echo, Add, Describe, EchoAsync, DoNothingAsync, Ping and WhoAmI",
            () => Handler(BamAccess.Execute),
            (handler) => new HappyOutcome(
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "hi"))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Add", Invocations.Args(("left", 2), ("right", 40))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Describe", Invocations.Args(("payload", new TestInvocationPayload { Name = "widget", Count = 21 }))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("EchoAsync", Invocations.Args(("text", "there"))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("DoNothingAsync"), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Ping"), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("WhoAmI"), alice))))
            .TheTest
            .ShouldPass<HappyOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a string result is bare JSON ({outcome.Echo})", outcome.Echo == "200:\"Echo: hi\"");
                because.ItsTrue($"ints are coerced and summed ({outcome.Add})", outcome.Add == "200:42");
                because.ItsTrue($"a POCO argument binds and a record result serializes ({outcome.Describe})", outcome.Describe == "200:" + JsonConvert.SerializeObject(new TestInvocationReply("widget", 42)));
                because.ItsTrue($"Task<string> is awaited and unwrapped ({outcome.EchoAsync})", outcome.EchoAsync == "200:\"Async: there\"");
                because.ItsTrue($"a plain Task yields null ({outcome.DoNothing})", outcome.DoNothing == "200:null");
                because.ItsTrue($"a static method needs no instance ({outcome.Ping})", outcome.Ping == "200:\"pong\"");
                because.ItsTrue($"the service sees the verified actor and client IP through IActorContext ({outcome.WhoAmI})", outcome.WhoAmI == "200:\"alice@10.1.2.3\"");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseMalformedUnknownAnonymousAndUnauthorizedRequests()
    {
        TestActor alice = new TestActor("alice");
        TestActor anonymous = new TestActor(Bam.Protocol.Server.AnonymousActorProvider.AnonymousHandle);
        When.A<BamInvocationHandler>("answers 400, 461, 403 and 401 before touching a service",
            () => Handler(BamAccess.Execute),
            (handler) => new RefusalOutcome(
                Run(handler, Request("this is not json", alice)),
                Run(handler, Request("{}", alice)),
                Run(handler, Request("{\"OperationIdentifier\":\"\"}", alice)),
                Run(handler, Request(Invocations.Envelope<UnregisteredService>("Secret"), alice)),
                Run(handler, Request(Invocations.Envelope("System.Reflection.Assembly+Load, System.Private.CoreLib", Invocations.Args(("assemblyString", "evil"))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Open"), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "x"))), null)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "x"))), anonymous)),
                Run(Handler(BamAccess.Read), Request(Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "x"))), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Privileged"), alice)),
                Run(handler, Request(Invocations.Envelope<TestInvocationService>("Add", Invocations.Args(("left", "two"), ("right", 1))), alice)),
                Run(Handler(BamAccess.Write), Request(Invocations.Envelope<UnattributedService>("Hi"), alice))))
            .TheTest
            .ShouldPass<RefusalOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a registered type with no access attribute is Denied even to Write ({outcome.Unattributed})", outcome.Unattributed.StartsWith("403:", StringComparison.Ordinal) && outcome.Unattributed.Contains("Denied", StringComparison.Ordinal));
                because.ItsTrue($"non-JSON is 400 ({outcome.NotJson})", outcome.NotJson.StartsWith("400:", StringComparison.Ordinal));
                because.ItsTrue($"an envelope without an operation is 400 ({outcome.NoOperation})", outcome.NoOperation.StartsWith("400:", StringComparison.Ordinal));
                because.ItsTrue($"a blank operation is 400 ({outcome.BlankOperation})", outcome.BlankOperation.StartsWith("400:", StringComparison.Ordinal));
                because.ItsTrue($"an unregistered type is 461 ({outcome.Unregistered})", outcome.Unregistered == "461:The operation is not available.");
                because.ItsTrue($"Assembly.Load is 461, never executed ({outcome.AssemblyLoad})", outcome.AssemblyLoad == "461:The operation is not available.");
                because.ItsTrue($"an anonymous-marked operation is 403 ({outcome.AnonymousOperation})", outcome.AnonymousOperation.StartsWith("403:", StringComparison.Ordinal));
                because.ItsTrue($"no actor is the uniform 401 ({outcome.NoActor})", outcome.NoActor == "401:" + ActorAuthenticationMiddleware.FailureMessage);
                because.ItsTrue($"the anonymous sentinel is the uniform 401 ({outcome.AnonymousActor})", outcome.AnonymousActor == "401:" + ActorAuthenticationMiddleware.FailureMessage);
                because.ItsTrue($"Read against an Execute method is 403 ({outcome.Insufficient})", outcome.Insufficient.StartsWith("403:", StringComparison.Ordinal) && outcome.Insufficient.Contains("Execute", StringComparison.Ordinal));
                because.ItsTrue($"Execute against a Write method is 403 ({outcome.Privileged})", outcome.Privileged.StartsWith("403:", StringComparison.Ordinal) && outcome.Privileged.Contains("Write", StringComparison.Ordinal));
                because.ItsTrue($"an unconvertible argument is 400 naming the parameter ({outcome.BadArgument})", outcome.BadArgument.StartsWith("400:", StringComparison.Ordinal) && outcome.BadArgument.Contains("'left'", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void HideTargetExceptionsBehindAGeneric500AndLogThem()
    {
        CapturingLogger<BamInvocationHandler> logger = new CapturingLogger<BamInvocationHandler>();
        When.A<BamInvocationHandler>("runs a method that throws",
            () => Handler(BamAccess.Execute, logger: logger),
            (handler) =>
            {
                string response = Run(handler, Request(Invocations.Envelope<TestInvocationService>("Fail"), new TestActor("alice")));
                bool logged = logger.Entries.Any(entry => entry.Key == Microsoft.Extensions.Logging.LogLevel.Error && entry.Value.Contains("Fail", StringComparison.Ordinal));
                return new FailureOutcome(response, logged);
            })
            .TheTest
            .ShouldPass<FailureOutcome>((because, outcome) =>
            {
                because.ItsTrue($"the caller gets the generic 500 ({outcome.Response})", outcome.Response == "500:" + BamInvocationHandler.InvocationFailedMessage);
                because.ItsTrue("the exception message does not leak", !outcome.Response.Contains("secret detail", StringComparison.Ordinal));
                because.ItsTrue("the host logged the failure at Error", outcome.Logged);
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void CapTheBodyAndDemandProofWhenTheTargetRequiresIt()
    {
        EccKeyPair pair = TestKeys.NewEcc();
        TestActor alice = new TestActor("alice");
        string body = Invocations.Envelope<TestInvocationService>("Echo", Invocations.Args(("text", "proved")));
        When.A<BamInvocationHandler>("enforces MaxBodyBytes and the proof threshold",
            () => Handler(BamAccess.Execute, authentication: new ActorAuthenticationOptions { ProofRequiredAtOrAbove = BamAccess.Execute }),
            (proving) =>
            {
                BamInvocationHandler tiny = Handler(BamAccess.Execute, options: new BamInvocationOptions { MaxBodyBytes = 16 });
                DefaultHttpContext declared = Request(body, alice);
                DefaultHttpContext undeclared = Request(body, alice);
                undeclared.Request.ContentLength = null;

                DefaultHttpContext unproved = Request(body, alice, pair.PublicPem);
                DefaultHttpContext proved = Request(body, alice, pair.PublicPem).Proved(pair, Now);
                DefaultHttpContext forged = Request(body, alice, pair.PublicPem).Proved(TestKeys.NewEcc(), Now);

                return new ProofOutcome(Run(tiny, declared), Run(tiny, undeclared), Run(proving, unproved), Run(proving, proved), Run(proving, forged));
            })
            .TheTest
            .ShouldPass<ProofOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a declared oversize body is 413 ({outcome.Declared})", outcome.Declared.StartsWith("413:", StringComparison.Ordinal));
                because.ItsTrue($"an undeclared oversize body is 413 once read ({outcome.Undeclared})", outcome.Undeclared.StartsWith("413:", StringComparison.Ordinal));
                because.ItsTrue($"a target at the proof threshold refuses an unsigned body ({outcome.Unproved})", outcome.Unproved.StartsWith("401:", StringComparison.Ordinal));
                because.ItsTrue($"a correctly signed body runs ({outcome.Proved})", outcome.Proved == "200:\"Echo: proved\"");
                because.ItsTrue($"a body signed by another key is 401 ({outcome.Forged})", outcome.Forged.StartsWith("401:", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void KeepInheritedNamesAndConstructedTypesOutOfReach()
    {
        TestActor alice = new TestActor("alice");
        When.A<ServiceRegistry>("refuses a base method under a derived name and a constructor dependency the registry cached",
            () =>
            {
                ServiceRegistry registry = Registry();
                registry.For<WriteBase>().Use<WriteBase>();
                registry.For<DerivedOfWrite>().Use<DerivedOfWrite>();
                registry.For<NeedsHelper>().Use<NeedsHelper>();
                return registry;
            },
            (registry) =>
            {
                BamInvocationHandler asExecute = Handler(BamAccess.Execute, registry: registry);
                BamInvocationHandler asRead = Handler(BamAccess.Read, registry: registry);
                return new ReachOutcome(
                    Run(asExecute, Request(Invocations.Envelope<DerivedOfWrite>("FromWriteBase"), alice)),
                    Run(asExecute, Request(Invocations.Envelope<WriteBase>("FromWriteBase"), alice)),
                    Run(asExecute, Request(Invocations.Envelope<DerivedOfWrite>("OwnOfWrite"), alice)),
                    Run(asExecute, Request(Invocations.Envelope<NeedsHelper>("Do"), alice)),
                    Run(asRead, Request(Invocations.Envelope<Helper>("Leak"), alice)));
            })
            .TheTest
            .ShouldPass<ReachOutcome>((because, outcome) =>
            {
                because.ItsTrue($"a Write-only base method is not reachable through the Execute derived type's name ({outcome.BaseViaDerived})", outcome.BaseViaDerived.StartsWith("461:", StringComparison.Ordinal));
                because.ItsTrue($"under its own name the base's own rule applies ({outcome.BaseByOwnName})", outcome.BaseByOwnName.StartsWith("403:", StringComparison.Ordinal));
                because.ItsTrue($"the derived type's own method runs ({outcome.DerivedOwn})", outcome.DerivedOwn == "200:\"own-of-write\"");
                because.ItsTrue($"a service with an unregistered constructor dependency runs ({outcome.NeedsHelper})", outcome.NeedsHelper == "200:\"did\"");
                because.ItsTrue($"the dependency the registry constructed and cached is still not invocable ({outcome.HelperLeak})", outcome.HelperLeak.StartsWith("461:", StringComparison.Ordinal));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void AcceptTheExactBamClientBodyAndIgnorePolymorphicTypeHints()
    {
        TestActor alice = new TestActor("alice");
        When.A<BamInvocationHandler>("reads a System.Text.Json envelope, refuses $type under poisoned defaults and answers 401 before reading",
            () => Handler(BamAccess.Execute),
            (handler) =>
            {
                MethodInvocationRequest request = MethodInvocationRequest.For(typeof(TestInvocationService), "Ping");
                request.ClientInitialize();
                string stjBody = System.Text.Json.JsonSerializer.Serialize((object)request);
                string stj = Run(handler, Request(stjBody, alice));

                string trap = typeof(BoobyTrap).AssemblyQualifiedName!;
                string poisonedEnvelope = "{\"$type\":\"" + trap + "\",\"OperationIdentifier\":" + JsonConvert.ToString(Invocations.Operation<TestInvocationService>("Echo")) + ",\"Arguments\":{\"text\":\"safe\"}}";
                string describeEnvelope = Invocations.Envelope<TestInvocationService>("Describe", Invocations.Args(("payload", new TestInvocationPayload { Name = "w", Count = 1 })));
                Func<JsonSerializerSettings>? previous = JsonConvert.DefaultSettings;
                string poisoned;
                string poisonedResponse;
                try
                {
                    JsonConvert.DefaultSettings = () => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                    poisoned = Run(handler, Request(poisonedEnvelope, alice));
                    poisonedResponse = Run(handler, Request(describeEnvelope, alice));
                }
                finally
                {
                    JsonConvert.DefaultSettings = previous;
                }

                DefaultHttpContext unauthenticated = Request(Invocations.Envelope<TestInvocationService>("Ping"), null);
                string refused = Run(handler, unauthenticated);
                bool bodyRead = unauthenticated.Request.Body.Position != 0;

                string elided = BamInvocationHandler.Describe(new string('x', BamInvocationHandler.LoggedOperationLength + 50));
                return new WireOutcome(stj, poisoned, poisonedResponse, bodyRead, refused + "|" + elided.Length);
            })
            .TheTest
            .ShouldPass<WireOutcome>((because, outcome) =>
            {
                because.ItsTrue($"the exact body BamClient<T> serializes with System.Text.Json invokes the method ({outcome.StjBody})", outcome.StjBody == "200:\"pong\"");
                because.ItsTrue($"a $type on the envelope is ignored under poisoned DefaultSettings ({outcome.Poisoned})", outcome.Poisoned == "200:\"Echo: safe\"");
                because.ItsTrue($"the response carries no $type under poisoned DefaultSettings ({outcome.PoisonedResponse})", outcome.PoisonedResponse == "200:{\"Name\":\"w\",\"Doubled\":2}");
                because.ItsTrue("an unauthenticated request is refused before any of the body is read", !outcome.BodyRead);
                because.ItsTrue($"the refusal is the uniform 401 and long identifiers are cut for logs ({outcome.Elided})", outcome.Elided == "401:" + ActorAuthenticationMiddleware.FailureMessage + "|" + (BamInvocationHandler.LoggedOperationLength + 1));
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    [UnitTest]
    public void UnwrapTasksOfEveryShape()
    {
        When.A<AmbientActorContext>("awaits Task, Task<T>, ValueTask-free results and plain values",
            () => new AmbientActorContext(),
            (_) => new UnwrapOutcome(
                BamInvocationHandler.UnwrapAsync(Task.FromResult(7)).GetAwaiter().GetResult(),
                BamInvocationHandler.UnwrapAsync(Task.CompletedTask).GetAwaiter().GetResult(),
                BamInvocationHandler.UnwrapAsync("plain").GetAwaiter().GetResult(),
                BamInvocationHandler.UnwrapAsync(null).GetAwaiter().GetResult(),
                BamInvocationHandler.UnwrapAsync(Task.Run(async () => { await Task.Yield(); return "later"; })).GetAwaiter().GetResult()))
            .TheTest
            .ShouldPass<UnwrapOutcome>((because, outcome) =>
            {
                because.ItsTrue("Task<int> yields its value", outcome.Generic is int value && value == 7);
                because.ItsTrue("a plain Task yields null", outcome.Plain is null);
                because.ItsTrue("a non-task passes through", outcome.Value is "plain");
                because.ItsTrue("null passes through", outcome.Null is null);
                because.ItsTrue("an unfinished Task<string> is awaited", outcome.Deferred is "later");
            })
            .SoBeHappy()
            .UnlessItFailed();
    }

    private sealed record HappyOutcome(string Echo, string Add, string Describe, string EchoAsync, string DoNothing, string Ping, string WhoAmI);

    private sealed record RefusalOutcome(string NotJson, string NoOperation, string BlankOperation, string Unregistered, string AssemblyLoad, string AnonymousOperation, string NoActor, string AnonymousActor, string Insufficient, string Privileged, string BadArgument, string Unattributed);

    private sealed record WireOutcome(string StjBody, string Poisoned, string PoisonedResponse, bool BodyRead, string Elided);

    private sealed record ReachOutcome(string BaseViaDerived, string BaseByOwnName, string DerivedOwn, string NeedsHelper, string HelperLeak);

    private sealed record FailureOutcome(string Response, bool Logged);

    private sealed record ProofOutcome(string Declared, string Undeclared, string Unproved, string Proved, string Forged);

    private sealed record UnwrapOutcome(object? Generic, object? Plain, object? Value, object? Null, object? Deferred);
}
