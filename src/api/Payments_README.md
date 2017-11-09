# Payments API

Payments is a feed of low level payments made by the system. They can optionally be filtered.

## Get payments

Getting payments can be done by consuming the following URI:

```
GET https://host:port/api/payments?periodId={period_id}&employerAccountId={employer_account_id}&page={page_number}&ukprn={ukprn}
```

Where:
* period_id = (Optional) period identifier to filter by, i.e. 1617-R02. Default is null / no filter
* employer_account_id = (Optional) employer account identifier to filter by, i.e. 12345. Default is null / no filter
* page_number = (Optional) page number to display, i.e. 10. Default is 1
* ukprn = (Optional) the ukprn to filter by

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
      "ContractType": "ContractWithSfa",
      "EarningDetails": [
                {
                    "RequiredPaymentId": "57999b0e-12ef-44b8-ace4-267baf68c5aa",
                    "StartDate": "2017-08-01T00:00:00",
                    "PlannedEndDate": "2018-08-01T00:00:00",
                    "ActualEndDate": "0001-01-01T00:00:00",
                    "CompletionStatus": 1,
                    "CompletionAmount": 240,
                    "MonthlyInstallment": 80,
                    "TotalInstallments": 12
                }
            ]
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
      "ContractType": "ContractWithSfa",
      "EarningDetails": [
                {
                    "RequiredPaymentId": "57999b0e-12ef-44b8-ace4-267baf68c5aa",
                    "StartDate": "2017-08-01T00:00:00",
                    "PlannedEndDate": "2018-08-01T00:00:00",
                    "ActualEndDate": "0001-01-01T00:00:00",
                    "CompletionStatus": 1,
                    "CompletionAmount": 240,
                    "MonthlyInstallment": 80,
                    "TotalInstallments": 12
                }
            ]
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
      "ContractType": "ContractWithEmployer",
      "EarningDetails": [
                {
                    "RequiredPaymentId": "57999b0e-12ef-44b8-ace4-267baf68c5aa",
                    "StartDate": "2017-08-01T00:00:00",
                    "PlannedEndDate": "2018-08-01T00:00:00",
                    "ActualEndDate": "0001-01-01T00:00:00",
                    "CompletionStatus": 1,
                    "CompletionAmount": 240,
                    "MonthlyInstallment": 80,
                    "TotalInstallments": 12
                }
            ]
    }
  ]
}
```

Response **Items** structure:

| Attribute | Data type | Optional | Description |
| --- | --- | --- | --- |
| Id | string | no | payment unique identifier |
| Ukprn | long | no | learning provider's ukprn |
| Uln | long | no | learner's unique number |
| ApprenticeshipId | long | yes | apprenticeship unique identifier |
| ApprenticeshipVersion | long | yes | apprenticeship version |
| EmployerAccountId | string | yes | employer account unique identifier |
| EmployerAccountVersion | string | yes | employer account version |
| DeliveryPeriod | CalendarPeriod | no | ilr learning period containing calendar month and calcndar year |
| CollectionPeriod | NamedCalendarPeriod | no | ilr collection period containing an id, calendar month and calcndar year |
| EvidenceSubmittedOn | DateTime | no | ilr file submission date and time |
| FundingSource | FundingSource | no | funding source of the payment, see following tables for all possible values |
| TransactionType | TransactionType | no | transaction of the payment, see following tables for all possible values |
| Amount | decimal | no | payment amount |
| StandardCode | long | yes | learning standard code |
| ProgrammeType | int | yes | learning programme type |
| FrameworkCode | int | yes | learning frameworh code |
| PathwayCode | int | yes | learning pathway code |
| ContractType | ContractType | no | apprenticeship contract type, see following tables for all possible values |
| EarningDetails | EarningDetails | no | the earning and ilr information used to generate the payment |

**EarningDetails** properties:
| Property | Description |
| --- | --- |
| RequiredPaymentId | The required payment's id that was used to generate this payment |
| StartDate | The start date (from the ilr)|
| PlannedEndDate | The planned end date (from the ilr)|
| ActualEndDate | The actual end date (from the ilr)|
| CompletionStatus | The completion status (from the ilr)|
| CompletionAmount | The calculated completion amount (from the earnings data)|
| MonthlyInstallment |The calculated monthly installment (from the earnings data)|
| TotalInstallments | The calculated number of installments (from the earnings data)|

**FundingSource** properties:

| Property | Description |
| --- | --- |
| Levy | payment funded by the employer's digital account |
| CoInvestedSfa | payment co-invested by the SFA |
| CoInvestedEmployer | payment co-invested by the employer |
| FullyFundedSfa | payment fully covered by the SFA |

**TransactionType** properties:

| Property | Description |
| --- | --- |
| Learning | on programme payment |
| Completion | completion payment |
| Balancing | balancing payment |
| First16To18EmployerIncentive | first 16-18 employer incentive payment |
| First16To18ProviderIncentive | first 16-18 provider incentive payment |
| Second16To18EmployerIncentive | second 16-18 employer incentive payment |
| Second16To18ProviderIncentive | second 16-18 provider incentive payment |
| OnProgramme16To18FrameworkUplift | 16-18 framework uplift on programme payment |
| Completion16To18FrameworkUplift | 16-18 framework uplift completion payment |
| Balancing16To18FrameworkUplift | 16-18 framework uplift balancing payment |
| FirstDisadvantagePayment | first provider disadvantage payment |
| SecondDisadvantagePayment | second provider disadvantage payment |
| OnProgrammeMathsAndEnglish | maths or english on programme payment |
| BalancingMathsAndEnglish | maths or english balancing payment |
| LearningSupport | learning support payment |

**ContractType** properties:

| Property | Description |
| --- | --- |
| ContractWithEmployer | levy contract |
| ContractWithSfa | non levy contract |

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