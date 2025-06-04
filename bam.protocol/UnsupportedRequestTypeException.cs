using Bam.Protocol.Client;

namespace bam.protocol;

public class UnsupportedRequestTypeException : Exception
{
    public UnsupportedRequestTypeException(Type type) : base(
        $"Unsupported request type: {type.Name}")
    {
    }
}