using System;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests
{
    public static class SqlExtensions
    {
        public static void RunDbSetupSqlScriptFile(this SqlConnection connection, string fileName, string databaseName)
        {
            var path = Path.Combine(GlobalTestContext.Current.AssemblyDirectory, "DbSetupScripts", fileName);
            RunSqlScriptFile(connection, path, databaseName);
        }
        public static void RunSqlScriptFile(this SqlConnection connection, string path, string databaseName)
        {
            var sqlScript = File.ReadAllText(path);
            RunSqlScript(connection, sqlScript, databaseName);
        }
        public static void RunSqlScript(this SqlConnection connection, string sqlScript, string databaseName)
        {
            var detokenizedSqlScript = ReplaceSqlTokens(sqlScript, databaseName);
            var commands = detokenizedSqlScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var command in commands)
            {
                connection.Execute(command);
            }
        }

        private static string ReplaceSqlTokens(string sql, string databaseName)
        {
            return sql.Replace("${ILR_Deds.FQ}", databaseName)
                      .Replace("${ILR_Summarisation.FQ}", databaseName)
                      .Replace("${DAS_Commitments.FQ}", databaseName)
                      .Replace("${DAS_PeriodEnd.FQ}", databaseName)
                      .Replace("${YearOfCollection}", "1617");
        }
    }
}
