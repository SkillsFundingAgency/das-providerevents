using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    public static class HelperExtensions
    {
        public static int ToCalendarMonth(this byte collectionPeriod)
        {
            if (collectionPeriod == 13)
                return 9;
            if (collectionPeriod == 14)
                return 10;

            if (collectionPeriod < 6)
                return collectionPeriod + 7;
            return collectionPeriod - 5;
        }

        public static int GetCalendarYear(this ItPayment payment)
        {
            if (payment.CollectionPeriod < 6)
                return int.Parse("20" + payment.AcademicYear.ToString().Substring(0, 2));
            return int.Parse("20" + payment.AcademicYear.ToString().Substring(2, 2));
        }

        public static string GetPeriodId(this ItPayment payment)
        {
            return $"{payment.AcademicYear}-R{payment.CollectionPeriod.ToString().PadLeft(2, '0')}";
        }
    }
}
