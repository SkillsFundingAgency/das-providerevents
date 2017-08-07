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
            SetupSubmissionTransientDatabase();
            SetupPeriodEndTransientDatabase();
        }


        private void SetupDedsDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.DedsDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("datalockevents.deds.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("MigrationScripts\\001_DataLockEvents_Add_EventStatus.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("MigrationScripts\\002_DataLockEvents_Change_Commitment_Verion.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("MigrationScripts\\003_DataLockEvents_Change_LearnRef_AimSequence_ColumnType.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }   
        }

        private void SetupSubmissionTransientDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientSubmissionDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("ilr.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("Ilr.Transient.Reference.CollectionPeriods.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("commitments.transient.reference.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.views.submission.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                connection.RunDbSetupSqlScriptFile("datalockevents.transient.reference.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.views.submission.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.procedures.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }
        }

        private void SetupPeriodEndTransientDatabase()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientPeriodEndDatabaseConnectionString))
            {
                connection.RunDbSetupSqlScriptFile("ilr.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("Ilr.Transient.Reference.CollectionPeriods.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("commitments.transient.reference.ddl.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.reference.transient.ddl.tables.periodend.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalock.transient.ddl.views.periodend.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                connection.RunDbSetupSqlScriptFile("datalockevents.transient.reference.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.tables.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.views.periodend.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                connection.RunDbSetupSqlScriptFile("datalockevents.transient.ddl.procedures.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
            }
        }
    }
}
