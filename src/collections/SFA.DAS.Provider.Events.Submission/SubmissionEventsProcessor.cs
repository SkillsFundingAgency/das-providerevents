using System.Linq;
using MediatR;
using SFA.DAS.Provider.Events.Submission.Application.GetCurrentVersions;
using SFA.DAS.Provider.Events.Submission.Application.GetLastSeenVersions;
using SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails;
using SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent;
using SFA.DAS.Provider.Events.Submission.Domain;

namespace SFA.DAS.Provider.Events.Submission
{
    public class SubmissionEventsProcessor
    {
        private readonly IMediator _mediator;

        public SubmissionEventsProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected SubmissionEventsProcessor()
        {
            // For unit testing
        }

        public virtual void Process()
        {
            var currentVersions = _mediator.Send(new GetCurrentVersionsQuery());
            var lastSeenVersions = _mediator.Send(new GetLastSeenVersionsQuery());

            foreach (var currentIlr in currentVersions.Items)
            {
                var lastSeenIlr = lastSeenVersions.Items.SingleOrDefault(ilr => ilr.Ukprn == currentIlr.Ukprn
                                                                             && ilr.Uln == currentIlr.Uln
                                                                             && ilr.PriceEpisodeIdentifier == currentIlr.PriceEpisodeIdentifier);
                var @event = CalculateDelta(currentIlr, lastSeenIlr);
                if (@event != null)
                {
                    _mediator.Send(new WriteSubmissionEventCommand
                    {
                        Event = @event
                    });
                }

                _mediator.Send(new WriteLastSeenIlrDetailsCommand
                {
                    LastSeenIlr = currentIlr
                });
            }
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
            }

            return @event;
        }
    }
}
