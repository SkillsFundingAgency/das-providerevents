using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.Data
{
    public abstract class SqlServerRepository
    {
        private readonly string _connectionString;

        protected SqlServerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }


        protected T[] Query<T>(string command, object param = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Query<T>(command, param).ToArray();
            }
        }
        protected T QuerySingle<T>(string command, object param = null)
        {
            return Query<T>(command, param).SingleOrDefault();
        }

        protected void Execute(string command, object param = null)
        {
            using (var connection = GetOpenConnection())
            {
                connection.Execute(command, param);
            }
        }
    }
}
