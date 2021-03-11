using System;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    public static class AcademicYearHelper
    {
        private static readonly short[] Years = { 1617, 1718, 1819, 1920, 2021 };

        public static short GetRandomAcademicYear()
        {
            var randomYear = new Random().Next(0, 4);
            return Years[randomYear];
        }

        public static byte GetRandomCollectionPeriod()
        {
            return (byte) new Random().Next(1, 14);
        }
    }
}
