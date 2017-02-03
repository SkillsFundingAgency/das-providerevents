# Period Ends API

Period ends are a slow moving feed of items signifying a successful completion of a period end.

## Get period ends

Getting period ends can be done by consuming the following URI:

    GET https://host:port/api/periodends/

Response:
```json
[
  {
    "Id": "1617-R12",
    "CalendarPeriod": {
      "Month": 8,
      "Year": 2017
    },
    "ReferenceData": {
      "AccountDataValidAt": "2017-09-01T00:00:00",
      "CommitmentDataValidAt": "2017-08-31T00:00:00"
    },
    "CompletionDateTime": "2017-09-05T19:00:00",
    "_links": {
      "PaymentsForPeriod": "https://host:port/api/payments?periodId=1617-R12"
    }
  }
]
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
var periodEnds = await client.GetPeriodEnds();
```