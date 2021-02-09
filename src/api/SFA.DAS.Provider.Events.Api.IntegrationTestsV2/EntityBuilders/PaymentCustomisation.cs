using System;
using Ploeh.AutoFixture;
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
                    .Create();
                
                return payment;
            });
        }
    }
}
