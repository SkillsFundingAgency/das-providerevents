using System.Collections.Generic;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    class TestData
    {
        /// <summary>
        /// List of test payments inserted into the [Payments2].[Payment] table during this test run.
        /// </summary>
        public static List<ItPayment> Payments { get; set; }
        /// <summary>
        /// List of all payments in the [Payments2].[Payment] table.
        /// </summary>
        public static List<ItPayment> AllPayments { get; set; }
    }
}
