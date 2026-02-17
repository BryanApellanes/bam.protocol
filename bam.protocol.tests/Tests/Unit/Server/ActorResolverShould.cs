using Bam;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("ActorResolver should")]
public class ActorResolverShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ResolveActorFromClientPublicKey()
    {
        string clientPublicKey = 64.RandomLetters();
        string personHandle = 8.RandomLetters();
        string profileName = "Test User";

        When.A<ActorResolver>("resolves actor from client public key",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                IProfile profile = Substitute.For<IProfile>();
                profile.PersonHandle.Returns(personHandle);
                profile.Name.Returns(profileName);
                profileManager.FindProfileByPublicKey(clientPublicKey.Sha256()).Returns(profile);
                return new ActorResolver(profileManager);
            },
            (resolver) =>
            {
                IBamServerContext context = CreateMockContext(clientPublicKey);
                return resolver.ResolveActor(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            IActor actor = because.TheResult.As<IActor>();
            because.ItsTrue("handle matches person handle", actor.Handle == personHandle);
            because.ItsTrue("name matches profile name", actor.Name == profileName);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReturnNullWhenNoClientPublicKey()
    {
        When.A<ActorResolver>("returns null when no client public key",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                return new ActorResolver(profileManager);
            },
            (resolver) =>
            {
                IBamServerContext context = Substitute.For<IBamServerContext>();
                IServerSessionState sessionState = Substitute.For<IServerSessionState>();
                sessionState.Get<string>("ClientPublicKey").Returns((string)null!);
                context.ServerSessionState.Returns(sessionState);
                return resolver.ResolveActor(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReturnNullWhenProfileNotFound()
    {
        string clientPublicKey = 64.RandomLetters();

        When.A<ActorResolver>("returns null when profile not found",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                profileManager.FindProfileByPublicKey(clientPublicKey.Sha256()).Returns((IProfile)null!);
                return new ActorResolver(profileManager);
            },
            (resolver) =>
            {
                IBamServerContext context = CreateMockContext(clientPublicKey);
                return resolver.ResolveActor(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReturnNullWhenNoSessionState()
    {
        When.A<ActorResolver>("returns null when no session state",
            () =>
            {
                IProfileManager profileManager = Substitute.For<IProfileManager>();
                return new ActorResolver(profileManager);
            },
            (resolver) =>
            {
                IBamServerContext context = Substitute.For<IBamServerContext>();
                context.ServerSessionState.Returns((IServerSessionState)null!);
                return resolver.ResolveActor(context);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static IBamServerContext CreateMockContext(string clientPublicKey)
    {
        IBamServerContext context = Substitute.For<IBamServerContext>();
        IServerSessionState sessionState = Substitute.For<IServerSessionState>();
        sessionState.Get<string>("ClientPublicKey").Returns(clientPublicKey);
        context.ServerSessionState.Returns(sessionState);
        return context;
    }
}
