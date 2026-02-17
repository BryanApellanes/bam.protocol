using Bam.Protocol.Data;
using Bam.Protocol.Server;
using Bam.Test;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("AuthorizationCalculator should")]
public class AuthorizationCalculatorShould : UnitTestMenuContainer
{
    [RequiredAccess(BamAccess.Read)]
    public class ReadService
    {
        public void ReadMethod() { }

        [RequiredAccess(BamAccess.Write)]
        public void WriteMethod() { }
    }

    [RequiredAccess(BamAccess.Write)]
    public class WriteService
    {
        [RequiredAccess(BamAccess.Read)]
        public void ReadMethod() { }

        public void WriteMethod() { }
    }

    public class NoAttributeService
    {
        public void SomeMethod() { }
    }

    private static IBamServerContext CreateMockContext(BamAccess actorAccess, string typeName, string methodName)
    {
        IAccessLevelProvider accessLevelProvider = Substitute.For<IAccessLevelProvider>();
        IBamServerContext context = Substitute.For<IBamServerContext>();
        ICommand command = new Command
        {
            TypeName = typeName,
            MethodName = methodName
        };
        context.Command.Returns(command);
        accessLevelProvider.GetAccessLevel(context).Returns(actorAccess);
        return context;
    }

    private static IAccessLevelProvider CreateMockProvider(IBamServerContext context, BamAccess actorAccess)
    {
        IAccessLevelProvider provider = Substitute.For<IAccessLevelProvider>();
        provider.GetAccessLevel(context).Returns(actorAccess);
        return provider;
    }

    [UnitTest]
    public void GrantWriteAccessWhenActorHasWrite()
    {
        string typeName = typeof(ReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Write, typeName, "WriteMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Write);

        When.A<AuthorizationCalculator>("grants write access when actor has write",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Write", result.Access == BamAccess.Write);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GrantReadAccessWhenActorHasRead()
    {
        string typeName = typeof(ReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Read, typeName, "ReadMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Read);

        When.A<AuthorizationCalculator>("grants read access when actor has read",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Read", result.Access == BamAccess.Read);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DenyAccessWhenActorHasDenied()
    {
        string typeName = typeof(ReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeName, "WriteMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("denies access when actor has denied",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Denied", result.Access == BamAccess.Denied);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DenyWhenActorHasReadButMethodRequiresWrite()
    {
        string typeName = typeof(ReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Read, typeName, "WriteMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Read);

        When.A<AuthorizationCalculator>("denies when actor has read but method requires write",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Denied", result.Access == BamAccess.Denied);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UseMethodAttributeOverClassAttribute()
    {
        string typeName = typeof(WriteService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Read, typeName, "ReadMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Read);

        When.A<AuthorizationCalculator>("uses method attribute over class attribute",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Read (method override)", result.Access == BamAccess.Read);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DenyByDefaultWhenNoAttribute()
    {
        string typeName = typeof(NoAttributeService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Write, typeName, "SomeMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Write);

        When.A<AuthorizationCalculator>("denies by default when no attribute",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Denied", result.Access == BamAccess.Denied);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GrantWhenActorExceedsRequired()
    {
        string typeName = typeof(ReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Write, typeName, "ReadMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Write);

        When.A<AuthorizationCalculator>("grants when actor exceeds required",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Read (required level)", result.Access == BamAccess.Read);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
