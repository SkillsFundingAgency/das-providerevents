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
        private readonly IDataLockEventDataRepository _dataLockEventDataRepository;

        private readonly string _academicYear;
        private readonly EventSource _eventsSource;

        public GetCurrentProviderEventsHandler(IDataLockEventDataRepository dataLockEventDataRepository,
            string yearOfCollection,
            EventSource eventsSource)
        {
            _dataLockEventDataRepository = dataLockEventDataRepository;

            _academicYear = yearOfCollection;
            _eventsSource = eventsSource;
        }

        public GetCurrentProviderEventsResponse Handle(GetCurrentProviderEventsRequest message)
        {
            try
            {
                var currentEvents = new List<DataLockEvent>();

                var entities = _dataLockEventDataRepository.GetCurrentEvents(message.Ukprn);
                if (entities != null)
                {
                    DataLockEvent currentEvent = null;
                    var errors = new List<DataLockEventError>();
                    var periods = new List<DataLockEventPeriod>();
                    var commitmentVersions = new List<DataLockEventCommitmentVersion>();
                    foreach (var entity in entities)
                    {

                        if (currentEvent == null || entity.LearnRefNumber != currentEvent.LearnRefnumber || entity.PriceEpisodeIdentifier != currentEvent.PriceEpisodeIdentifier)
                            
                        {
                            if (currentEvent != null)
                            {
                                
                                var existingEvent = currentEvents.FirstOrDefault(x => x.LearnRefnumber == currentEvent.LearnRefnumber && x.PriceEpisodeIdentifier == currentEvent.PriceEpisodeIdentifier);
                                if (existingEvent == null)
                                {
                                    currentEvent.Errors = errors.ToArray();
                                    currentEvent.Periods = periods.ToArray();
                                    currentEvent.CommitmentVersions = commitmentVersions.ToArray();
                                    currentEvents.Add(currentEvent);
                                }
                                else
                                {
                                    var existingErrors = existingEvent.Errors.ToList();
                                    existingErrors.AddRange(errors);
                                    existingEvent.Errors = existingErrors.ToArray();

                                    var existingPeriods = existingEvent.Periods.ToList();
                                    existingPeriods.AddRange(periods);
                                    existingEvent.Periods = existingPeriods.ToArray();

                                    var existingCommitments = existingEvent.CommitmentVersions.ToList();
                                    existingCommitments.AddRange(commitmentVersions);
                                    existingEvent.CommitmentVersions = existingCommitments.ToArray();
                                }
                            }

                            errors.Clear();
                            periods.Clear();
                            commitmentVersions.Clear();
                            currentEvent = new DataLockEvent
                            {
                                IlrFileName = entity.IlrFilename,
                                SubmittedDateTime = entity.SubmittedTime,
                                AcademicYear = _academicYear,
                                Ukprn = entity.Ukprn,
                                Uln = entity.Uln,
                                LearnRefnumber = entity.LearnRefNumber,
                                AimSeqNumber = entity.AimSeqNumber,
                                PriceEpisodeIdentifier = entity.PriceEpisodeIdentifier,
                                CommitmentId = entity.CommitmentId,
                                EmployerAccountId = entity.EmployerAccountId,
                                EventSource = _eventsSource,
                                HasErrors = !entity.IsSuccess,
                                IlrStartDate = entity.IlrStartDate,
                                IlrStandardCode = entity.IlrStandardCode,
                                IlrProgrammeType = entity.IlrProgrammeType,
                                IlrFrameworkCode = entity.IlrFrameworkCode,
                                IlrPathwayCode = entity.IlrPathwayCode,
                                IlrTrainingPrice = entity.IlrTrainingPrice,
                                IlrEndpointAssessorPrice = entity.IlrEndpointAssessorPrice,
                                IlrPriceEffectiveDate = entity.IlrPriceEffectiveDate
                            };
                        }

                        var collectionPeriod = GetCollectionPeriod(entity.Period);
                        if (!periods.Any(p => p.CollectionPeriod.Month == collectionPeriod.Month
                                           && p.CollectionPeriod.Year == collectionPeriod.Year
                                           && (int)p.TransactionType == entity.TransactionType))
                        {
                            periods.Add(new DataLockEventPeriod
                            {
                                CollectionPeriod = collectionPeriod,
                                CommitmentVersion = entity.CommitmentVersionId,
                                IsPayable = entity.Payable,
                                TransactionType = (TransactionType)entity.TransactionType
                            });
                        }

                        if (!errors.Any(e => e.ErrorCode == entity.RuleId))
                        {
                            errors.Add(new DataLockEventError
                            {
                                ErrorCode = entity.RuleId,
                                SystemDescription = GetErrorDescription(entity.RuleId)
                            });
                        }

                        if (!commitmentVersions.Any(v => v.CommitmentVersion == entity.CommitmentVersionId))
                        {
                            commitmentVersions.Add(new DataLockEventCommitmentVersion
                            {
                                CommitmentVersion = entity.CommitmentVersionId,
                                CommitmentStandardCode = entity.CommitmentStandardCode,
                                CommitmentProgrammeType = entity.CommitmentProgrammeType,
                                CommitmentFrameworkCode = entity.CommitmentFrameworkCode,
                                CommitmentPathwayCode = entity.CommitmentPathwayCode,
                                CommitmentStartDate = entity.CommitmentStartDate,
                                CommitmentNegotiatedPrice = entity.CommitmentNegotiatedPrice,
                                CommitmentEffectiveDate = entity.CommitmentEffectiveDate
                            });
                        }
                    }
                    if (currentEvent != null)
                    {
                        currentEvent.Errors = errors.ToArray();
                        currentEvent.Periods = periods.ToArray();
                        currentEvent.CommitmentVersions = commitmentVersions.ToArray();
                        currentEvents.Add(currentEvent);
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
                case "DLOCK_11":
                    return "The employer is not currently a levy payer";
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