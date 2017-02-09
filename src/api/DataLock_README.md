# Data Lock Events API

Data lock events is a feed of changes that have happened to the data lock results. A new record is added to the API every time a data lock status change occurs, and will include details of what has changed since the last event. The first event for a record will include the initial data lock status (success or failure) and all relevant information including a list of all related errors.

## Getting data lock events

Getting data lock events can be done by consuming the following URI:

    GET https://host:port/api/datalock?sinceEventId={since_event_id}&sinceTime={since_time}&employerAccountId={employer_account_id}&ukprn={ukprn}page={page_number}
    
Where:
* since_event_id = (Optional) The event id that you want to read from. This is non-inclusive.
* since_time = (Optional) The time that you want to read from. This is non-inclusive.
* employer_account_id = (Optional) employer account identifier to filter by, i.e. 12345. Default is null / no filter.
* ukprn = (Optional) learning provider ukprn to filter by, i.e. 12345. Default is null / mo filter.
* page_number = (Optional) page number to display, i.e. 10. Default is 1.

Note: You may specify since_event_id, since_time or neither. You may not specify both.

Response:
```json
{
  "PageNumber": 1,
  "TotalNumberOfPages": 1,
  "Items": [
    {
      "Id": 1,
      "ProcessDateTime": "2017-02-09T15:45:30",
      "IlrFileName": "ILR-123456",
      "Ukprn": 123456,
      "Uln": 123,
      "LearnRefNumber": "Lrn-001",
      "AimSeqNumber": 1,
      "PriceEpisodeIdentifier": "25-27-01/05/2017",
      "ApprenticeshipId": 1,
      "EmployerAccountId": 999,
      "EventSource": "Submission",
      "HasErrors": true,
      "IlrStartDate": "2017-05-01T00:00:00",
      "IlrStandardCode": 27,
      "IlrTrainingPrice": 12000,
      "IlrEndpointAssessorPrice": 3000,
      "Errors": [
        {
          "ErrorCode": "ERR09",
          "SystemDescription": "Mismatch on price"
        }
      ],
      "Periods": [
        {
          "ApprenticeshipVersion": 99,
          "Period": {
            "Id": "1617-R10",
            "Month": 5,
            "Year": 2017
          },
          "IsPayable": false
        },
        {
          "ApprenticeshipVersion": 99,
          "Period": {
            "Id": "1617-R11",
            "Month": 6,
            "Year": 2017
          },
          "IsPayable": false
        },
        {
          "ApprenticeshipVersion": 99,
          "Period": {
            "Id": "1617-R12",
            "Month": 7,
            "Year": 2017
          },
          "IsPayable": false
        }
      ],
      "Apprenticeships": [
        {
          "Version": 99,
          "StartDate": "2017-05-01T00:00:00",
          "StandardCode": 27,
          "NegotiatedPrice": 17500,
          "EffectiveDate": "2017-05-01T00:00:00"
        }
      ]
    },
    {
      "Id": 2,
      "ProcessDateTime": "2017-02-09T09:10:23",
      "IlrFileName": "ILR-654321",
      "Ukprn": 654321,
      "Uln": 321,
      "LearnRefNumber": "Lrn-100",
      "AimSeqNumber": 0,
      "PriceEpisodeIdentifier": "20-550-6-01/06/2017",
      "ApprenticeshipId": 9,
      "EmployerAccountId": 456,
      "EventSource": "PeriodEnd",
      "HasErrors": false,
      "IlrStartDate": "2017-06-01T00:00:00",
      "IlrProgrammeType": 20,
      "IlrFrameworkCode": 550,
      "IlrPathwayCode": 6,
      "IlrTrainingPrice": 6000,
      "IlrEndpointAssessorPrice": 1500,
      "Errors": [],
      "Periods": [
        {
          "ApprenticeshipVersion": 17,
          "Period": {
            "Id": "1617-R11",
            "Month": 6,
            "Year": 2017
          },
          "IsPayable": true
        },
        {
          "ApprenticeshipVersion": 25,
          "Period": {
            "Id": "1617-R12",
            "Month": 7,
            "Year": 2017
          },
          "IsPayable": false
        }
      ],
      "Apprenticeships": [
        {
          "Version": 17,
          "StartDate": "2017-06-01T00:00:00",
          "ProgrammeType": 20,
          "FrameworkCode": 550,
          "PathwayCode": 6,
          "NegotiatedPrice": 7500,
          "EffectiveDate": "2017-06-01T00:00:00"
        },
        {
          "Version": 25,
          "StartDate": "2017-06-01T00:00:00",
          "ProgrammeType": 20,
          "FrameworkCode": 550,
          "PathwayCode": 6,
          "NegotiatedPrice": 7500,
          "EffectiveDate": "2017-07-15T00:00:00"
        }
      ]
    }
  ]
}
```

Response **Items** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| Id | long | no | event unique identifier |
| ProcessDateTime | DateTime | No | ??? |
| IlrFileName | string | no | name of the related ilr file |
| Ukprn | long | no | learning provider's ukprn |
| Uln | long | no | learner's unique number |
| LearnRefNumber | string | no | learner unique identifier inside the ilr file |
| AimSeqNumber | long | no | learner's unique learning aim sequence number inside the ilr file |
| PriceEpisodeIdentifier | string | no | learning price episode unique identifier for the ilr file |
| ApprenticeshipId | long | no | matched apprenticeship unique identifier |
| EmployerAccountId | long | no | matched apprenticeship's employer account unique identifier |
| EventSource | EventSource | no | source of the data lock event: **Submission** (generated by an ilr submission) or **PeriodEnd** (generated by a period end run) |
| HasErrors | bool | no | flag indicating whether the data lock event represents success or failure |
| IlrStartDate | DateTime | yes | ilr price episode start date |
| IlrStandardCode | long | yes | ilr price episode standard code |
| IlrProgrammeType | int | yes | ilr price episode programme type |
| IlrFrameworkCode | int | yes | ilr price episode frameworh code |
| IlrPathwayCode | int | yes | ilr price episode pathway code |
| IlrTrainingPrice | decimal | yes | ilr price episode training price |
| IlrEndpointAssessorPrice | decimal | yes | ilr price episode endpoint assessor price |
| Errors | DataLockEventError[] | no | event's list of errors, see following table for structure |
| Periods | DataLockEventPeriod[] | no | event's list of ilr collection periods, see following table for structure |
| Apprenticeships | DataLockEventApprenticeship[] | no | event's list of matched apprenticeships, see following table for structure |

**Errors** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| ErrorCode | string | no | error code |
| SystemDescription | string | no | error description |

**Periods** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| ApprenticeshipVersion | long | no | apprenticeship version |
| Period | NamedCollectionPeriod | no | ilr collection period containing an id, calendar month and calcndar year |
| IsPayable | bool | no | whether any earnings against the period will be paid or not |

**Apprenticeships** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| Version | long | no | apprenticeship version |
| StartDate | DateTime | no | apprenticeship funding start date |
| StandardCode | long | yes | apprenticeship standard code |
| ProgrammeType | int | yes | apprenticeship programme type |
| FrameworkCode | int | yes | apprenticeship framework code |
| PathwayCode | int | yes | apprenticeship pathway code |
| NegotiatedPrice | decimal | no | apprenticeship negotiated price |
| EffectiveDate | DateTime | no | apprenticeship change effective from date |

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
var dataLockEvents = await client.GetDataLockEvents();
// The above also optionally takes sinceEventId, sinceTime, employerAccountId, ukprn and page, e.g.
// var dataLockEvents = await client.GetDataLockEvents(sinceEventId: 123, page: 2);
// var dataLockEvents = await client.GetDataLockEvents(sinceTime: lastPollTime, page: 2);
// var dataLockEvents = await client.GetDataLockEvents(employerAccountId: 123, page: 2);
// var dataLockEvents = await client.GetDataLockEvents(ukprn: 456, page: 2);
```
