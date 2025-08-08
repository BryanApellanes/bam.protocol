using Bam.Web;
using System.Text;

namespace Bam.Protocol
{
    public interface IHttpRequest
    {
        Uri Uri { get; set; }
        IDictionary<string, string> Headers { get; }
        string ContentType { get; set; }
        HttpVerbs Verb { get; set; }
        string Content { get; set; }
        Encoding Encoding { get; set; }
        HttpRequestMessage ToHttpRequestMessage(string url);
        HttpRequestMessage ToHttpRequestMessage();
    }
}
