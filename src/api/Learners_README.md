# Learners API

Learners is an endpoint to allow retrieval of learner information. A learner object will be returned for each Standard that the learner is associated with.

## Gettings submissions

Getting learner details can be done by consuming the following URI:

    GET https://host:port/api/learners?uln={uln}&sinceEventId={since_event_id}&page_number={page_number}
    
Where:
* uln = (Optional) The learner's ULN to filter on.
* since_event_id = (Optional) The event id that you want to read from. This is non-inclusive.
* page_number = (Optional) page number to display, i.e. 10. Default is 1

Response:
```json
[
  {
    "Id": 1,
    "IlrFileName": "ILR-80810436-1617-20170202-075857-01.xml",
    "FileDateTime": "2017-02-02T00:00:00",
    "SubmittedDateTime": "2017-02-02T08:02:57.577",
    "AcademicYear": "1617",
    "ComponentVersionNumber": 1,
    "Ukprn": 80810436,
    "Uln": 321,
    "EmployerReferenceNumber": 123456,
    "StandardCode": 27,
    "ActualStartDate": "2017-05-01T00:00:00",
    "PlannedEndDate": "2018-06-15T00:00:00",
    "TrainingPrice": 12000,
    "EndpointAssessorPrice": 3000,
    "NiNumber": "AB123456A",
    "ApprenticeshipId": 78,
    "EPAOrgId": "EPAO0001",
    "GivenNames":"John",
    "FamilyName":"Jones",
    "CompStatus":1
  },
  {
    "Id": 1,
    "IlrFileName": "ILR-80810436-1617-20170202-075857-01.xml",
    "FileDateTime": "2017-02-02T00:00:00",
    "SubmittedDateTime": "2017-02-02T08:02:57.577",
    "AcademicYear": "1617",
    "ComponentVersionNumber": 1,
    "Ukprn": 80810436,
    "Uln": 321,
    "EmployerReferenceNumber": 123456,
    "StandardCode": 104,
    "ActualStartDate": "2017-05-01T00:00:00",
    "PlannedEndDate": "2018-06-15T00:00:00",
    "TrainingPrice": 12000,
    "EndpointAssessorPrice": 3000,
    "NiNumber": "AB123456A",
    "ApprenticeshipId": 78,
    "EPAOrgId": "EPAO0001",
    "GivenNames":"John",
    "FamilyName":"Jones",
    "CompStatus":1
  }
]
```

Response **Items** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| Id | long | no | event unique identifier |
| IlrFileName | string | no | name of the related ilr file |
| FileDateTime | DateTime | no | ilr file date and time |
| SubmittedDateTime | DateTime | no | ilr file submission date and time |
| AcademicYear | string | no | ilr academic year |
| ComponentVersionNumber | int | no | version of the compoonent that created the event |
| Ukprn | long | no | learning provider's ukprn |
| Uln | long | no | learner's unique number |
| EmployerReferenceNumber | int | yes | ilr learner employer's reference number |
| StandardCode | long | yes | ilr learning standard code |
| ProgrammeType | int | yes | ilr learning programme type |
| FrameworkCode | int | yes | ilr learning frameworh code |
| PathwayCode | int | yes | ilr learning pathway code |
| ActualStartDate | DateTime | yes | ilr learning start date |
| PlannedEndDate | DateTime | yes | ilr learning planned end date |
| ActualEndDate | DateTime | yes | ilr learning actual date |
| TrainingPrice | decimal | yes | ilr learning training price |
| EndpointAssessorPrice | decimal | yes | ilr learning endpoint assessor price |
| NiNumber | string | yes | ilr learner NI Number |
| ApprenticeshipId | long | yes | apprenticeship unique identifier |
|EPAOrgId|string|no|EPAO Id|
|GivenNames|string|no|Given names of the learner|
|FamilyName|string|no|Family name of the learner|
|CompStatus|int|yes|The completion status of the apprenticeship|

## Using the client

A .NET client also exists to easy calling the api and encapsulates the authentication and deserialization. It can be added from nuget:

```powershell
Install-Package SFA.DAS.Provider.Events.Api.Client
```

And then called by:

```csharp
var config = new PaymentsEventsApiConfiguration
{
    ClientToken = "YOUR_JWT_TOKEN",
    ApiBaseUrl = "https://some-server/"
};
var client = new PaymentsEventsApiClient(config);
var submissions = await client.GetLatestLearnerEventForStandards(uln: 1234567891);
// The above also optionally takes sinceEventId, e.g.
// var submissions = await client.GetLatestLearnerEventForStandards(uln: 1234567891, sinceEventId: 123);
```
