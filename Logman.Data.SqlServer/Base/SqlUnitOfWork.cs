using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Logman.Common.Code;
using Logman.Common.Data;

namespace Logman.Data.SqlServer.Base
{
    public sealed class SqlUnitOfWork : IUnitOfWork
    {
        private SqlConnection _connection;

        public SqlUnitOfWork()
        {
            CreateConnection();
        }

        private void CreateConnection()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings[Constants.DefaultConnectionStringName].ToString();

            _connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public DbConnection Connection
        {
            get
            {
                if (_connection.ConnectionString == string.Empty)
                {
                    CreateConnection();
                }
                return _connection;
            }
        }
    }
}