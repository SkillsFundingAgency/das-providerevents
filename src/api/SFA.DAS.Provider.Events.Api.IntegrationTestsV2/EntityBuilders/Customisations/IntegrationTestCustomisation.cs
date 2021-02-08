using Ploeh.AutoFixture;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.EntityBuilders.Customisations
{
    class IntegrationTestCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(
                new CompositeCustomization(
                    new PaymentCustomisation(),
                    new EarningCustomisation(),
                    new DataLockEventCustomisation(),
                    new RequiredPaymentCustomisation(),
                    new TransferCustomisation(),
                    new RandomRangedNumberCustomization()
                ));
        }
    }
}
