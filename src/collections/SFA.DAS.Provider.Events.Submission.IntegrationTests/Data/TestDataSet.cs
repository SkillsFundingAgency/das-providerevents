using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class TestDataSet
    {
        public TestDataSet()
        {
            Providers = new List<ProviderEntity>();
            LearningDeliveries = new List<LearningDeliveryEntity>();
            PriceEpisodes = new List<PriceEpisodeEntity>();
            PreviousVersions = new List<LastSeenVersionEntity>();
        }

        public string AcademicYear { get; set; }
        
        public List<ProviderEntity> Providers { get; set; }
        public List<LearningDeliveryEntity> LearningDeliveries { get; set; }
        public List<PriceEpisodeEntity> PriceEpisodes { get; set; }
        public List<LastSeenVersionEntity> PreviousVersions { get; set; }

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

            foreach (var version in PreviousVersions)
            {
                LastSeenVersionRepository.Create(version);
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
                AcademicYear = academicYear,
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
                        Tnp2 = 3000,
                        CommitmentId = 567577,
                        EmpId=1
                    }
                }
            };
        }
        public static TestDataSet GetInsignificantlyUpdatedSubmissionDataSet()
        {
            var dataset = GetFirstSubmissionDataset();

            dataset.PreviousVersions = GetPreviousVersionsFromSubmissions(dataset);

            return dataset;
        }
        public static TestDataSet GetUpdatedSubmissionDataSet()
        {
            var dataset = GetInsignificantlyUpdatedSubmissionDataSet();

            foreach (var version in dataset.PreviousVersions)
            {
                version.StandardCode += 11;
                version.OnProgrammeTotalPrice -= 123;
            }

            return dataset;
        }


        private static List<LastSeenVersionEntity> GetPreviousVersionsFromSubmissions(TestDataSet dataset)
        {
            var previousVersions = new List<LastSeenVersionEntity>();

            foreach (var learningDelivery in dataset.LearningDeliveries)
            {
                var provider = dataset.Providers.Single(p => p.Ukprn == learningDelivery.Ukprn);
                previousVersions.AddRange(dataset.PriceEpisodes
                    .Where(pe => pe.Ukprn == learningDelivery.Ukprn)
                    .Select(pe => new LastSeenVersionEntity
                    {
                        IlrFileName = provider.FileName,
                        FileDateTime = ExtractPrepDateFromFileName(provider.FileName),
                        SubmittedDateTime = provider.SubmittedTime,
                        ComponentVersionNumber = SubmissionEventsTask.ComponentVersion,
                        Ukprn = provider.Ukprn,
                        Uln = learningDelivery.Uln,
                        LearnRefNumber = learningDelivery.LearnRefNumber,
                        AimSeqNumber = learningDelivery.AimSeqNumber,
                        PriceEpisodeIdentifier = pe.PriceEpisodeIdentifier,
                        StandardCode = learningDelivery.StdCode,
                        ProgrammeType = learningDelivery.ProgType,
                        FrameworkCode = learningDelivery.FworkCode,
                        PathwayCode = learningDelivery.PwayCode,
                        ActualStartDate = learningDelivery.LearnStartDate,
                        PlannedEndDate = learningDelivery.LearnPlanEndDate,
                        ActualEndDate = learningDelivery.LearnActEndDate,
                        OnProgrammeTotalPrice = pe.Tnp1,
                        CompletionTotalPrice = pe.Tnp2,
                        NiNumber = learningDelivery.NiNumber,
                        CommitmentId = pe.CommitmentId,
                        EmployerReferenceNumber = pe.EmpId,
                        AcademicYear = dataset.AcademicYear
                    }));
            }

            return previousVersions;
        }
        private static DateTime ExtractPrepDateFromFileName(string fileName)
        {
            var match = Regex.Match(fileName, @"^ILR\-[0-9]{8,12}\-[0-9]{4}\-([0-9]{4})([0-9]{2})([0-9]{2})\-([0-9]{2})([0-9]{2})([0-9]{2})\-[0-9]{2}$");
            if (!match.Success)
            {
                throw new ArgumentOutOfRangeException($"{nameof(fileName)} is not in a recognised format. {nameof(fileName)} = '{fileName}'");
            }

            return new DateTime(int.Parse(match.Groups[1].Value),
                                int.Parse(match.Groups[2].Value),
                                int.Parse(match.Groups[3].Value),
                                int.Parse(match.Groups[4].Value),
                                int.Parse(match.Groups[5].Value),
                                int.Parse(match.Groups[6].Value));
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
