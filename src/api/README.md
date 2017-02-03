# DAS Provider Events API

REST api for surfacing information about payments made to providers

## Functionality

The API has 3 endpoints [PeriodEnds](PeriodEnds_README.md), [Payments](Payments_README.md) and [Submissions](Submissions_README.md).

Period ends is a slow moving feed to notify consumers when a period end has occured. Payments gives details of all payments and can optionally be filtered by period and/or employer. As payments can only occur as part of a period end, is is advisable that consumers poll the period end endpoint to see if there will be any new payments. Then query the payments endpoint filtered by at least period to minimise the amount of traffic and processing that needs to occur.

Submissions is a feed of changes that have happened to the learning record.

## Security
All API endpoints are protected by JWT tokens.

Each request must include the header:
```
Authorization:Bearer {your_token}
```

The token needs to include one or more of the following scopes:

    ReadPayments - To read period ends or payments
    ReadSubmissions - To read submissions

Note that tokens can viewed using an online tool such as the one at [http://jwt.io](http://jwt.io)
