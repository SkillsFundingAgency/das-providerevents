using System;
using System.Linq;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.DataLock.Application.GetCurrentProviderEvents;
using SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenProviderEvents;
using SFA.DAS.Provider.Events.DataLock.Application.GetProviders;
using SFA.DAS.Provider.Events.DataLock.Application.WriteDataLockEvent;
using SFA.DAS.Provider.Events.DataLock.Domain;

namespace SFA.DAS.Provider.Events.DataLock
{
    public class DataLockEventsProcessor
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public DataLockEventsProcessor(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        protected DataLockEventsProcessor()
        {
            // For unit testing
        }

        public virtual void Process()
        {
            _logger.Info("Started data lock events processor");

            var providersResponse = ReturnValidGetProvidersQueryResponseOrThrow();
            _logger.Info($"Found {providersResponse.Items?.Length} providers to process");

            if (providersResponse.HasAnyItems())
            {
                foreach (var provider in providersResponse.Items)
                {
                    _logger.Info($"Starting to process provider {provider.Ukprn}");
                    var currentEventsResponse = ReturnValidGetCurrentProviderEventsResponseOrThrow(provider.Ukprn);
                    var lastSeenEventsResponse = ReturnValidGetLastSeenProviderEventsResponseOrThrow(provider.Ukprn);

                    if (!currentEventsResponse.HasAnyItems())
                    {
                        _logger.Info("Provider does not have any current events. Skipping");
                        continue;
                    }

                    foreach (var current in currentEventsResponse.Items)
                    {
                        _logger.Info($"Found event. Price episode = {current.PriceEpisodeIdentifier}, Uln = {current.Uln}");
                        var lastSeen = lastSeenEventsResponse.Items?.SingleOrDefault(ev => ev.Ukprn == current.Ukprn &&
                                                                                           ev.PriceEpisodeIdentifier == current.PriceEpisodeIdentifier &&
                                                                                           ev.Uln == current.Uln);

                        if (EventsAreDifferent(current, lastSeen))
                        {
                            _logger.Info("Event has changed");
                            current.Id = 0;
                            current.ProcessDateTime = DateTime.Now;

                            _mediator.Send(new WriteDataLockEventCommandRequest
                            {
                                Event = current
                            });
                        }
                        else
                        {
                            _logger.Info("Event is same as previous");
                        }
                    }
                }
            }
        }

        private GetProvidersQueryResponse ReturnValidGetProvidersQueryResponseOrThrow()
        {
            var response = _mediator.Send(new GetProvidersQueryRequest());

            if (!response.IsValid)
            {
                throw response.Exception;
            }

            return response;
        }

        private GetCurrentProviderEventsResponse ReturnValidGetCurrentProviderEventsResponseOrThrow(long ukprn)
        {
            var response = _mediator.Send(new GetCurrentProviderEventsRequest
            {
                Ukprn = ukprn
            });

            if (!response.IsValid)
            {
                throw response.Exception;
            }

            return response;
        }

        private GetLastSeenProviderEventsResponse ReturnValidGetLastSeenProviderEventsResponseOrThrow(long ukprn)
        {
            var response = _mediator.Send(new GetLastSeenProviderEventsRequest
            {
                Ukprn = ukprn
            });

            if (!response.IsValid)
            {
                throw response.Exception;
            }

            return response;
        }

        private bool EventsAreDifferent(DataLockEvent current, DataLockEvent lastSeen)
        {
            if (lastSeen == null)
            {
                return true;
            }

            if (current.CommitmentId != lastSeen.CommitmentId)
            {
                return true;
            }

            if (current.EmployerAccountId != lastSeen.EmployerAccountId)
            {
                return true;
            }

            if (current.HasErrors != lastSeen.HasErrors)
            {
                return true;
            }

            if (current.IlrStartDate != lastSeen.IlrStartDate)
            {
                return true;
            }

            if (current.IlrStandardCode != lastSeen.IlrStandardCode)
            {
                return true;
            }

            if (current.IlrProgrammeType != lastSeen.IlrProgrammeType)
            {
                return true;
            }

            if (current.IlrFrameworkCode != lastSeen.IlrFrameworkCode)
            {
                return true;
            }

            if (current.IlrPathwayCode != lastSeen.IlrPathwayCode)
            {
                return true;
            }

            if (current.IlrTrainingPrice != lastSeen.IlrTrainingPrice)
            {
                return true;
            }

            if (current.IlrEndpointAssessorPrice != lastSeen.IlrEndpointAssessorPrice)
            {
                return true;
            }

            if (current.IlrPriceEffectiveDate != lastSeen.IlrPriceEffectiveDate)
            {
                return true;
            }

            if (ErrorsAreDifferent(current.Errors, lastSeen.Errors))
            {
                return true;
            }

            if (PeriodsAreDifferent(current.Periods, lastSeen.Periods))
            {
                return true;
            }

            if (CommitmentVersionsAreDifferent(current.CommitmentVersions, lastSeen.CommitmentVersions))
            {
                return true;
            }

            return false;
        }

        private bool ErrorsAreDifferent(DataLockEventError[] current, DataLockEventError[] lastSeen)
        {
            if (current == null && lastSeen == null)
            {
                return false;
            }

            if (current == null || lastSeen == null)
            {
                return true;
            }

            if (current.Length != lastSeen.Length)
            {
                return true;
            }

            foreach (var currentError in current)
            {
                if (!lastSeen.Any(e =>
                    e.ErrorCode == currentError.ErrorCode &&
                    e.SystemDescription == currentError.SystemDescription))
                {
                    return true;
                }
            }

            return false;
        }

        private bool PeriodsAreDifferent(DataLockEventPeriod[] current, DataLockEventPeriod[] lastSeen)
        {
            if (current == null && lastSeen == null)
            {
                return false;
            }

            if (current == null || lastSeen == null)
            {
                return true;
            }

            if (current.Length != lastSeen.Length)
            {
                return true;
            }

            foreach (var currentPeriod in current)
            {
                if (!lastSeen.Any(p =>
                    p.CollectionPeriod?.Name == currentPeriod.CollectionPeriod?.Name &&
                    p.CollectionPeriod?.Month == currentPeriod.CollectionPeriod?.Month &&
                    p.CollectionPeriod?.Year == currentPeriod.CollectionPeriod?.Year &&
                    p.CommitmentVersion == currentPeriod.CommitmentVersion &&
                    p.IsPayable == currentPeriod.IsPayable &&
                    p.TransactionType == currentPeriod.TransactionType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CommitmentVersionsAreDifferent(DataLockEventCommitmentVersion[] current, DataLockEventCommitmentVersion[] lastSeen)
        {
            if (current == null && lastSeen == null)
            {
                return false;
            }

            if (current == null || lastSeen == null)
            {
                return true;
            }

            if (current.Length != lastSeen.Length)
            {
                return true;
            }

            foreach (var currentVersion in current)
            {
                if (!lastSeen.Any(v =>
                    v.CommitmentVersion == currentVersion.CommitmentVersion &&
                    v.CommitmentStartDate == currentVersion.CommitmentStartDate &&
                    v.CommitmentStandardCode == currentVersion.CommitmentStandardCode &&
                    v.CommitmentProgrammeType == currentVersion.CommitmentProgrammeType &&
                    v.CommitmentFrameworkCode == currentVersion.CommitmentFrameworkCode &&
                    v.CommitmentPathwayCode == currentVersion.CommitmentPathwayCode &&
                    v.CommitmentNegotiatedPrice == currentVersion.CommitmentNegotiatedPrice &&
                    v.CommitmentEffectiveDate == currentVersion.CommitmentEffectiveDate))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
