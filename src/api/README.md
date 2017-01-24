# DAS Provider Events API

REST api for surfacing information about payments made to providers

## Functionality

### Get period ends

Period ends are a slow moving feed of items signifying a successful completion of a period end.

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

### Get payments

Payments is a feed of low level payments made by the system. They can optionally be filtered.

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

## Security
All API endpoints are protected by JWT tokens.

Each request must include the header:
```
Authorization:Bearer {your_token}
```

The only supported scope is:

    ReadPayments

Note that tokens can viewed using an online tool such as the one at [http://jwt.io](http://jwt.io)
