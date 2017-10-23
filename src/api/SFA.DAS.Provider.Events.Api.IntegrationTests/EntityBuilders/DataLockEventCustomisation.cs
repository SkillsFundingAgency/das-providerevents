using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.EntityBuilders
{
    class DataLockEventCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var builder = fixture.Build<DataLockEventEntity>()
                    
                    .Create();
                return builder;
            });
        }
    }
}
