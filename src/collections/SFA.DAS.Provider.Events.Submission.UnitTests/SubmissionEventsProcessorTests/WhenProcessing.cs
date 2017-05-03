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
            _mediator.Verify(m => m.Send(It.IsAny<WriteSubmissionEventCommand>()), Times.Exactly(1));

            // Provider with first submission
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].IlrFileName == ilrForFirstSubmission.IlrFileName)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].FileDateTime == ilrForFirstSubmission.FileDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].SubmittedDateTime == ilrForFirstSubmission.SubmittedDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].ComponentVersionNumber == SubmissionEventsTask.ComponentVersion)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].Ukprn == ilrForFirstSubmission.Ukprn)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].Uln == ilrForFirstSubmission.Uln)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].LearnRefNumber == ilrForFirstSubmission.LearnRefNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].AimSeqNumber == ilrForFirstSubmission.AimSeqNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].PriceEpisodeIdentifier == ilrForFirstSubmission.PriceEpisodeIdentifier)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].StandardCode == ilrForFirstSubmission.StandardCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].ProgrammeType == ilrForFirstSubmission.ProgrammeType)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].FrameworkCode == ilrForFirstSubmission.FrameworkCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].PathwayCode == ilrForFirstSubmission.PathwayCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].ActualStartDate == ilrForFirstSubmission.ActualStartDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].PlannedEndDate == ilrForFirstSubmission.PlannedEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].ActualEndDate == ilrForFirstSubmission.ActualEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].OnProgrammeTotalPrice == ilrForFirstSubmission.OnProgrammeTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].CompletionTotalPrice == ilrForFirstSubmission.CompletionTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[0].NiNumber == ilrForFirstSubmission.NiNumber)));

            // Provider with updated submission
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].IlrFileName == updatedSubmissionChanged.IlrFileName)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].FileDateTime == updatedSubmissionChanged.FileDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].SubmittedDateTime == updatedSubmissionChanged.SubmittedDateTime)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].ComponentVersionNumber == SubmissionEventsTask.ComponentVersion)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].Ukprn == updatedSubmissionChanged.Ukprn)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].Uln == updatedSubmissionChanged.Uln)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].LearnRefNumber == updatedSubmissionChanged.LearnRefNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].AimSeqNumber == updatedSubmissionChanged.AimSeqNumber)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].PriceEpisodeIdentifier == updatedSubmissionChanged.PriceEpisodeIdentifier)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].StandardCode == updatedSubmissionChanged.StandardCode)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].ProgrammeType == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].FrameworkCode == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].PathwayCode == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].ActualStartDate == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].PlannedEndDate == updatedSubmissionChanged.PlannedEndDate)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].ActualEndDate == null)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].OnProgrammeTotalPrice == updatedSubmissionChanged.OnProgrammeTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].CompletionTotalPrice == updatedSubmissionChanged.CompletionTotalPrice)));
            _mediator.Verify(m => m.Send(It.Is<WriteSubmissionEventCommand>(c => c.Events[1].NiNumber == null)));
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
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].IlrFileName == ilrForFirstSubmission.IlrFileName)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].FileDateTime == ilrForFirstSubmission.FileDateTime)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].SubmittedDateTime == ilrForFirstSubmission.SubmittedDateTime)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].Ukprn == ilrForFirstSubmission.Ukprn)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].Uln == ilrForFirstSubmission.Uln)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].LearnRefNumber == ilrForFirstSubmission.LearnRefNumber)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].AimSeqNumber == ilrForFirstSubmission.AimSeqNumber)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].PriceEpisodeIdentifier == ilrForFirstSubmission.PriceEpisodeIdentifier)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].StandardCode == ilrForFirstSubmission.StandardCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].ProgrammeType == ilrForFirstSubmission.ProgrammeType)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].FrameworkCode == ilrForFirstSubmission.FrameworkCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].PathwayCode == ilrForFirstSubmission.PathwayCode)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].ActualEndDate == ilrForFirstSubmission.ActualEndDate)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].PlannedEndDate == ilrForFirstSubmission.PlannedEndDate)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].OnProgrammeTotalPrice == ilrForFirstSubmission.OnProgrammeTotalPrice)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].CompletionTotalPrice == ilrForFirstSubmission.CompletionTotalPrice)), Times.Once);
            _mediator.Verify(m => m.Send(It.Is<WriteLastSeenIlrDetailsCommand>(c => c.LastSeenIlrs[0].NiNumber == ilrForFirstSubmission.NiNumber)), Times.Once);

        }


    }
}
