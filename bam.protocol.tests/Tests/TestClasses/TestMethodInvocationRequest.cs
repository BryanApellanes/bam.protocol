using System.Reflection;
using Bam.DependencyInjection;

namespace Bam.Protocol;

public class TestMethodInvocationRequest : MethodInvocationRequest
{
    public TestMethodInvocationRequest() : base()
    {
    }

    public TestMethodInvocationRequest(MethodInfo methodInfo) : base(methodInfo)
    {
        
    }
    public object GetInstance()
    {
        return Instance;
    }
}