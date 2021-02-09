namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    public static class HelperExtensions
    {
        public static int ToCalendarMonth(this byte collectionPeriod)
        {
            if (collectionPeriod < 6)
                return collectionPeriod + 7;
            return collectionPeriod - 5;
        }
    }
}
