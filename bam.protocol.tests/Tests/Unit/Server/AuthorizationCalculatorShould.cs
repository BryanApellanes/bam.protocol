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

    [RequiredAccess(BamAccess.Execute)]
    public class ExecuteService
    {
        public void ExecuteMethod() { }

        [RequiredAccess(BamAccess.Write)]
        public void WriteMethod() { }
    }

    [AnonymousAccess]
    [RequiredAccess(BamAccess.Read)]
    public class AnonymousReadService
    {
        public void ReadMethod() { }

        [AnonymousAccess(false)]
        public void ProtectedMethod() { }
    }

    [AnonymousAccess]
    public class AnonymousNoRequiredAccessService
    {
        public void SomeMethod() { }
    }

    [AnonymousAccess(encryptionRequired: true)]
    [RequiredAccess(BamAccess.Execute)]
    public class EncryptedAnonymousService
    {
        public void SecureMethod() { }
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

    [UnitTest]
    public void GrantExecuteAccessWhenActorHasExecute()
    {
        string typeName = typeof(ExecuteService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Execute, typeName, "ExecuteMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Execute);

        When.A<AuthorizationCalculator>("grants execute access when actor has execute",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Execute", result.Access == BamAccess.Execute);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void DenyExecuteWhenActorHasRead()
    {
        string typeName = typeof(ExecuteService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Read, typeName, "ExecuteMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Read);

        When.A<AuthorizationCalculator>("denies execute when actor has read",
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
    public void GrantAnonymousAccessWhenAttributePresent()
    {
        string typeName = typeof(AnonymousReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeName, "ReadMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("grants anonymous access when attribute present",
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
    public void DenyAnonymousWhenMethodOverridesWithFalse()
    {
        string typeName = typeof(AnonymousReadService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeName, "ProtectedMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("denies anonymous when method overrides with false",
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
    public void DefaultToExecuteForAnonymousWithNoRequiredAccess()
    {
        string typeName = typeof(AnonymousNoRequiredAccessService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeName, "SomeMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("defaults to execute for anonymous with no required access",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Execute", result.Access == BamAccess.Execute);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GrantEncryptedAnonymousAccessWhenAttributePresent()
    {
        string typeName = typeof(EncryptedAnonymousService).FullName!;
        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeName, "SecureMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("grants encrypted anonymous access when attribute present",
            () => new AuthorizationCalculator(provider),
            (calculator) => calculator.CalculateAuthorization(context))
        .TheTest
        .ShouldPass(because =>
        {
            IAuthorizationCalculation result = because.TheResult.As<IAuthorizationCalculation>();
            because.ItsTrue("access is Execute", result.Access == BamAccess.Execute);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveEncryptionRequiredFromAttribute()
    {
        ICommand encryptedCommand = new Command
        {
            TypeName = typeof(EncryptedAnonymousService).FullName!,
            MethodName = "SecureMethod"
        };
        ICommand plainAnonymousCommand = new Command
        {
            TypeName = typeof(AnonymousReadService).FullName!,
            MethodName = "ReadMethod"
        };
        ICommand noAttributeCommand = new Command
        {
            TypeName = typeof(NoAttributeService).FullName!,
            MethodName = "SomeMethod"
        };

        IBamServerContext context = CreateMockContext(BamAccess.Denied, typeof(EncryptedAnonymousService).FullName!, "SecureMethod");
        IAccessLevelProvider provider = CreateMockProvider(context, BamAccess.Denied);

        When.A<AuthorizationCalculator>("resolves encryption required from attribute",
            () => new AuthorizationCalculator(provider),
            (_) =>
            {
                bool encryptedResult = CommandAttributeResolver.IsEncryptionRequired(encryptedCommand);
                bool plainResult = CommandAttributeResolver.IsEncryptionRequired(plainAnonymousCommand);
                bool noAttrResult = CommandAttributeResolver.IsEncryptionRequired(noAttributeCommand);
                return new { encryptedResult, plainResult, noAttrResult };
            })
        .TheTest
        .ShouldPass(because =>
        {
            dynamic result = because.TheResult.As<dynamic>();
            because.ItsTrue("encrypted anonymous returns true", (bool)result.encryptedResult == true);
            because.ItsTrue("plain anonymous returns false", (bool)result.plainResult == false);
            because.ItsTrue("no attribute returns false", (bool)result.noAttrResult == false);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
