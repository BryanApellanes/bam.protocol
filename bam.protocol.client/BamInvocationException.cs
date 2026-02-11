using System;

namespace Bam.Protocol.Client
{
    public class BamInvocationException : Exception
    {
        public BamInvocationException(Type serviceType, string methodName, int statusCode, string responseContent)
            : base($"Invocation of {serviceType.Name}.{methodName} failed with status {statusCode}: {responseContent}")
        {
            ServiceType = serviceType;
            MethodName = methodName;
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public Type ServiceType { get; }
        public string MethodName { get; }
        public int StatusCode { get; }
        public string ResponseContent { get; }
    }
}
