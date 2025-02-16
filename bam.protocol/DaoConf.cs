/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Data;
using System.Configuration;

namespace Bam.Server
{
    public class DaoConf
    {
        /// <summary>
        /// Get a DaoConf with default settings (uses SQLite)
        /// </summary>
        /// <param name="daoType"></param>
        /// <param name="bryanConf"></param>
        /// <returns></returns>
        public static DaoConf GetDefault(Type daoType, string contentRoot)
        {
            return GetDefault(Dao.ConnectionName(daoType), contentRoot);
        }

        /// <summary>
        /// Get a DaoConf with default settings (uses SQLite)
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="bryanConf"></param>
        /// <returns></returns>
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
        /// The name of the connection (this equates to ContextName in some instances (example, qi.js) and 
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

        public override string ToString()
        {
            return $"DaoConf::{ConnectionName}::{RegistrarCaller}";
        }
    }
}
