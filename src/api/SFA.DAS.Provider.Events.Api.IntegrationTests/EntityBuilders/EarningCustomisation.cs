using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.EntityBuilders
{
    class EarningCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var earning = fixture
                    .Build<ItEarning>()
                    .Without(x => x.RequiredPaymentId)
                    .Create();
                return earning;
            });
        }
    }
}
