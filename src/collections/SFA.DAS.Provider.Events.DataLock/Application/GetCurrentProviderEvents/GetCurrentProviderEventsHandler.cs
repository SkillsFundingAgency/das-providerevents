using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetCurrentProviderEvents
{
    public class GetCurrentProviderEventsHandler : IRequestHandler<GetCurrentProviderEventsRequest, GetCurrentProviderEventsResponse>
    {
        private readonly IPriceEpisodeMatchRepository _priceEpisodeMatchRepository;
        private readonly IPriceEpisodePeriodMatchRepository _priceEpisodePeriodMatchRepository;
        private readonly IValidationErrorRepository _validationErrorRepository;
        private readonly IIlrPriceEpisodeRepository _ilrPriceEpisodeRepository;
        private readonly ICommitmentRepository _commitmentRepository;

        private readonly string _academicYear;
        private readonly EventSource _eventsSource;

        public GetCurrentProviderEventsHandler(IPriceEpisodeMatchRepository priceEpisodeMatchRepository,
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

        public GetCurrentProviderEventsResponse Handle(GetCurrentProviderEventsRequest message)
        {
            try
            {
                var currentEvents = new List<DataLockEvent>();

                var priceEpisodeMatches = _priceEpisodeMatchRepository.GetProviderPriceEpisodeMatches(message.Ukprn);

                if (priceEpisodeMatches != null)
                {
                    foreach (var match in priceEpisodeMatches)
                    {
                        var matchPeriods = _priceEpisodePeriodMatchRepository.GetPriceEpisodePeriodMatches(match.Ukprn, match.PriceEpisodeIdentifier, match.LearnRefnumber);
                        var matchErrors = _validationErrorRepository.GetPriceEpisodeValidationErrors(match.Ukprn, match.PriceEpisodeIdentifier, match.LearnRefnumber);
                        var ilrData = _ilrPriceEpisodeRepository.GetPriceEpisodeIlrData(match.Ukprn, match.PriceEpisodeIdentifier, match.LearnRefnumber);
                        var commitmentVersions = _commitmentRepository.GetCommitmentVersions(match.CommitmentId);

                        if (ilrData == null)
                        {
                            throw new Exception($"Could not find ILR data for price episode with ukprn {match.Ukprn}, identifier {match.PriceEpisodeIdentifier}, learner reference number {match.LearnRefnumber}");
                        }

                        if (commitmentVersions == null || commitmentVersions.Length == 0)
                        {
                            throw new Exception($"Could not find any versions for commitment with id {match.CommitmentId}");
                        }

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

                return new GetCurrentProviderEventsResponse
                {
                    IsValid = true,
                    Items = currentEvents.ToArray()
                };
            }
            catch (Exception ex)
            {
                return new GetCurrentProviderEventsResponse
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
            var versions = new List<DataLockEventCommitmentVersion>();

            if (periodMatches == null || commitmentVersions == null)
            {
                return new DataLockEventCommitmentVersion[0];
            }

            foreach (var version in commitmentVersions)
            {
                var period = periodMatches
                    .FirstOrDefault(p => p.CommitmentId == version.CommitmentId && p.VersionId == version.CommitmentVersion);

                if (period != null)
                {
                    versions.Add(new DataLockEventCommitmentVersion
                    {
                        CommitmentVersion = version.CommitmentVersion,
                        CommitmentStartDate = version.StartDate,
                        CommitmentStandardCode = version.StandardCode,
                        CommitmentProgrammeType = version.ProgrammeType,
                        CommitmentFrameworkCode = version.FrameworkCode,
                        CommitmentPathwayCode = version.PathwayCode,
                        CommitmentNegotiatedPrice = version.NegotiatedPrice,
                        CommitmentEffectiveDate = version.EffectiveDate
                    });
                }
            }

            return versions.ToArray();
        }

        private string GetErrorDescription(string errorCode)
        {
            switch (errorCode)
            {
                case "DLOCK_01":
                    return "No matching record found in an employer digital account for the UKPRN";
                case "DLOCK_02":
                    return "No matching record found in the employer digital account for the ULN";
                case "DLOCK_03":
                    return "No matching record found in the employer digital account for the standard code";
                case "DLOCK_04":
                    return "No matching record found in the employer digital account for the framework code";
                case "DLOCK_05":
                    return "No matching record found in the employer digital account for the programme type";
                case "DLOCK_06":
                    return "No matching record found in the employer digital account for the pathway code";
                case "DLOCK_07":
                    return "No matching record found in the employer digital account for the negotiated cost of training";
                case "DLOCK_08":
                    return "Multiple matching records found in the employer digital account";
                case "DLOCK_09":
                    return "The start date for this negotiated price is before the corresponding price start date in the employer digital account";
                case "DLOCK_10":
                    return "The employer has stopped payments for this apprentice";
                default:
                    return errorCode;
            }
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