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
                    .Create();
                
                return payment;
            });
        }
    }
}
