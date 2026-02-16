using Bam.ServiceProxy;

namespace Bam.Server
{
    /// <summary>
    /// Provides extension methods for HTTP request handling, including request ID management and client IP resolution.
    /// </summary>
    public static class RequestExtensions
    {
        private static string RequestIdHeader = "x-request-id";

        /// <summary>
        /// If the `x-request-id` header is not present then it is added.
        /// </summary>
        /// <param name="request"></param>
        public static void SetRequestId(this IRequest request)
        {
            if (!request.HasHeader(RequestIdHeader))
            {
                request.Headers.Add(RequestIdHeader, "bam-".RandomString(16));
            }
        }

        /// <summary>
        /// Sets the request ID on the request within the specified HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context whose request should receive a request ID.</param>
        public static void SetRequestId(this IHttpContext context)
        {
            SetRequestId(context.Request);
        }

        /// <summary>
        /// Gets the value of the <c>x-request-id</c> header from the request.
        /// </summary>
        /// <param name="request">The request to get the ID from.</param>
        /// <returns>The request ID, or an empty string if the header is not present.</returns>
        public static string GetRequestId(this IRequest request)
        {
            if (request?.Headers == null)
            {
                return string.Empty;
            }

            return request.HasHeader(RequestIdHeader, out string requestId) ? requestId : string.Empty;
        }

        /// <summary>
        /// Determines whether the request contains the specified header.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <param name="headerName">The name of the header to look for.</param>
        /// <returns>True if the header is present; otherwise, false.</returns>
        public static bool HasHeader(this IRequest request, string headerName)
        {
            return HasHeader(request, headerName, out string ignore);
        }

        /// <summary>
        /// Determines whether the request contains the specified header and retrieves its value.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <param name="headerName">The name of the header to look for.</param>
        /// <param name="headerValue">When this method returns, contains the header value if found; otherwise, an empty string.</param>
        /// <returns>True if the header is present and non-empty; otherwise, false.</returns>
        public static bool HasHeader(this IRequest request, string headerName, out string headerValue)
        {
            headerValue = string.Empty;

            if (request?.Headers != null && !string.IsNullOrEmpty(request?.Headers[headerName]))
            {
                headerValue = request.Headers[headerName];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the request contains an <c>x-request-id</c> header.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <returns>True if the request ID header is present; otherwise, false.</returns>
        public static bool HasRequestIdHeader(this IRequest request)
        {
            return HasRequestIdHeader(request, out _);
        }

        /// <summary>
        /// Determines whether the request contains an <c>x-request-id</c> header and retrieves its value.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <param name="requestId">When this method returns, contains the request ID if found; otherwise, an empty string.</param>
        /// <returns>True if the request ID header is present; otherwise, false.</returns>
        public static bool HasRequestIdHeader(this IRequest request, out string requestId)
        {
            return HasHeader(request, RequestIdHeader, out requestId);
        }
        
        /// <summary>
        /// Gets the client IP address from the HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>The client IP address.</returns>
        public static string GetClientIp(this IHttpContext context)
        {
            return GetClientIp(context?.Request);
        }

        /// <summary>
        /// Gets the client IP address from the request, checking <c>X-Forwarded-For</c>, <c>Remote-Addr</c>, and <c>UserHostAddress</c> in order.
        /// </summary>
        /// <param name="request">The request to get the client IP from.</param>
        /// <returns>The client IP address.</returns>
        public static string GetClientIp(this IRequest request)
        {
            return request?.Headers["X-Forwarded-For"]
                .Or(request?.Headers["Remote-Addr"])
                .Or(request?.UserHostAddress);
        }
    }
}