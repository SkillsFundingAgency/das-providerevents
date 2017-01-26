using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext
{
    public class GlobalTestContext
    {
        private GlobalTestContext()
        {
            SetupAssemblyDirectory();
            SetupConnectionStrings();
            ExtractDatabaseNames();
        }

        public string AssemblyDirectory { get; private set; }

        public string TransientDatabaseConnectionString { get; private set; }
        public string TransientDatabaseName { get; private set; }
        public string TransientDatabaseNameBracketed { get; private set; }

        public string DedsDatabaseConnectionString { get; private set; }
        public string DedsDatabaseName { get; private set; }
        public string DedsDatabaseNameBracketed { get; private set; }



        private void SetupAssemblyDirectory()
        {
            AssemblyDirectory = System.IO.Path.GetDirectoryName(typeof(GlobalTestContext).Assembly.Location);
        }

        private void SetupConnectionStrings()
        {
            TransientDatabaseConnectionString = Environment.GetEnvironmentVariable("TransientDatabaseConnectionString");
            if (string.IsNullOrEmpty(TransientDatabaseConnectionString))
            {
                TransientDatabaseConnectionString = ConfigurationManager.AppSettings["TransientDatabaseConnectionString"];
            }

            DedsDatabaseConnectionString = Environment.GetEnvironmentVariable("DedsDatabaseConnectionString");
            if (string.IsNullOrEmpty(DedsDatabaseConnectionString))
            {
                DedsDatabaseConnectionString = ConfigurationManager.AppSettings["DedsDatabaseConnectionString"];
            }
        }

        private void ExtractDatabaseNames()
        {
            TransientDatabaseName = ExtractDatabaseNameFromConnectionString(TransientDatabaseConnectionString);
            TransientDatabaseNameBracketed = $"[{TransientDatabaseName}]";

            DedsDatabaseName = ExtractDatabaseNameFromConnectionString(DedsDatabaseConnectionString);
            DedsDatabaseNameBracketed = $"[{DedsDatabaseName}]";
        }
        private string ExtractDatabaseNameFromConnectionString(string connectionString)
        {
            var match = Regex.Match(connectionString, @"database=([A-Z0-9\-_]{1,});", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            match = Regex.Match(connectionString, @"initial catalog=([A-Z0-9\-_]{1,});", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new Exception("Cannot extract database name from connection string");
        }



        private static GlobalTestContext _current;
        public static GlobalTestContext Current => _current ?? (_current = new GlobalTestContext());
    }
}
