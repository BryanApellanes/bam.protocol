
namespace Bam.Protocol;

public class UnsupportedRequestTypeException : Exception
{
    public UnsupportedRequestTypeException(Type type) : base(
        $"Unsupported request type: {type.Name}")
    {
    }
}