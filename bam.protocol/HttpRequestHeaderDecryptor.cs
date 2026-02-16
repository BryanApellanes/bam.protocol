using Bam.Protocol;

namespace Bam.Encryption
{
    /// <summary>
    /// Decrypts cipher headers on an HTTP request by replacing cipher header values with their decrypted plain-text equivalents.
    /// </summary>
    public class HttpRequestHeaderDecryptor : IHttpRequestHeaderDecryptor
    {
        /// <summary>
        /// Initializes a new instance with the specified decryptor.
        /// </summary>
        /// <param name="decryptor">The decryptor to use for header values.</param>
        public HttpRequestHeaderDecryptor(IDecryptor decryptor)
        {
            this.Decryptor = decryptor;
        }

        /// <summary>
        /// Gets the decryptor used for header values.
        /// </summary>
        public IDecryptor Decryptor { get; private set; }

        /// <summary>
        /// Decrypts cipher headers on the specified request, replacing cipher headers with their plain-text equivalents.
        /// </summary>
        /// <param name="request">The request whose cipher headers should be decrypted.</param>
        public void DecryptHeaders(IHttpRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            foreach(string header in HttpHeaders.CipherHeaders)
            {
                if(request.Headers.ContainsKey(header))
                {
                    string cipherHeaderValue = request.Headers[header];
                    request.Headers.Remove(header);
                    request.Headers.Add(header.Truncate("-Cipher".Length), Decryptor.Decrypt(cipherHeaderValue));
                }
            }
        }
    }
}
