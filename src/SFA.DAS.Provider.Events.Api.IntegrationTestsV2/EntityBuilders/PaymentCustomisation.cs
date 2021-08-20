using System;
using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.EntityBuilders
{
    class PaymentCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var payment = fixture
                    .Build<ItPayment>()
                    .With(x => x.RequiredPaymentEventId, new Random().Next(0, 100) > 90 ? (Guid?)null : Guid.NewGuid())
                    .With(x => x.AccountId, TestData.EmployerAccountId)
                    .With(x => x.CollectionPeriod, TestData.CollectionPeriod)
                    .With(x => x.AcademicYear, TestData.AcademicYear)
                    .With(x => x.TransferSenderAccountId, new Random().Next(0, 100) > 90 ? (long?)null : new Random().Next())
                    .Create();

                //if the sender id is set, FundingSource must be set to 5. Else, if the FundingSource is 5 on other payments, set them back to 1, because we don't want non transfer payments with FundingSource 5 as that is invalid data
                if (payment.TransferSenderAccountId.HasValue)
                {
                    payment.FundingSource = 5;
                }
                else if(payment.FundingSource == 5)
                {
                    payment.FundingSource = 1;
                }
                    
                
                return payment;
            });
        }
    }
}
