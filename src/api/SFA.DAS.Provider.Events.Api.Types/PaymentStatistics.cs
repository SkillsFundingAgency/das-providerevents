using System;
using System.Collections.Generic;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class PaymentStatistics
    {
        public int TotalNumberOfPayments { get; set; }
        public int TotalNumberOfRecievedPayments { get; set; }
    }
}
