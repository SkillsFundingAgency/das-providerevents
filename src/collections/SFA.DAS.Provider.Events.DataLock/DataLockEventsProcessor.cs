using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.DataLock.Application.GetCurrentCollectionPeriod;
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
                var eventsToStore = new List<DataLockEvent>();

                var currentPeriod = _mediator.Send(new GetCurrentCollectionPeriodRequest());
                var currentPeriodEndDate = new DateTime(
                    currentPeriod.CollectionPeriod.Year,
                    currentPeriod.CollectionPeriod.Month,
                    DateTime.DaysInMonth(currentPeriod.CollectionPeriod.Year, currentPeriod.CollectionPeriod.Month));

                foreach (var provider in providersResponse.Items)
                {
                    _logger.Info($"Starting to process provider {provider.Ukprn}");
                    var currentEventsResponse = ReturnValidGetCurrentProviderEventsResponseOrThrow(provider.Ukprn);
                    var lastSeenEventsResponse = ReturnValidGetLastSeenProviderEventsResponseOrThrow(provider.Ukprn);

                    var currentEvents = currentEventsResponse.HasAnyItems() ? currentEventsResponse.Items.ToList() : new List<DataLockEvent>();
                    var lastSeenEvents = lastSeenEventsResponse.HasAnyItems() ? lastSeenEventsResponse.Items.ToList() : new List<DataLockEvent>();

                    if (!currentEvents.Any() && !lastSeenEvents.Any())
                    {
                        _logger.Info("Provider does not have any current or existing events. Skipping");
                        continue;
                    }

                    // Look for events that are no longer in system
                    foreach (var lastSeen in lastSeenEvents)
                    {
                        _logger.Info($"Looking at last seen event for price episode = {lastSeen.PriceEpisodeIdentifier}, Uln = {lastSeen.Uln}");
                        var current = currentEvents.SingleOrDefault(ev => ev.Ukprn == lastSeen.Ukprn &&
                                                                            ev.PriceEpisodeIdentifier == lastSeen.PriceEpisodeIdentifier &&
                                                                            ev.LearnRefnumber == lastSeen.LearnRefnumber &&
                                                                            ev.CommitmentId == lastSeen.CommitmentId);
                        if (current == null)
                        {
                            _logger.Info("Event has been removed");
                            lastSeen.DataLockEventId = Guid.Empty;
                            lastSeen.Status = EventStatus.Removed;
                            eventsToStore.Add(lastSeen);
                        }
                        else if (EventsAreDifferent(current, lastSeen))
                        {
                            _logger.Info("Event has changed");
                            current.ProcessDateTime = DateTime.Now;
                            current.Status = EventStatus.Updated;

                            eventsToStore.Add(current);
                            currentEvents.Remove(current);
                        }
                        else
                        {
                            currentEvents.Remove(current);
                            _logger.Info("Event has not changed");
                        }
                    }

                    // Process new events
                    foreach (var current in currentEvents)
                    {
                        _logger.Info($"Found new event. Price episode = {current.PriceEpisodeIdentifier}, Uln = {current.Uln}");


                        _logger.Info("Event has changed");
                        current.ProcessDateTime = DateTime.Now;
                        current.Status = EventStatus.New;
                        
                        eventsToStore.Add(current);
                    }
                }

                if (eventsToStore.Any())
                {
                    foreach (var dataLockEvent in eventsToStore)
                    {
                        dataLockEvent.CurrentPeriodToDate = currentPeriodEndDate;
                    }

                    _mediator.Send(new WriteDataLockEventCommandRequest
                    {
                        Events = eventsToStore.ToArray()
                    });
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

            if (current.IlrPriceEffectiveFromDate != lastSeen.IlrPriceEffectiveFromDate)
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
