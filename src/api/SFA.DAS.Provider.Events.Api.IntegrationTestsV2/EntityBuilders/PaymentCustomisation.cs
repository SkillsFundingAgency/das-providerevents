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
                    .Without(x => x.RequiredPaymentId)
                    .Create();
                
                return payment;
            });
        }
    }
}
