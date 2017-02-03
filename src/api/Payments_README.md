# Payments API

Payments is a feed of low level payments made by the system. They can optionally be filtered.

## Get payments

Getting payments can be done by consuming the following URI:

    GET https://host:port/api/payments?periodId={period_id}&employerAccountId={employer_account_id}&page={page_number}

Where:
* period_id = (Optional) period identifier to filter by, i.e. 1617-R02. Default is null / no filter
* employer_account_id = (Optional) employer account identifier to filter by, i.e. 12345. Default is null / no filter
* page_number = (Optional) page number to display, i.e. 10. Default is 1

Response:
```json
{
  "PageNumber": 1,
  "TotalNumberOfPages": 1,
  "Items": [
    {
      "Id": "631E716C-C760-48BC-9BA1-9EAF78F6BF6C",
      "Ukprn": 234432,
      "Uln": 45126,
      "DeliveryPeriod": {
        "Month": 4,
        "Year": 2017
      },
      "CollectionPeriod": {
        "Id": "1617-R09",
        "Month": 4,
        "Year": 2017
      },
      "EvidenceSubmittedOn": "2017-01-09T10:40:11.68",
      "FundingSource": "CoInvestedEmployer",
      "TransactionType": "Learning",
      "Amount": 110.76923,
      "FrameworkCode": 550,
      "ProgrammeType": 20,
      "PathwayCode": 6,
      "ContractType": "ContractWithSfa"
    },
    {
      "Id": "E04992C3-3A54-40A5-89DC-A1432B5993B5",
      "Ukprn": 234432,
      "Uln": 45126,
      "DeliveryPeriod": {
        "Month": 4,
        "Year": 2017
      },
      "CollectionPeriod": {
        "Id": "1617-R09",
        "Month": 4,
        "Year": 2017
      },
      "EvidenceSubmittedOn": "2017-01-09T10:40:11.68",
      "FundingSource": "CoInvestedSfa",
      "TransactionType": "Learning",
      "Amount": 996.92308,
      "FrameworkCode": 550,
      "ProgrammeType": 20,
      "PathwayCode": 6,
      "ContractType": "ContractWithSfa"
    },
    {
      "Id": "D08BEF99-382A-4138-8B69-F235A6529F18",
      "Ukprn": 234432,
      "Uln": 23423,
      "EmployerAccountId": "602844077",
      "ApprenticeshipId": 999,
      "DeliveryPeriod": {
        "Month": 4,
        "Year": 2017
      },
      "CollectionPeriod": {
        "Id": "1617-R09",
        "Month": 4,
        "Year": 2017
      },
      "EvidenceSubmittedOn": "2017-01-09T10:40:11.68",
      "EmployerAccountVersion": "20170104115139",
      "ApprenticeshipVersion": "1",
      "FundingSource": "Levy",
      "TransactionType": "Learning",
      "Amount": 923.07692,
      "StandardCode": 25,
      "ContractType": "ContractWithEmployer"
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
var payments = await client.GetPayments();
// The above also optionally takes periodId, employerAccountId and page, e.g.
// var payments = await client.GetPayments("1617-R02", "1029102", 2);
```