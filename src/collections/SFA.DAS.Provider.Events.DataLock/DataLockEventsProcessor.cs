using System;
using System.Linq;
using MediatR;
using SFA.DAS.Provider.Events.DataLock.Application.GetCurrentEvents;
using SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenEvents;
using SFA.DAS.Provider.Events.DataLock.Application.WriteDataLockEvent;
using SFA.DAS.Provider.Events.DataLock.Domain;

namespace SFA.DAS.Provider.Events.DataLock
{
    public class DataLockEventsProcessor
    {
        private readonly IMediator _mediator;

        public DataLockEventsProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected DataLockEventsProcessor()
        {
            // For unit testing
        }

        public virtual void Process()
        {
            var currentEventsResponse = ReturnValidGetCurrentEventsResponseOrThrow();
            var lastSeenEventsResponse = ReturnValidGetLastSeenEventsResponseOrThrow();

            if (!currentEventsResponse.HasAnyItems())
            {
                return;
            }

            foreach (var current in currentEventsResponse.Items)
            {
                var lastSeen = lastSeenEventsResponse.Items?.SingleOrDefault(ev => ev.Ukprn == current.Ukprn &&
                                                                                   ev.PriceEpisodeIdentifier == current.PriceEpisodeIdentifier &&
                                                                                   ev.Uln == current.Uln);

                if (EventsAreDifferent(current, lastSeen))
                {
                    current.ProcessDateTime = DateTime.Now;

                    _mediator.Send(new WriteDataLockEventCommandRequest
                    {
                        Event = current
                    });
                }
            }
        }

        private GetCurrentEventsResponse ReturnValidGetCurrentEventsResponseOrThrow()
        {
            var response = _mediator.Send(new GetCurrentEventsRequest());

            if (!response.IsValid)
            {
                throw response.Exception;
            }

            return response;
        }

        private GetLastSeenEventsResponse ReturnValidGetLastSeenEventsResponseOrThrow()
        {
            var response = _mediator.Send(new GetLastSeenEventsRequest());

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
                    p.CollectionPeriod.Name == currentPeriod.CollectionPeriod.Name &&
                    p.CollectionPeriod.Month == currentPeriod.CollectionPeriod.Month &&
                    p.CollectionPeriod.Year == currentPeriod.CollectionPeriod.Year &&
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
