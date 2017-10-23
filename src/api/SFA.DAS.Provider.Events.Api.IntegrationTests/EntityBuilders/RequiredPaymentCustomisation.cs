using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.EntityBuilders
{
    class RequiredPaymentCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var requiredPayment = fixture
                    .Build<ItRequiredPayment>()
                    .Create();
                foreach (var payment in requiredPayment.Payments)
                {
                    payment.RequiredPaymentId = requiredPayment.Id;
                    foreach (var earning in payment.Earnings)
                    {
                        earning.RequiredPaymentId = payment.RequiredPaymentId;
                    }
                }
                return requiredPayment;
            });
        }
    }
}
