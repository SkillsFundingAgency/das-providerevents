namespace SFA.DAS.Provider.Events.Application.Extensions
{
    public static class PeriodExtensions
    {
        public static int GetCollectionPeriod(this Data.Period period)
        {
            return int.Parse(period.Id.Substring(6));
        }

        public static int GetAcademicYear(this Data.Period period)
        {
            return int.Parse(period.Id.Substring(0, 4));
        }
    }
}
