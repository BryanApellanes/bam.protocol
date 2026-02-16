/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Data;
using System.Configuration;

namespace Bam.Server
{
    /// <summary>
    /// Represents DAO configuration, including the connection name and registrar caller for database registration.
    /// </summary>
    public class DaoConf
    {
        /// <summary>
        /// Gets a DaoConf with default settings (uses SQLite), deriving the connection name from the specified DAO type.
        /// </summary>
        /// <param name="daoType">The DAO type to derive the connection name from.</param>
        /// <param name="contentRoot">The content root directory for resolving the SQLite database path.</param>
        /// <returns>A new <see cref="DaoConf"/> configured for SQLite.</returns>
        public static DaoConf GetDefault(Type daoType, string contentRoot)
        {
            return GetDefault(Dao.ConnectionName(daoType), contentRoot);
        }

        /// <summary>
        /// Gets a DaoConf with default settings (uses SQLite) for the specified connection name.
        /// </summary>
        /// <param name="connectionName">The database connection name.</param>
        /// <param name="contentRoot">The content root directory for resolving the SQLite database path.</param>
        /// <returns>A new <see cref="DaoConf"/> configured for SQLite.</returns>
        public static DaoConf GetDefault(string connectionName, string contentRoot)
        {
            SQLiteConnectionStringResolver connResolver = new SQLiteConnectionStringResolver()
            {
                Directory = new DirectoryInfo(Path.Combine(contentRoot, "common", "workspace"))
            };
            ConnectionStringSettings settings = connResolver.Resolve(connectionName);
            return new DaoConf { ConnectionName = connectionName, RegistrarCaller = typeof(SQLiteRegistrarCaller).AssemblyQualifiedName };
        }
        
        /// <summary>
        /// The name of the connection (this equals ContextName in some instances (example, qi.js) and 
        /// is also the name of the connection string setting used in the default configuration file)
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// The AssemblyQualifiedName of an IRegistrarCaller implementation
        /// used to register the underlying database type (SQLite, SqlClient, etc.)
        /// </summary>
        public string RegistrarCaller { get; set; }

        IRegistrarCaller _registrarCaller;
        readonly object _registrarCallerLock = new object();
        protected IRegistrarCaller RegistrarCallerInstance
        {
            get
            {
                return _registrarCallerLock.DoubleCheckLock<IRegistrarCaller>(ref _registrarCaller, () => Type.GetType(RegistrarCaller).Construct<IRegistrarCaller>());
            }
        }

        /// <summary>
        /// Calls the appropriate RegistrarCaller for the ConnectionName of this DaoConf
        /// </summary>
        public void Register()
        {
            RegistrarCallerInstance.Register(ConnectionName);
        }

        /// <summary>
        /// Returns a string representation of this DAO configuration.
        /// </summary>
        /// <returns>A string containing the connection name and registrar caller.</returns>
        public override string ToString()
        {
            return $"DaoConf::{ConnectionName}::{RegistrarCaller}";
        }
    }
}
