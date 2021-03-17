namespace SFA.DAS.Provider.Events.Application.UnitTests
{
    public static class PeriodExtensions
    {
        public static int GetYearFromPaymentEntity(short academicYear, byte period)
        {
            if (period < 6)
            {
                return int.Parse("20" + academicYear.ToString().Substring(0, 2));
            }
            else
            {
                return int.Parse("20" + academicYear.ToString().Substring(2, 2));
            }
        }

        public static int GetMonthFromPaymentEntity(byte period)
        {
            if (period == 13)
                return 9;
            if (period == 14)
                return 10;

            if (period < 6)
                return period + 7;
            return period - 5;
        }
    }
}
