using System.Collections.Generic;
using System.Linq;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Submission.Application.GetCurrentVersions;
using SFA.DAS.Provider.Events.Submission.Application.GetLastSeenVersions;
using SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails;
using SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent;
using SFA.DAS.Provider.Events.Submission.Domain;

namespace SFA.DAS.Provider.Events.Submission
{
    public class SubmissionEventsProcessor
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly string _yearOfCollection;

        public SubmissionEventsProcessor(ILogger logger, IMediator mediator, string yearOfCollection)
        {
            _logger = logger;
            _mediator = mediator;
            _yearOfCollection = yearOfCollection;
        }
        protected SubmissionEventsProcessor()
        {
            // For unit testing
        }

        public virtual void Process()
        {
            _logger.Info("Started submission events processor");

            var currentVersions = _mediator.Send(new GetCurrentVersionsQuery());
            if (!currentVersions.IsValid)
            {
                _logger.Error(currentVersions.Exception, "Error getting current versions");
                throw currentVersions.Exception;
            }
            if (!currentVersions.HasAnyItems())
            {
                _logger.Info("Did not find any current versions. Exiting");
            }
            _logger.Info($"Found {currentVersions.Items.Length} current versions");

            var lastSeenVersions = _mediator.Send(new GetLastSeenVersionsQuery());
            _logger.Info($"Found {lastSeenVersions.Items.Length} previous versions");

            var changedSubmissions = new List<IlrDetails>();
            var events = new List<SubmissionEvent>();
            foreach (var currentIlr in currentVersions.Items)
            {
                _logger.Info($"Starting to compare {currentIlr.PriceEpisodeIdentifier} for uln {currentIlr.Uln}");

                var lastSeenIlr = lastSeenVersions.Items.SingleOrDefault(ilr => ilr.Ukprn == currentIlr.Ukprn
                                                                             && ilr.Uln == currentIlr.Uln
                                                                             && ilr.PriceEpisodeIdentifier == currentIlr.PriceEpisodeIdentifier);
                currentIlr.AcademicYear = _yearOfCollection;
                var @event = CalculateDelta(currentIlr, lastSeenIlr);
                if (@event != null)
                {
                    events.Add(@event);
                    _logger.Info("Current version has changed");
                }
                else
                {
                    _logger.Info("Current version has not changed");
                }

                changedSubmissions.Add(currentIlr);
            }

            _mediator.Send(new WriteSubmissionEventCommand
            {
                Events = events.ToArray()
            });
            _mediator.Send(new WriteLastSeenIlrDetailsCommand
            {
                LastSeenIlrs = changedSubmissions.ToArray()
            });
        }


        private SubmissionEvent CalculateDelta(IlrDetails currentIlr, IlrDetails lastSeenIlr)
        {
            SubmissionEvent @event = null;

            // Check for any changes in properties we care about
            if (currentIlr.StandardCode != lastSeenIlr?.StandardCode)
            {
                (@event = @event ?? new SubmissionEvent()).StandardCode = currentIlr.StandardCode;
            }

            if (currentIlr.ProgrammeType != lastSeenIlr?.ProgrammeType)
            {
                (@event = @event ?? new SubmissionEvent()).ProgrammeType = currentIlr.ProgrammeType;
            }

            if (currentIlr.FrameworkCode != lastSeenIlr?.FrameworkCode)
            {
                (@event = @event ?? new SubmissionEvent()).FrameworkCode = currentIlr.FrameworkCode;
            }

            if (currentIlr.PathwayCode != lastSeenIlr?.PathwayCode)
            {
                (@event = @event ?? new SubmissionEvent()).PathwayCode = currentIlr.PathwayCode;
            }

            if (currentIlr.ActualStartDate != lastSeenIlr?.ActualStartDate)
            {
                (@event = @event ?? new SubmissionEvent()).ActualStartDate = currentIlr.ActualStartDate;
            }

            if (currentIlr.PlannedEndDate != lastSeenIlr?.PlannedEndDate)
            {
                (@event = @event ?? new SubmissionEvent()).PlannedEndDate = currentIlr.PlannedEndDate;
            }

            if (currentIlr.ActualEndDate != lastSeenIlr?.ActualEndDate)
            {
                (@event = @event ?? new SubmissionEvent()).ActualEndDate = currentIlr.ActualEndDate;
            }

            if (currentIlr.OnProgrammeTotalPrice != lastSeenIlr?.OnProgrammeTotalPrice)
            {
                (@event = @event ?? new SubmissionEvent()).OnProgrammeTotalPrice = currentIlr.OnProgrammeTotalPrice;
            }

            if (currentIlr.CompletionTotalPrice != lastSeenIlr?.CompletionTotalPrice)
            {
                (@event = @event ?? new SubmissionEvent()).CompletionTotalPrice = currentIlr.CompletionTotalPrice;
            }

            if (currentIlr.NiNumber != lastSeenIlr?.NiNumber)
            {
                (@event = @event ?? new SubmissionEvent()).NiNumber = currentIlr.NiNumber;
            }

            if (currentIlr.CommitmentId != lastSeenIlr?.CommitmentId)
            {
                (@event = @event ?? new SubmissionEvent()).CommitmentId = currentIlr.CommitmentId;
            }

            if (currentIlr.EmployerReferenceNumber != lastSeenIlr?.EmployerReferenceNumber)
            {
                (@event = @event ?? new SubmissionEvent()).EmployerReferenceNumber = currentIlr.EmployerReferenceNumber;
            }


            // If there have been changes then set the standard properties
            if (@event != null)
            {
                @event.IlrFileName = currentIlr.IlrFileName;
                @event.FileDateTime = currentIlr.FileDateTime;
                @event.SubmittedDateTime = currentIlr.SubmittedDateTime;
                @event.ComponentVersionNumber = SubmissionEventsTask.ComponentVersion;
                @event.Ukprn = currentIlr.Ukprn;
                @event.Uln = currentIlr.Uln;
                @event.LearnRefNumber = currentIlr.LearnRefNumber;
                @event.AimSeqNumber = currentIlr.AimSeqNumber;
                @event.PriceEpisodeIdentifier = currentIlr.PriceEpisodeIdentifier;
                @event.EmployerReferenceNumber = currentIlr.EmployerReferenceNumber;
                @event.AcademicYear = currentIlr.AcademicYear;
            }

            return @event;
        }
    }
}
