using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.EntityBuilders.Customisations;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess
{
    class DatabaseSetup
    {
        private readonly PopulateTables _populate;

        public DatabaseSetup(PopulateTables populate)
        {
            _populate = populate;
        }

        public async Task PopulateAllData()
        {
            Debug.WriteLine("Populating tables, this could take a while");

            var data = _populate;

            var payments = new IEnumerable<ItPayment>[8];
            var fixture = new Fixture().Customize(new IntegrationTestCustomisation());
            Parallel.For(0, 8, i => payments[i] = fixture.CreateMany<ItPayment>(2500));

            TestData.Payments = payments.SelectMany(p => p).ToList();
            
            await data.BulkInsertPayments(TestData.Payments).ConfigureAwait(false);
        }
    }
}
