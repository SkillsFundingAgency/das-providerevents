using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public abstract class DcfsRepository
    {
        private readonly string _connectionStringName;

        protected DcfsRepository() 
            : this("PaymentsV2ConnectionString")
        {
        }

        protected DcfsRepository(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        protected async Task<SqlConnection> GetOpenConnection()
        {
            var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName));
            await connection.OpenAsync().ConfigureAwait(false);
            return connection;
        }

        protected async Task<T[]> Query<T>(string command, object param = null)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                return (await connection.QueryAsync<T>(command, param).ConfigureAwait(false)).ToArray();
            }
        }

        protected async Task<T> QuerySingle<T>(string command, object param = null)
        {
            return (await Query<T>(command, param).ConfigureAwait(false)).SingleOrDefault();
        }

        protected int NumberOfPages(int totalRows, int pageSize)
        {
            return (int)Math.Ceiling(totalRows / (float)pageSize);
        }

        protected PageOfResults<T> PageResults<T>(List<T> entities, int pageNumber, int pageSize, int count)
        {
            var returnValue = new PageOfResults<T>
            {
                PageNumber = pageNumber,
                TotalNumberOfPages = 0,
                Items = entities.ToArray(),
            };

            if (entities.Any())
            {
                returnValue.TotalNumberOfPages = NumberOfPages(count, pageSize);
            }

            return returnValue;
        }
    }
}
