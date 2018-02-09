using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Microsoft.SqlServer.Dac;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    internal static class TestDataHelperDataLockEventStorage
    {
        static readonly string _connectionString = ConfigurationManager.AppSettings["DataLockEventConnectionString"];
        private const string DatabaseName = "ProviderEventsAT_Local";

        public static void CreateDatabase()
        {
            //if (_connectionString.Contains(DatabaseName))
            //{
            //    using (var connection = new SqlConnection(_connectionString.Replace(DatabaseName, "master")))
            //        connection.Execute($"if(db_id(N'{DatabaseName}') IS NOT NULL) drop database [{DatabaseName}]");
            //}

            var instance = new DacServices(_connectionString);
            var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(TestDataHelperDeds).Assembly.Location),
                @"..\..\..\SFA.DAS.Provider.Events.DataLockEventsDatabase\bin\Debug\SFA.DAS.Provider.Events.DataLockEventsDatabase.dacpac"));

            using (var dacpac = DacPackage.Load(path))
            {
                instance.Deploy(dacpac, DatabaseName, true);
            }
        }

        public static int Count(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                return connection.ExecuteScalar<int>(sql, parameters);
        }

        public static IList<DataLockEventEntity> GetAllEvents()
        {
            return null;
        }

        public static void Clean()
        {
        }

        public static void AddProvider(long ukprn, DateTime ilrSubmissionDate)
        {
        }


            //private static int DaysInMonth(DateTime value)
            //{
            //    return DateTime.DaysInMonth(value.Year, value.Month);
            //}

            //private static DateTime LastDayOfMonth(DateTime value)
            //{
            //    return new DateTime(value.Year, value.Month, DaysInMonth(value));
            //}


        private static void Execute(string command, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(command, param);
        }
    }
}
