using System;
using System.Collections.Generic;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class TestDataSet
    {
        public List<ProviderEntity> Providers { get; set; }
        public List<LearningDeliveryEntity> LearningDeliveries { get; set; }
        public List<PriceEpisodeEntity> PriceEpisodes { get; set; }

        public void Store()
        {
            foreach (var provider in Providers)
            {
                ProviderRepository.Create(provider);
            }

            foreach (var learningDelivery in LearningDeliveries)
            {
                LearningDeliveryRepository.Create(learningDelivery);
            }

            foreach (var priceEpisode in PriceEpisodes)
            {
                PriceEpisodeRepository.Create(priceEpisode);
            }
        }

        public static TestDataSet GetFirstSubmissionDataset()
        {
            var rdm = new Random();

            var ukprn = rdm.Next(10000000, 99999999);
            var prepDateTime = DateTime.Now.AddMinutes(-5);
            var academicYear = GetAcademicYear(prepDateTime);
            var startDate = DateTime.Today.AddMonths(3);
            return new TestDataSet
            {
                Providers = new List<ProviderEntity>
                {
                    new ProviderEntity
                    {
                        Ukprn = ukprn,
                        FileName = $"ILR-{ukprn}-{academicYear}-{prepDateTime.ToString("yyyyMMdd-HHmmss")}-01",
                        SubmittedTime = DateTime.Now.AddMinutes(-1)
                    }
                },
                LearningDeliveries = new List<LearningDeliveryEntity>
                {
                    new LearningDeliveryEntity
                    {
                        Ukprn = ukprn,
                        LearnRefNumber = "1",
                        AimSeqNumber = 1,
                        Uln = rdm.Next(1000000000, int.MaxValue),
                        NiNumber = "AB123456A",
                        StdCode = 34,
                        LearnStartDate = startDate,
                        LearnPlanEndDate = DateTime.Today.AddMonths(16)
                    }
                },
                PriceEpisodes = new List<PriceEpisodeEntity>
                {
                    new PriceEpisodeEntity
                    {
                        PriceEpisodeIdentifier = $"00-34-01/{startDate.ToString("MM/yyyy")}",
                        Ukprn = ukprn,
                        LearnRefNumber = "1",
                        PriceEpisodeAimSeqNumber = 1,
                        EpisodeEffectiveTnpStartDate = startDate,
                        Tnp1 = 12000,
                        Tnp2 = 3000
                    }
                }
            };
        }
        private static string GetAcademicYear(DateTime date)
        {
            var startYear = date.Year - 2000;

            if (date.Month < 8)
            {
                startYear -= 1;
            }

            return $"{startYear}{startYear + 1}";
        }
    }
}
