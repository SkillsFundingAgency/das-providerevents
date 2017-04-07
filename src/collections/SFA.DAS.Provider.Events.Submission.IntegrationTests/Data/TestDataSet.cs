using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Castle.Components.DictionaryAdapter;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class TestDataSet
    {
        public TestDataSet()
        {
            Providers = new List<ProviderEntity>();
            FileDetails = new List<FileDetailsEntity>();
            Learners = new EditableList<LearnerEntity>();
            LearningDeliveries = new List<LearningDeliveryEntity>();
            LearnersEmploymentStatuses = new List<LearnerEmploymentStatusEntity>();
            PriceEpisodes = new List<PriceEpisodeEntity>();
            PriceEpisodeMatches = new List<PriceEpisodeMatchEntity>();
            PreviousVersions = new List<LastSeenVersionEntity>();
        }

        public string AcademicYear { get; set; }
        
        public List<ProviderEntity> Providers { get; set; }
        public List<FileDetailsEntity> FileDetails { get; set; }
        public List<LearnerEntity> Learners { get; set; }
        public List<LearningDeliveryEntity> LearningDeliveries { get; set; }
        public List<LearnerEmploymentStatusEntity> LearnersEmploymentStatuses { get; set; }
        public List<PriceEpisodeEntity> PriceEpisodes { get; set; }
        public List<PriceEpisodeMatchEntity> PriceEpisodeMatches { get; set; }
        public List<LastSeenVersionEntity> PreviousVersions { get; set; }

        public void Store()
        {
            foreach (var provider in Providers)
            {
                ProviderRepository.Create(provider);
            }

            foreach (var fileDetails in FileDetails)
            {
                FileDetailsRepository.Create(fileDetails);
            }

            foreach (var learner in Learners)
            {
                LearnerRepository.Create(learner);
            }

            foreach (var learningDelivery in LearningDeliveries)
            {
                LearningDeliveryRepository.Create(learningDelivery);
            }

            foreach (var employmentStatus in LearnersEmploymentStatuses)
            {
                LearnerEmploymentStatusRepository.Create(employmentStatus);
            }

            foreach (var priceEpisode in PriceEpisodes)
            {
                PriceEpisodeRepository.Create(priceEpisode);
            }

            foreach (var match in PriceEpisodeMatches)
            {
                PriceEpisodeMatchRepository.Create(match);
            }

            foreach (var version in PreviousVersions)
            {
                LastSeenVersionRepository.Create(version);
            }
        }

        public void Clean()
        {
            ProviderRepository.Clean();
            FileDetailsRepository.Clean();
            LearnerRepository.Clean();
            LearningDeliveryRepository.Clean();
            LearnerEmploymentStatusRepository.Clean();
            PriceEpisodeRepository.Clean();
            PriceEpisodeMatchRepository.Clean();
        }

        public static TestDataSet GetFirstSubmissionDataset()
        {
            var rdm = new Random();

            var ukprn = rdm.Next(10000000, 99999999);
            var prepDateTime = DateTime.Now.AddMinutes(-5);
            var academicYear = GetAcademicYear(prepDateTime);
            var startDate = DateTime.Today.AddMonths(3);
            var learnRefNumber = "1";
            return new TestDataSet
            {
                AcademicYear = academicYear,
                Providers = new List<ProviderEntity>
                {
                    new ProviderEntity
                    {
                        Ukprn = ukprn
                    }
                },
                FileDetails = new List<FileDetailsEntity>
                {
                    new FileDetailsEntity
                    {
                        Ukprn = ukprn,
                        FileName = $"ILR-{ukprn}-{academicYear}-{prepDateTime.ToString("yyyyMMdd-HHmmss")}-01.xml",
                        SubmittedTime = DateTime.Now.AddMinutes(-1)
                    }
                },
                Learners = new List<LearnerEntity>
                {
                    new LearnerEntity
                    {
                        Ukprn = ukprn,
                        LearnRefNumber = learnRefNumber,
                        Uln = rdm.Next(1000000000, int.MaxValue),
                        NiNumber = "AB123456A"
                    }
                },
                LearningDeliveries = new List<LearningDeliveryEntity>
                {
                    new LearningDeliveryEntity
                    {
                        Ukprn = ukprn,
                        LearnRefNumber = learnRefNumber,
                        AimSeqNumber = 1,
                        StdCode = 34,
                        LearnStartDate = startDate,
                        LearnPlanEndDate = DateTime.Today.AddMonths(16)
                    }
                },
                LearnersEmploymentStatuses = new List<LearnerEmploymentStatusEntity>
                {
                    new LearnerEmploymentStatusEntity
                    {
                        Ukprn = ukprn,
                        LearnRefNumber = learnRefNumber,
                        EmploymentStatus = 10,
                        EmploymentStatusDate = startDate,
                        EmployerId = 123456
                    }
                },
                PriceEpisodes = new List<PriceEpisodeEntity>
                {
                    new PriceEpisodeEntity
                    {
                        PriceEpisodeIdentifier = $"00-34-01/{startDate.ToString("MM/yyyy")}",
                        Ukprn = ukprn,
                        LearnRefNumber = learnRefNumber,
                        PriceEpisodeAimSeqNumber = 1,
                        EpisodeEffectiveTnpStartDate = startDate,
                        Tnp1 = 12000,
                        Tnp2 = 3000
                    }
                },
                PriceEpisodeMatches = new List<PriceEpisodeMatchEntity>
                {
                    new PriceEpisodeMatchEntity
                    {
                        Ukprn = ukprn,
                        PriceEpisodeIdentifier = $"00-34-01/{startDate.ToString("MM/yyyy")}",
                        LearnRefNumber = learnRefNumber,
                        AimSeqNumber = 1,
                        CommitmentId = 567577,
                        IsSuccess = true
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
                var fileDetails = dataset.FileDetails.Single(fd => fd.Ukprn == learningDelivery.Ukprn);
                var learner = dataset.Learners.Single(l => l.Ukprn == learningDelivery.Ukprn && l.LearnRefNumber == learningDelivery.LearnRefNumber);
                var employmentStatus = dataset.LearnersEmploymentStatuses.Single(l => l.Ukprn == learningDelivery.Ukprn && l.LearnRefNumber == learningDelivery.LearnRefNumber);

                previousVersions.AddRange(dataset.PriceEpisodes
                    .Where(pe => pe.Ukprn == learningDelivery.Ukprn)
                    .Select(pe =>
                    {
                        var match = dataset.PriceEpisodeMatches.Single(m => m.Ukprn == pe.Ukprn && m.PriceEpisodeIdentifier == pe.PriceEpisodeIdentifier);

                        return new LastSeenVersionEntity
                        {
                            IlrFileName = fileDetails.FileName,
                            FileDateTime = ExtractPrepDateFromFileName(fileDetails.FileName),
                            SubmittedDateTime = fileDetails.SubmittedTime,
                            ComponentVersionNumber = SubmissionEventsTask.ComponentVersion,
                            Ukprn = provider.Ukprn,
                            Uln = learner.Uln,
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
                            NiNumber = learner.NiNumber,
                            CommitmentId = match.CommitmentId,
                            EmployerReferenceNumber = employmentStatus.EmployerId,
                            AcademicYear = dataset.AcademicYear
                        };
                    }));
            }

            return previousVersions;
        }
        private static DateTime ExtractPrepDateFromFileName(string fileName)
        {
            var match = Regex.Match(fileName, @"^ILR\-[0-9]{8,12}\-[0-9]{4}\-([0-9]{4})([0-9]{2})([0-9]{2})\-([0-9]{2})([0-9]{2})([0-9]{2})\-[0-9]{2}.xml$");
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
