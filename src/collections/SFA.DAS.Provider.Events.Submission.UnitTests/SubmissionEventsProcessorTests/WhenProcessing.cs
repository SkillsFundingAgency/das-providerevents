using System;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.Application.GetCurrentVersions;
using SFA.DAS.Provider.Events.Submission.Application.GetLastSeenVersions;
using SFA.DAS.Provider.Events.Submission.Application.WriteLastSeenIlrDetails;
using SFA.DAS.Provider.Events.Submission.Application.WriteSubmissionEvent;
using SFA.DAS.Provider.Events.Submission.Domain;

namespace SFA.DAS.Provider.Events.Submission.UnitTests.SubmissionEventsProcessorTests
{
    public class WhenProcessing
    {
        private Mock<IMediator> _mediator;
        private SubmissionEventsProcessor _processor;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _mediator = new Mock<IMediator>();

            _processor = new SubmissionEventsProcessor(_logger.Object, _mediator.Object,"1617");
        }

        [Test]
        public void ThenItShouldWriteAnEventForEachIlrDeliveryThatHasChanged()
        {
            // Arrange
            var ilrForFirstSubmission = new IlrDetails
            {
                IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                FileDateTime = new DateTime(2017, 1, 1),
                SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                Ukprn = 123456,
                Uln = 987654,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                StandardCode = 34,
                ActualEndDate = new DateTime(2016, 12, 13),
                PlannedEndDate = new DateTime(2018, 2, 1),
                OnProgrammeTotalPrice = 12000,
                CompletionTotalPrice = 3000,
                NiNumber = "AB123456A"
            };
            var updatedSubmissionOriginal = new IlrDetails
            {
                IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                FileDateTime = new DateTime(2017, 1, 1),
                SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                Ukprn = 654789,
                Uln = 987654,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                StandardCode = 34,
                ActualEndDate = new DateTime(2016, 12, 13),
                PlannedEndDate = new DateTime(2018, 2, 1),
                OnProgrammeTotalPrice = 12000,
                CompletionTotalPrice = 3000,
                NiNumber = "AB123456A"
            };
            var updatedSubmissionChanged = new IlrDetails
            {
                IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                FileDateTime = new DateTime(2017, 1, 1),
                SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                Ukprn = 654789,
                Uln = 987654,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                StandardCode = 46,
                ActualEndDate = new DateTime(2016, 12, 13),
                PlannedEndDate = new DateTime(2018, 3, 1),
                OnProgrammeTotalPrice = 12500,
                CompletionTotalPrice = 3050,
                NiNumber = "AB123456A"
            };

            _mediator.Setup(m => m.Send(It.IsAny<GetCurrentVersionsQuery>()))
                .Returns(new GetCurrentVersionsQueryResponse
                {
                    IsValid = true,
                    Items = new[]
                    {
                        ilrForFirstSubmission,
                        updatedSubmissionChanged
                    }
                });
            _mediator.Setup(m => m.Send(It.IsAny<GetLastSeenVersionsQuery>()))
                .Returns(new GetLastSeenVersionsQueryResponse
                {
                    IsValid = true,
                    Items = new[]
                    {
                        updatedSubmissionOriginal
                    }
                });

            // Act
            _processor.Process();

            // Assert
            _mediator.Verify(m => m.Send(It.IsAny<WriteSubmissionEventCommand>()), Times.Exactly(2));

            // Provider with first submission
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.IlrFileName == ilrForFirstSubmission.IlrFileName)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.FileDateTime == ilrForFirstSubmission.FileDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.SubmittedDateTime == ilrForFirstSubmission.SubmittedDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ComponentVersionNumber == SubmissionEventsTask.ComponentVersion)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.Ukprn == ilrForFirstSubmission.Ukprn)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.Uln == ilrForFirstSubmission.Uln)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.LearnRefNumber == ilrForFirstSubmission.LearnRefNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.AimSeqNumber == ilrForFirstSubmission.AimSeqNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PriceEpisodeIdentifier == ilrForFirstSubmission.PriceEpisodeIdentifier)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.StandardCode == ilrForFirstSubmission.StandardCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ProgrammeType == ilrForFirstSubmission.ProgrammeType)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.FrameworkCode == ilrForFirstSubmission.FrameworkCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PathwayCode == ilrForFirstSubmission.PathwayCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ActualStartDate == ilrForFirstSubmission.ActualStartDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PlannedEndDate == ilrForFirstSubmission.PlannedEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ActualEndDate == ilrForFirstSubmission.ActualEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.OnProgrammeTotalPrice == ilrForFirstSubmission.OnProgrammeTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.CompletionTotalPrice == ilrForFirstSubmission.CompletionTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.NiNumber == ilrForFirstSubmission.NiNumber)));

            // Provider with updated submission
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.IlrFileName == updatedSubmissionChanged.IlrFileName)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.FileDateTime == updatedSubmissionChanged.FileDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.SubmittedDateTime == updatedSubmissionChanged.SubmittedDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ComponentVersionNumber == SubmissionEventsTask.ComponentVersion)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.Ukprn == updatedSubmissionChanged.Ukprn)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.Uln == updatedSubmissionChanged.Uln)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.LearnRefNumber == updatedSubmissionChanged.LearnRefNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.AimSeqNumber == updatedSubmissionChanged.AimSeqNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PriceEpisodeIdentifier == updatedSubmissionChanged.PriceEpisodeIdentifier)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.StandardCode == updatedSubmissionChanged.StandardCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ProgrammeType == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.FrameworkCode == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PathwayCode == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ActualStartDate == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.PlannedEndDate == updatedSubmissionChanged.PlannedEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.ActualEndDate == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.OnProgrammeTotalPrice == updatedSubmissionChanged.OnProgrammeTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.CompletionTotalPrice == updatedSubmissionChanged.CompletionTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Event.NiNumber == null)));
        }

        [Test]
        public void ThenItShouldUpdateLastSeenVersionToBeCurrentVersion()
        {
            // Arrange
            var ilrForFirstSubmission = new IlrDetails
            {
                IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                FileDateTime = new DateTime(2017, 1, 1),
                SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                Ukprn = 123456,
                Uln = 987654,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                StandardCode = 34,
                ActualEndDate = new DateTime(2016, 12, 13),
                PlannedEndDate = new DateTime(2018, 2, 1),
                OnProgrammeTotalPrice = 12000,
                CompletionTotalPrice = 3000,
                NiNumber = "AB123456A"
            };

            _mediator.Setup(m => m.Send(It.IsAny<GetCurrentVersionsQuery>()))
                .Returns(new GetCurrentVersionsQueryResponse
                {
                    IsValid = true,
                    Items = new[]
                    {
                        ilrForFirstSubmission
                    }
                });
            _mediator.Setup(m => m.Send(It.IsAny<GetLastSeenVersionsQuery>()))
                .Returns(new GetLastSeenVersionsQueryResponse
                {
                    IsValid = true,
                    Items = new IlrDetails[0]
                });

            // Act
            _processor.Process();

            // Assert
            _mediator.Verify(m => m.Send(It.IsAny<WriteLastSeenIlrDetailsCommand>()), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.IlrFileName == ilrForFirstSubmission.IlrFileName)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.FileDateTime == ilrForFirstSubmission.FileDateTime)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.SubmittedDateTime == ilrForFirstSubmission.SubmittedDateTime)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.Ukprn == ilrForFirstSubmission.Ukprn)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.Uln == ilrForFirstSubmission.Uln)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.LearnRefNumber == ilrForFirstSubmission.LearnRefNumber)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.AimSeqNumber == ilrForFirstSubmission.AimSeqNumber)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.PriceEpisodeIdentifier == ilrForFirstSubmission.PriceEpisodeIdentifier)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.StandardCode == ilrForFirstSubmission.StandardCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.ProgrammeType == ilrForFirstSubmission.ProgrammeType)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.FrameworkCode == ilrForFirstSubmission.FrameworkCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.PathwayCode == ilrForFirstSubmission.PathwayCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.ActualEndDate == ilrForFirstSubmission.ActualEndDate)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.PlannedEndDate == ilrForFirstSubmission.PlannedEndDate)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.OnProgrammeTotalPrice == ilrForFirstSubmission.OnProgrammeTotalPrice)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.CompletionTotalPrice == ilrForFirstSubmission.CompletionTotalPrice)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlr.NiNumber == ilrForFirstSubmission.NiNumber)), Times.Once);

        }


    }
}
