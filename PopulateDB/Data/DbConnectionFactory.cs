using Microsoft.Data.SqlClient;
using PopulateDB.Interfaces;
using System.Data;

namespace PopulateDB.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionsString = string.Empty;

        public DbConnectionFactory(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            if (connString is not null)
                _connectionsString = connString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionsString);
        }
    }
}
