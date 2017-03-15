using System.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests
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


        internal static void SetupDedsDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.DedsDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("submissions.deds.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("ilr.deds.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.deds.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }   
        }

        internal static void SetupTransientDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("submissions.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("submissions.transient.ddl.functions.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("submissions.transient.ddl.views.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }
        }

    }
}
