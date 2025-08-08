using Bam.Protocol;

namespace Bam.Encryption
{
    public interface IHttpRequest<TContent> : IHttpRequest
    {
        new TContent TypedContent { get; set; }
    }
}
