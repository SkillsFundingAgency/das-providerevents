using System;
using System.Collections.Generic;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

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

        public static List<ItPeriod> GetAllValidTestPeriods()
        {
            var periods = new List<ItPeriod>();
            foreach (var year in Years)
            {
                for (byte i = 1; i <= 14; i++)
                {
                    periods.Add(new ItPeriod{ Period = i, AcademicYear = year, CompletionDate = DateTime.Now, ReferenceDataValidationDate = DateTime.Now });
                }
            }
            return periods;
        }
    }
}
