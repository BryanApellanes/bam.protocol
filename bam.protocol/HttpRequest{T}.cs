namespace Bam.Encryption
{
    /// <summary>
    /// Represents a strongly-typed HTTP request that automatically serializes and deserializes the content body as JSON.
    /// </summary>
    /// <typeparam name="TContent">The type of the request body content.</typeparam>
    public class HttpRequest<TContent> : HttpRequest, IHttpRequest<TContent>
    {
        TContent content;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest{TContent}"/> class with default settings.
        /// </summary>
        public HttpRequest():base()
        {
        }

        /// <summary>
        /// Gets or sets the strongly-typed content. Getting deserializes from JSON if needed; setting serializes to JSON.
        /// </summary>
        public TContent TypedContent
        {
            get
            {
                if (this.content == null && !string.IsNullOrEmpty(base.Content))
                {
                    base.Content.TryFromJson<TContent>(out this.content);
                }
                return this.content;
            }
            set
            {
                this.content = value;
                base.Content = this.TypedContent.ToJson();
            }
        }

        /// <summary>
        /// Copies all properties from the specified typed request to this instance.
        /// </summary>
        /// <param name="request">The typed request to copy from.</param>
        public void Copy(IHttpRequest<TContent> request)
        {
            this.Uri = request.Uri;
            this.TypedContent = request.TypedContent;
            this.ContentType = request.ContentType;
            this.Verb =  request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
