using Bam.Web;

namespace Bam.Encryption
{
    /// <summary>
    /// Provides lists of plain-text and cipher HTTP header names used in encrypted communication.
    /// </summary>
    public static class HttpHeaders
    {
        /// <summary>
        /// Gets the list of plain-text header names used in Bam protocol communication.
        /// </summary>
        public static List<string> PlainHeaders
        {
            get
            {
                return new List<string>()
                {
                    Headers.ProcessLocalIdentifier,
                    Headers.ProcessDescriptor,
                    Headers.ProcessMode,
                    Headers.ApplicationName,
                    Headers.Hash
                };
            }
        }

        /// <summary>
        /// Gets the list of cipher header names, derived by appending "-Cipher" to each plain header name.
        /// </summary>
        public static List<string> CipherHeaders
        {
            get
            {
                return PlainHeaders.Select(header => $"{header}-Cipher").ToList();
            }
        }
    }
}
