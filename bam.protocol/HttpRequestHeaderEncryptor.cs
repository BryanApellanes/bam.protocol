using Bam.Protocol;

namespace Bam.Encryption
{
    /// <summary>
    /// Encrypts plain-text headers on an HTTP request by replacing them with cipher header equivalents.
    /// </summary>
    public class HttpRequestHeaderEncryptor : IHttpRequestHeaderEncryptor
    {
        /// <summary>
        /// Initializes a new instance with the specified encryptor.
        /// </summary>
        /// <param name="encryptor">The encryptor to use for header values.</param>
        public HttpRequestHeaderEncryptor(IEncryptor encryptor)
        {
            this.Encryptor = encryptor;
        }

        /// <summary>
        /// Gets the encryptor used for header values.
        /// </summary>
        public IEncryptor Encryptor { get; private set; }

        /// <summary>
        /// Encrypts plain-text headers on the specified request, replacing them with cipher header equivalents and adding a Content-Type cipher.
        /// </summary>
        /// <param name="request">The request whose headers should be encrypted.</param>
        public void EncryptHeaders(IHttpRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            if(request.Headers == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(request.ContentType))
            {
                request.Headers.Add("Content-Type-Cipher", Encryptor.Encrypt(request.ContentType));
            }
            foreach(string header in HttpHeaders.PlainHeaders)
            {
                if (request.Headers.ContainsKey(header))
                {
                    string plainHeaderValue = request.Headers[header];
                    request.Headers.Remove(header);
                    request.Headers.Add($"{header}-Cipher", Encryptor.Encrypt(plainHeaderValue));
                }
            }
        }
    }
}
