using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.TestContext
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

        public string TransientSubmissionDatabaseConnectionString { get; private set; }
        public string TransientSubmissionDatabaseName { get; private set; }
        public string TransientSubmissionDatabaseNameBracketed { get; private set; }

        public string TransientPeriodEndDatabaseConnectionString { get; private set; }
        public string TransientPeriodEndDatabaseName { get; private set; }
        public string TransientPeriodEndDatabaseNameBracketed { get; private set; }

        public string DedsDatabaseConnectionString { get; private set; }
        public string DedsDatabaseName { get; private set; }
        public string DedsDatabaseNameBracketed { get; private set; }



        private void SetupAssemblyDirectory()
        {
            AssemblyDirectory = System.IO.Path.GetDirectoryName(typeof(GlobalTestContext).Assembly.Location);
        }

        private void SetupConnectionStrings()
        {
            TransientSubmissionDatabaseConnectionString = Environment.GetEnvironmentVariable("TransientSubmissionDatabaseConnectionString");
            if (string.IsNullOrEmpty(TransientSubmissionDatabaseConnectionString))
            {
                TransientSubmissionDatabaseConnectionString = ConfigurationManager.AppSettings["TransientSubmissionDatabaseConnectionString"];
            }

            TransientPeriodEndDatabaseConnectionString = Environment.GetEnvironmentVariable("TransientPeriodEndDatabaseConnectionString");
            if (string.IsNullOrEmpty(TransientPeriodEndDatabaseConnectionString))
            {
                TransientPeriodEndDatabaseConnectionString = ConfigurationManager.AppSettings["TransientPeriodEndDatabaseConnectionString"];
            }

            DedsDatabaseConnectionString = Environment.GetEnvironmentVariable("DedsDatabaseConnectionString");
            if (string.IsNullOrEmpty(DedsDatabaseConnectionString))
            {
                DedsDatabaseConnectionString = ConfigurationManager.AppSettings["DedsDatabaseConnectionString"];
            }
        }

        private void ExtractDatabaseNames()
        {
            TransientSubmissionDatabaseName = ExtractDatabaseNameFromConnectionString(TransientSubmissionDatabaseConnectionString);
            TransientSubmissionDatabaseNameBracketed = $"[{TransientSubmissionDatabaseName}]";

            TransientPeriodEndDatabaseName = ExtractDatabaseNameFromConnectionString(TransientPeriodEndDatabaseConnectionString);
            TransientPeriodEndDatabaseNameBracketed = $"[{TransientPeriodEndDatabaseName}]";

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
