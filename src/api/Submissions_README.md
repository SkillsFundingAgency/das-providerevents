# Submissions API

Submissions is a feed of changes that have happened to the learning record. A new record is added to the API everytime a significant change occurs, and will only include details of what has changed since the last event. The first event for a record will include all details.

## Gettings submissions

Getting submissions can be done by consuming the following URI:

    GET https://host:port/api/submissions?periodId={since_event_id}&sinceTime={since_time}&page={page_number}
    
Where:
* since_event_id = (Optional) The event id that you want to read from. This is non-inclusive.
* since_time = (Optional) The time that you want to read from. This is non-inclusive.
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
      "IlrFileName": "ILR-80810436-1617-20170202-075857-01",
      "FileDateTime": "2017-02-02T00:00:00",
      "SubmittedDateTime": "2017-02-02T08:02:57.577",
      "ComponentVersionNumber": 1,
      "Ukprn": 80810436,
      "Uln": 1569840654,
      "StandardCode": 34,
      "ProgrammeType": 0,
      "FrameworkCode": 0,
      "PathwayCode": 0,
      "ActualStartDate": "2017-05-02T00:00:00",
      "PlannedEndDate": "2018-06-02T00:00:00",
      "OnProgrammeTotalPrice": 12000,
      "CompletionTotalPrice": 3000,
      "NiNumber": "AB123456A",
      "CommitmentId": 6757
    },
    {
      "Id": 2,
      "IlrFileName": "ILR-80810436-1617-20170202-075857-01",
      "FileDateTime": "2017-02-02T00:00:00",
      "SubmittedDateTime": "2017-02-03T010:00:33.832",
      "ComponentVersionNumber": 1,
      "Ukprn": 80810436,
      "Uln": 1569840654,
      "StandardCode": 46,
      "OnProgrammeTotalPrice": 18000,
      "CompletionTotalPrice": 4500
    }
  ]
}
```

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
// The above also optionally takes sinceEventId, sinceTime and page, e.g.
// var payments = await client.GetPayments(sinceEventId: 123, page: 2);
// var payments = await client.GetPayments(sinceTime: lastPollTime, page: 2);
```