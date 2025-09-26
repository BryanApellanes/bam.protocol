
namespace Bam.Protocol;

public interface IBamResponse<T>: IBamResponse
{
    T Content { get; }
}