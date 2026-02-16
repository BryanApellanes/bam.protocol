using Bam.Web;
using System.Text;
using Bam.Protocol;

namespace Bam.Encryption
{
    /// <summary>
    /// Represents an HTTP request with URI, headers, content, verb, and encoding, and supports conversion to <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class HttpRequest : IHttpRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class with default settings (localhost, UTF-8, JSON content type).
        /// </summary>
        public HttpRequest()
        {
            this.Uri = new Uri("https://localhost");
            this.Encoding = Encoding.UTF8;
            this.Headers = new Dictionary<string, string>();
            this.ContentType = MediaTypes.Json;
        }

        /// <summary>
        /// Gets or sets the URI for this request.
        /// </summary>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of request headers.
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the MIME content type.
        /// </summary>
        public virtual string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HTTP verb (GET, POST, etc.).
        /// </summary>
        public HttpVerbs Verb
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request body content as a string.
        /// </summary>
        public virtual string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the character encoding for the request content.
        /// </summary>
        public Encoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Copies all properties from the specified request to this instance.
        /// </summary>
        /// <param name="request">The request to copy from.</param>
        public virtual void Copy(IHttpRequest request)
        {
            this.Uri = request.Uri;
            this.Content = request.Content;
            this.ContentType = request.ContentType;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }

        /// <summary>
        /// Converts this request to an <see cref="HttpRequestMessage"/> with the specified URL.
        /// </summary>
        /// <param name="url">The URL to use for the request message.</param>
        /// <returns>An <see cref="HttpRequestMessage"/> configured with this request's properties.</returns>
        public HttpRequestMessage ToHttpRequestMessage(string url)
        {
            HttpRequestMessage requestMessage = ToHttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
            return requestMessage;
        }

        /// <summary>
        /// Converts this request to an <see cref="HttpRequestMessage"/> using the current URI.
        /// </summary>
        /// <returns>An <see cref="HttpRequestMessage"/> configured with this request's properties.</returns>
        public virtual HttpRequestMessage ToHttpRequestMessage()
        {
            HttpRequestMessage result = new HttpRequestMessage(MethodsByVerbs[Verb], Uri)
            {
                Content = new StringContent(Content, Encoding, ContentType)
            };
            foreach(string key in Headers.Keys)
            {
                result.Headers.Add(key, Headers[key]);
            }
            return result;
        }

        /// <summary>
        /// Creates an <see cref="HttpRequest"/> from an <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="httpRequestMessage">The HTTP request message to convert.</param>
        /// <returns>A new <see cref="HttpRequest"/> populated from the message.</returns>
        public static HttpRequest FromHttpRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            HttpRequest request = new HttpRequest
            {
                Uri = httpRequestMessage.RequestUri,
                Content = httpRequestMessage.Content.ReadAsStringAsync().Result,
                ContentType = httpRequestMessage.Content?.Headers?.ContentType?.MediaType,
                Verb = VerbsByMethod[httpRequestMessage.Method]
            };
            foreach(System.Collections.Generic.KeyValuePair<string, IEnumerable<string>> kvp in httpRequestMessage.Headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value.ToString());
            }
            return request;
        }

        /// <summary>
        /// Creates a strongly-typed <see cref="HttpRequest{TContent}"/> from an <see cref="HttpRequestMessage"/>, deserializing the body as JSON.
        /// </summary>
        /// <typeparam name="TContent">The type to deserialize the request body into.</typeparam>
        /// <param name="httpRequestMessage">The HTTP request message to convert.</param>
        /// <returns>A new <see cref="HttpRequest{TContent}"/> populated from the message.</returns>
        public static HttpRequest<TContent> FromHttpRequestMessage<TContent>(HttpRequestMessage httpRequestMessage)
        {
            HttpRequest<TContent> request = new HttpRequest<TContent>
            {
                Uri = httpRequestMessage.RequestUri,
                ContentType = httpRequestMessage.Content?.Headers?.ContentType?.MediaType,
                Verb = VerbsByMethod[httpRequestMessage.Method]
            };

            if (httpRequestMessage.Content != null && 
                httpRequestMessage.Content.ReadAsStringAsync().Result.TryFromJson<TContent>(out TContent content))
            {
                request.TypedContent = content;
            }
            foreach (System.Collections.Generic.KeyValuePair<string, IEnumerable<string>> kvp in httpRequestMessage.Headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value.ToArray().ToDelimited(val => val, ","));
            }
            return request;
        }

        static Dictionary<HttpMethod, HttpVerbs> _verbsByMethod;
        protected static Dictionary<HttpMethod, HttpVerbs> VerbsByMethod
        {
            get
            {
                if (_verbsByMethod == null)
                {
                    _verbsByMethod = new Dictionary<HttpMethod, HttpVerbs>
                    {
                        { HttpMethod.Get, HttpVerbs.Get },
                        { HttpMethod.Post, HttpVerbs.Post },
                        { HttpMethod.Put, HttpVerbs.Put },
                        { HttpMethod.Delete, HttpVerbs.Delete },
                        { HttpMethod.Head, HttpVerbs.Head },
                        { HttpMethod.Options, HttpVerbs.Options },
                        { HttpMethod.Trace, HttpVerbs.Trace }
                    };
                }
                return _verbsByMethod;
            }
        }

        Dictionary<HttpVerbs, HttpMethod> _methodsByVerbs;
        private Dictionary<HttpVerbs, HttpMethod> MethodsByVerbs
        {
            get
            {
                if(_methodsByVerbs == null)
                {
                    _methodsByVerbs = new Dictionary<HttpVerbs, HttpMethod> 
                    {
                        { HttpVerbs.Get, HttpMethod.Get },
                        { HttpVerbs.Post, HttpMethod.Post },
                        { HttpVerbs.Put, HttpMethod.Put },
                        { HttpVerbs.Delete, HttpMethod.Delete },
                        { HttpVerbs.Head, HttpMethod.Head },
                        { HttpVerbs.Options, HttpMethod.Options },
                        { HttpVerbs.Trace, HttpMethod.Trace }
                    };
                }
                return _methodsByVerbs;
            }
        }
    }
}
