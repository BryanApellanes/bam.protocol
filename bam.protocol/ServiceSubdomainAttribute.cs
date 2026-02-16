namespace Bam.ServiceProxy
{
    /// <summary>
    /// Used to specify the subdomain 
    /// a class should be served from when resolving
    /// hostname for a service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceSubdomainAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSubdomainAttribute"/> class with the specified subdomain.
        /// </summary>
        /// <param name="subdomain">The subdomain to serve the class from.</param>
        public ServiceSubdomainAttribute(string subdomain)
        {
            Subdomain = subdomain;
        }
        /// <summary>
        /// Gets the subdomain value.
        /// </summary>
        public string Subdomain { get; }
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is ServiceSubdomainAttribute a)
            {
                return a.Subdomain.EndsWith(Subdomain);
            }
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Subdomain.ToSha1Int();
        }
    }
}