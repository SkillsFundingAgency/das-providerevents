using System;
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
                // these fields are datetime's in the db, so can't match the precision of c#'s datetime, so we chop off the milliseconds
                // ideally, they should be datetime2's in the db
                earning.ActualEndDate = earning.ActualEndDate.AddTicks(-(earning.ActualEndDate.Ticks % TimeSpan.TicksPerSecond));
                earning.PlannedEndDate = earning.PlannedEndDate.AddTicks(-(earning.PlannedEndDate.Ticks % TimeSpan.TicksPerSecond));
                earning.StartDate = earning.StartDate.AddTicks(-(earning.StartDate.Ticks % TimeSpan.TicksPerSecond));
                return earning;
            });
        }
    }
}
