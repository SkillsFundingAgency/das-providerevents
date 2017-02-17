using System.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests
{
    [SetUpFixture]
    public class GlobalSetup
    {

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            SetupDedsDatabase();
            SetupTransientDatabase();
        }


        private void SetupDedsDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.DedsDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("DasCommitments.deds.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("DataLock.deds.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("Ilr.deds.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("Ilr.Earnings.deds.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("Payments.deds.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                connection.RunDbSetupSqlScriptFile("datalock.deds.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }   
        }

        private void SetupTransientDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("datalock.transient.reference.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.views.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }
        }
    }
}
