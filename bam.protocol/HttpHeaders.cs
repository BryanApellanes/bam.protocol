using Bam.Web;

namespace Bam.Encryption
{
    public static class HttpHeaders
    {
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

        public static List<string> CipherHeaders
        {
            get
            {
                return PlainHeaders.Select(header => $"{header}-Cipher").ToList();
            }
        }
    }
}
