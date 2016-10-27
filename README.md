# DAS Payments Events API

REST api for surfacing information about payments made on behalf of employer accounts

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
      "Id": "7F341453-F4D7-455F-8B8A-D7FC81F5D121",
      "Ukprn": 123456,
      "Uln": 987654,
      "EmployerAccountId": "123",
      "ApprenticeshipId": 1,
      "DeliveryPeriod": {
        "Month": 8,
        "Year": 2017
      },
      "CollectionPeriod": {
        "Id": "1617-R12",
        "Month": 8,
        "Year": 2017
      },
      "EvidenceSubmittedOn": "2017-07-01T00:00:00",
      "EmployerAccountVersion": "20170315",
      "ApprenticeshipVersion": "1",
      "FundingSource": "Levy",
      "TransactionType": "Learning",
      "Amount": 1234
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
