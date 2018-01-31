using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.SubmissionsApiTests.When
{
    [TestFixture]
    public class RequestingSubmissionEvents
    {
        [Test]
        public async Task ThenTheEventDataIsCorrect()
        {
            var results = await IntegrationTestServer.Client.GetAsync("/api/v2/submissions?sinceEventId=0").ConfigureAwait(false);

            var resultsAsString = await results.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(resultsAsString);

            Assert.IsNotNull(events);
            Assert.IsNotNull(events.Items);

            // not checked
            // Id -> identity column

            // normal fields
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.AcademicYear), events.Items.Select(e => e.AcademicYear), "Checking AcademicYear");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.ActualEndDate), events.Items.Select(e => e.ActualEndDate), "Checking ActualEndDate");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.ActualStartDate), events.Items.Select(e => e.ActualStartDate), "Checking ActualStartDate");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.ComponentVersionNumber), events.Items.Select(e => e.ComponentVersionNumber), "Checking ComponentVersionNumber");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.EmployerReferenceNumber), events.Items.Select(e => e.EmployerReferenceNumber), "Checking EmployerReferenceNumber");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.FileDateTime), events.Items.Select(e => e.FileDateTime), "Checking FileDateTime");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.FrameworkCode), events.Items.Select(e => e.FrameworkCode), "Checking FrameworkCode");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.IlrFileName), events.Items.Select(e => e.IlrFileName), "Checking IlrFileName");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.NiNumber), events.Items.Select(e => e.NiNumber), "Checking NiNumber");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.PlannedEndDate), events.Items.Select(e => e.PlannedEndDate), "Checking PlannedEndDate");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.StandardCode), events.Items.Select(e => e.StandardCode), "Checking StandardCode");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.SubmittedDateTime), events.Items.Select(e => e.SubmittedDateTime), "Checking SubmittedDateTime");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.Ukprn), events.Items.Select(e => e.Ukprn), "Checking Ukprn");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.Uln), events.Items.Select(e => e.Uln), "Checking Uln");

            // renamed fields
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.CommitmentId), events.Items.Select(e => e.ApprenticeshipId), "Checking ApprenticeshipId");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.OnProgrammeTotalPrice), events.Items.Select(e => e.TrainingPrice), "Checking TrainingPrice");
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.CompletionTotalPrice), events.Items.Select(e => e.EndpointAssessorPrice), "Checking EndpointAssessorPrice");

            // new addition
            CollectionAssert.AreEqual(TestData.SubmissionEvents.Select(e => e.EPAOrgId), events.Items.Select(e => e.EPAOrgId), "Checking EPAOrgId");
        }
    }
}
