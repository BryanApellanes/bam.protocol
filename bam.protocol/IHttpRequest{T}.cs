using Bam.Protocol;

namespace Bam.Encryption
{
    /// <summary>
    /// Defines a strongly-typed HTTP request with a typed content body.
    /// </summary>
    /// <typeparam name="TContent">The type of the request body content.</typeparam>
    public interface IHttpRequest<TContent> : IHttpRequest
    {
        /// <summary>
        /// Gets or sets the strongly-typed content body.
        /// </summary>
        new TContent TypedContent { get; set; }
    }
}
