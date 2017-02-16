using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetCurrentEvents
{
    public class GetCurrentEventsHandler : IRequestHandler<GetCurrentEventsRequest, GetCurrentEventsResponse>
    {
        private readonly IPriceEpisodeMatchRepository _priceEpisodeMatchRepository;
        private readonly IPriceEpisodePeriodMatchRepository _priceEpisodePeriodMatchRepository;
        private readonly IValidationErrorRepository _validationErrorRepository;
        private readonly IIlrPriceEpisodeRepository _ilrPriceEpisodeRepository;
        private readonly ICommitmentRepository _commitmentRepository;

        private readonly string _academicYear;
        private readonly EventSource _eventsSource;

        public GetCurrentEventsHandler(IPriceEpisodeMatchRepository priceEpisodeMatchRepository,
            IPriceEpisodePeriodMatchRepository priceEpisodePeriodMatchRepository,
            IValidationErrorRepository validationErrorRepository,
            IIlrPriceEpisodeRepository ilrPriceEpisodeRepository,
            ICommitmentRepository commitmentRepository,
            string yearOfCollection,
            EventSource eventsSource)
        {
            _priceEpisodeMatchRepository = priceEpisodeMatchRepository;
            _priceEpisodePeriodMatchRepository = priceEpisodePeriodMatchRepository;
            _validationErrorRepository = validationErrorRepository;
            _ilrPriceEpisodeRepository = ilrPriceEpisodeRepository;
            _commitmentRepository = commitmentRepository;

            _academicYear = yearOfCollection;
            _eventsSource = eventsSource;
        }

        public GetCurrentEventsResponse Handle(GetCurrentEventsRequest message)
        {
            try
            {
                var currentEvents = new List<DataLockEvent>();

                var priceEpisodeMatches = _priceEpisodeMatchRepository.GetCurrentPriceEpisodeMatches();

                if (priceEpisodeMatches != null)
                {
                    foreach (var match in priceEpisodeMatches)
                    {
                        var matchPeriods = _priceEpisodePeriodMatchRepository.GetPriceEpisodePeriodMatches(match.Ukprn, match.PriceEpisodeIdentifier, match.PriceEpisodeIdentifier);
                        var matchErrors = _validationErrorRepository.GetPriceEpisodeValidationErrors(match.Ukprn, match.PriceEpisodeIdentifier, match.LearnRefnumber);
                        var ilrData = _ilrPriceEpisodeRepository.GetPriceEpisodeIlrData(match.Ukprn, match.PriceEpisodeIdentifier, match.LearnRefnumber);
                        var commitmentVersions = _commitmentRepository.GetCommitmentVersions(match.CommitmentId);

                        var @event = new DataLockEvent
                        {
                            IlrFileName = ilrData.IlrFileName,
                            SubmittedDateTime = ilrData.SubmittedTime,
                            AcademicYear = _academicYear,
                            Ukprn = match.Ukprn,
                            Uln = ilrData.Uln,
                            LearnRefnumber = match.LearnRefnumber,
                            AimSeqNumber = match.AimSeqNumber,
                            PriceEpisodeIdentifier = match.PriceEpisodeIdentifier,
                            CommitmentId = match.CommitmentId,
                            EmployerAccountId = commitmentVersions[0].EmployerAccountId,
                            EventSource = _eventsSource,
                            HasErrors = !match.IsSuccess,
                            IlrStartDate = ilrData.IlrStartDate,
                            IlrStandardCode = ilrData.IlrStandardCode,
                            IlrProgrammeType = ilrData.IlrProgrammeType,
                            IlrFrameworkCode = ilrData.IlrFrameworkCode,
                            IlrPathwayCode = ilrData.IlrPathwayCode,
                            IlrTrainingPrice = ilrData.IlrTrainingPrice,
                            IlrEndpointAssessorPrice = ilrData.IlrEndpointAssessorPrice,
                            Errors = GetEventErrors(matchErrors),
                            Periods = GetEventPeriods(matchPeriods),
                            CommitmentVersions = GetEventCommitmentVersions(matchPeriods, commitmentVersions)
                        };

                        currentEvents.Add(@event);
                    }
                }

                return new GetCurrentEventsResponse
                {
                    IsValid = true,
                    Items = currentEvents.ToArray()
                };
            }
            catch (Exception ex)
            {
                return new GetCurrentEventsResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }

        private DataLockEventError[] GetEventErrors(ValidationErrorEntity[] validationErrors)
        {
            if (validationErrors == null)
            {
                return new DataLockEventError[0];
            }

            return validationErrors
                .Select(ve => new DataLockEventError
                {
                    ErrorCode = ve.RuleId,
                    SystemDescription = GetErrorDescription(ve.RuleId)
                })
                .ToArray();
        }

        private DataLockEventPeriod[] GetEventPeriods(PriceEpisodePeriodMatchEntity[] periodMatches)
        {
            if (periodMatches == null)
            {
                return new DataLockEventPeriod[0];
            }

            return periodMatches
                .Select(p => new DataLockEventPeriod
                {
                    CollectionPeriod = GetCollectionPeriod(p.Period),
                    CommitmentVersion = p.VersionId,
                    IsPayable = p.Payable,
                    TransactionType = (TransactionType) p.TransactionType
                })
                .ToArray();
        }

        private DataLockEventCommitmentVersion[] GetEventCommitmentVersions(PriceEpisodePeriodMatchEntity[] periodMatches, CommitmentEntity[] commitmentVersions)
        {
            if (periodMatches == null || commitmentVersions == null)
            {
                return new DataLockEventCommitmentVersion[0];
            }

            return periodMatches
                .Select(p =>
                {
                    var commitment = commitmentVersions
                        .Single(c => c.CommitmentId == p.CommitmentId && c.CommitmentVersion == p.VersionId);

                    return new DataLockEventCommitmentVersion
                    {
                        CommitmentVersion = commitment.CommitmentVersion,
                        CommitmentStartDate = commitment.StartDate,
                        CommitmentStandardCode = commitment.StandardCode,
                        CommitmentProgrammeType = commitment.ProgrammeType,
                        CommitmentFrameworkCode = commitment.FrameworkCode,
                        CommitmentPathwayCode = commitment.PathwayCode,
                        CommitmentNegotiatedPrice = commitment.NegotiatedPrice,
                        CommitmentEffectiveDate = commitment.EffectiveDate
                    };
                })
                .ToArray();
        }

        private string GetErrorDescription(string errorCode)
        {
            return errorCode;
        }

        private CollectionPeriod GetCollectionPeriod(int period)
        {
            var month = 0;
            var year = 2000;
            var name = string.Empty;

            switch (period)
            {
                case 1:
                    name = "R01";
                    month = 8;
                    year += int.Parse(_academicYear.Substring(0, 2));
                    break;
                case 2:
                    name = "R02";
                    month = 9;
                    year += int.Parse(_academicYear.Substring(0, 2));
                    break;
                case 3:
                    name = "R03";
                    month = 10;
                    year += int.Parse(_academicYear.Substring(0, 2));
                    break;
                case 4:
                    name = "R04";
                    month = 11;
                    year += int.Parse(_academicYear.Substring(0, 2));
                    break;
                case 5:
                    name = "R05";
                    month = 12;
                    year += int.Parse(_academicYear.Substring(0, 2));
                    break;
                case 6:
                    name = "R06";
                    month = 1;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 7:
                    name = "R07";
                    month = 2;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 8:
                    name = "R08";
                    month = 3;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 9:
                    name = "R09";
                    month = 4;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 10:
                    name = "R10";
                    month = 5;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 11:
                    name = "R11";
                    month = 6;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
                case 12:
                    name = "R12";
                    month = 7;
                    year += int.Parse(_academicYear.Substring(2, 2));
                    break;
            }

            return new CollectionPeriod
            {
                Name = $"{_academicYear}-{name}",
                Month = month,
                Year = year
            };
        }
    }
}