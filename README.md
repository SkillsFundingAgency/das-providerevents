# DAS Provider Events

Repository to capture and surface significant events create by providers as part of the data collections process

## Events Api

The events api is where the events can be consumed from. You can get the .NET client from nuget:

```powershell
Install-Package SFA.DAS.Provider.Events.Api.Client
```

The api has a number of streams:

* [Period ends](src/api/PeriodEnds_README.md) - notification that a period end has been completed.
* [Payments](src/api/Payments_README.md) - notification of payments that have been processed.
* [Submissions](src/api/Submissions_README.md) - notification that material information about learning has changed.
* [Data lock events](src/api/DataLock_README.md) - notification that data lock status has changed.

More information about the API can be viewed in the [README](src/api/README.md).