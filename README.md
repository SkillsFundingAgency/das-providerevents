# DAS Provider Events

Repository to capture and surface significant events create by providers as part of the data collections process

## Events Api

The events api is where the events can be consumed from. You can get the .NET client from nuget:

```powershell
Install-Package SFA.DAS.Provider.Events.Api.Client
```

The api has a number of streams:

* [Period ends](src/api/README.md) - notification that a period end has been completed.
* [Payments](src/api/README.md) - notification of payments that have been processed.
* Submissions - notification that material information about learning has changed.