namespace Bam.Server
{
    /// <summary>
    /// Represents a mapping from any possible
    /// Uri.Host value to an application.
    /// </summary>
    public class HostAppMap
    {
        /// <summary>
        /// Gets or sets the host.  This equates to the Host
        /// property of a Uri.
        /// </summary>
        public string Host { get; set; } = null!;

        /// <summary>
        /// Gets or sets the AppName that the Host should be mapped to.
        /// </summary>
        public string AppName { get; set; } = null!;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.GetHashCode(Host, AppName);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is HostAppMap hostMapping)
            {
                return hostMapping.Host.Equals(Host) &&
                    hostMapping.AppName.Equals(AppName);
            }
            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Host={Host}, AppName={AppName}";
        }

        /// <summary>
        /// Loads an array of <see cref="HostAppMap"/> instances from a JSON file, removing duplicates.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>An array of unique <see cref="HostAppMap"/> instances.</returns>
        public static HostAppMap[] Load(string filePath)
        {
            return new HashSet<HostAppMap>(filePath.FromJsonFile<HostAppMap[]>()).ToArray();
        }
    }
}