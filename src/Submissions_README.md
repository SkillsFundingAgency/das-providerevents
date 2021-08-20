# Submissions API

Submissions is a feed of changes that have happened to the learning record. A new record is added to the API everytime a significant change occurs, and will only include details of what has changed since the last event. The first event for a record will include all details.

## Gettings submissions

Getting submissions can be done by consuming the following URI:

    GET https://host:port/api/submissions?sinceEventId={since_event_id}&sinceTime={since_time}&ukprn={ukprn}&page={page_number}
    
Where:
* since_event_id = (Optional) The event id that you want to read from. This is non-inclusive.
* since_time = (Optional) The time that you want to read from. This is non-inclusive.
* ukprn = (Optional) learning provider ukprn to filter by, i.e. 12345. Default is null / no filter.
* page_number = (Optional) page number to display, i.e. 10. Default is 1

Note: You may specify since_event_id, since_time or neither. You may not specify both.

Response:
```json
{
  "PageNumber": 1,
  "TotalNumberOfPages": 1,
  "Items": [
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
      "ApprenticeshipId": 78
    },
    {
      "Id": 2,
      "IlrFileName": "ILR-80810463-1617-20170205-075957-01.xml",
      "FileDateTime": "2017-02-05T00:00:00",
      "SubmittedDateTime": "2017-02-05T08:05:57.577",
      "AcademicYear": "1617",
      "ComponentVersionNumber": 1,
      "Ukprn": 80810463,
      "Uln": 456,
      "EmployerReferenceNumber": 654321,
      "ProgrammeType": 20,
      "FrameworkCode": 550,
      "PathwayCode": 6,
      "ActualStartDate": "2017-06-01T00:00:00",
      "PlannedEndDate": "2018-07-27T00:00:00",
      "TrainingPrice": 6000,
      "EndpointAssessorPrice": 1500,
      "NiNumber": "AB123487A"
    }
  ]
}
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
var submissions = await client.GetSubmissionEvents();
// The above also optionally takes sinceEventId, sinceTime, ukprn and page, e.g.
// var submissions = await client.GetSubmissionEvents(sinceEventId: 123, page: 2);
// var submissions = await client.GetSubmissionEvents(sinceTime: lastPollTime, page: 2);
// var submissions = await client.GetSubmissionEvents(ukprn: 456, page: 2);
```
