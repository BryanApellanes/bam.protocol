/*
    Copyright © Bryan Apellanes 2015
*/

namespace Bam.Server
{
    /// <summary>
    /// Specifies the serialization format for configuration files.
    /// </summary>
    public enum ConfFormat
    {
        /// <summary>
        /// JSON format.
        /// </summary>
        Json,
        /// <summary>
        /// YAML format.
        /// </summary>
        Yaml,
        /// <summary>
        /// XML format.
        /// </summary>
        Xml
    }
}