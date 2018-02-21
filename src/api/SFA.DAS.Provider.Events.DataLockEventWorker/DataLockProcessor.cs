using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetHistoricDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun;
using SFA.DAS.Provider.Events.Application.DataLock.UpdateProviderQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private const int PageSize = 10000;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public DataLockProcessor(ILog logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task ProcessDataLocks()
        {
            _logger.Debug("ProcessDataLocks started");

            var providers = await GetProviders().ConfigureAwait(false);

            if (providers == null || providers.Count == 0)
            {
                _logger.Debug("No providers found. Execution complete.");
                return;
            }

            Parallel.ForEach(providers, async provider =>
            {
                int? runId = null;
                try
                {
                    runId = await RecordProcessStart(provider).ConfigureAwait(false);

                    if (provider.RequiresInitialImport)
                        await ProviderInitialImport(provider).ConfigureAwait(false);
                    else
                        await ProcessProvider(provider).ConfigureAwait(false);

                    await RecordProcessEnd(provider.Ukprn, runId.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (runId.HasValue)
                        await RecordProcessEnd(provider.Ukprn, runId.Value, ex.ToString()).ConfigureAwait(false);

                    _logger.Error(ex, $"Error processing data locks for provider {provider.Ukprn}");
                }

            });
        }

        private async Task ProviderInitialImport(ProviderEntity provider)
        {
            var page = 1;

            while (true)
            {
                var query = new GetCurrentDataLocksQueryRequest {Ukprn = provider.Ukprn, PageNumber = page, PageSize = PageSize};
                var response = await _mediator.SendAsync(query).ConfigureAwait(false);

                if (!response.IsValid)
                    throw new ApplicationException($"Failed to get current data locks for provider {provider.Ukprn} initial import", response.Exception);

                if (response.Result == null || response.Result.Items == null || response.Result.Items.Length == 0) 
                    break;

                var writeDataLocks = new WriteDataLocksQueryRequest
                {
                    NewDataLocks = response.Result.Items
                };

                var writeResponse = await _mediator.SendAsync(writeDataLocks).ConfigureAwait(false);

                if (!writeResponse.IsValid)
                    throw new ApplicationException($"Failed to write data locks for provider {provider.Ukprn} initial import", response.Exception);

                if (response.Result.Items.Length < PageSize)
                    break;

                page++;
            }

            page = 1;

            while (true)
            {
                var query = new GetHistoricDataLockEventsQueryRequest {Ukprn = provider.Ukprn, PageNumber = page, PageSize = PageSize};
                var response = await _mediator.SendAsync(query).ConfigureAwait(false);

                if (!response.IsValid)
                    throw new ApplicationException($"Failed to get historic data lock events for provider {provider.Ukprn} initial import", response.Exception);

                if (response.Result == null || response.Result.Items == null || response.Result.Items.Length == 0)
                    break;

                var writeRequest = new WriteDataLockEventsQueryRequest
                {
                    DataLockEvents = response.Result.Items
                };

                var writeResponse = await _mediator.SendAsync(writeRequest).ConfigureAwait(false);
                if (!writeResponse.IsValid)
                    throw new ApplicationException($"Failed to write historic data lock events for provider {provider.Ukprn} initial import", response.Exception);

                if (response.Result.Items.Length < PageSize)
                    break;

                page++;
            }

            provider.RequiresInitialImport = false;
            
            var updateProviderRequest = new UpdateProviderQueryRequest {Provider = provider};
            var updateProviderResponse = await _mediator.SendAsync(updateProviderRequest).ConfigureAwait(false);
            if (!updateProviderResponse.IsValid)
                throw new ApplicationException($"Failed to update provider {provider.Ukprn} submission date", updateProviderResponse.Exception);
        }

        private async Task ProcessProvider(ProviderEntity provider)
        {
            var fetchNextCurrentLock = true;
            var fetchNextLastLock = true;

            DataLock currentLock = null;
            DataLock lastSeenLock = null;

            Queue<DataLock> currentLocks = null;
            Queue<DataLock> lastSeenLocks = null;
            var events = new List<DataLockEvent>();
            var newDataLocks = new List<DataLock>();
            var deletedDataLocks = new List<DataLock>();
            var updatedDataLocks = new List<DataLock>();

            var currentLockPage = 1;
            var currentLocksDone = false;
            var lastLockPage = 1;
            var lastLocksDone = false;

            while (true)
            {
                if (fetchNextCurrentLock)
                {
                    if ((currentLocks == null || currentLocks.Count == 0) && !currentLocksDone)
                    {
                        currentLocks = await GetCurrentDataLocks(provider, currentLockPage).ConfigureAwait(false);
                        if (currentLocks == null || currentLocks.Count == 0)
                        {
                            currentLocksDone = true;
                        }
                        else
                        {
                            currentLockPage++;
                            currentLocksDone = currentLocks.Count < PageSize;
                        }
                    }

                    if (currentLocks != null && currentLocks.Count > 0)
                        currentLock = currentLocks.Dequeue();
                    else
                        currentLock = null;

                    fetchNextCurrentLock = false;
                }

                if (fetchNextLastLock)
                {
                    if ((lastSeenLocks == null || lastSeenLocks.Count == 0) && !lastLocksDone)
                    {
                        lastSeenLocks = await GetLastDataLocks(provider, lastLockPage).ConfigureAwait(false);
                        if (lastSeenLocks == null || lastSeenLocks.Count == 0)
                        {
                            lastLocksDone = true;
                        }
                        else
                        {
                            lastLockPage++;
                            lastLocksDone = lastSeenLocks.Count < PageSize;
                        }
                    }

                    if (lastSeenLocks != null && lastSeenLocks.Count > 0)
                        lastSeenLock = lastSeenLocks.Dequeue();
                    else
                        lastSeenLock = null;

                    fetchNextLastLock = false;
                }

                if (currentLock == null && lastSeenLock == null)
                    break;

                var compare = Compare(currentLock, lastSeenLock);

                if (compare == 0)
                {
                    // same lock
                    if (!AreEqual(currentLock, lastSeenLock))
                    {
                        events.Add(FromDataLock(currentLock, EventStatus.Updated, provider));
                        updatedDataLocks.Add(currentLock);
                    }

                    fetchNextCurrentLock = true;
                    fetchNextLastLock = true;
                }
                else
                {
                    if (compare < 0)
                    {
                        // new data lock
                        newDataLocks.Add(currentLock);
                        events.Add(FromDataLock(currentLock, EventStatus.New, provider));
                        fetchNextCurrentLock = true;
                    }
                    else
                    {
                        // removed data lock
                        deletedDataLocks.Add(lastSeenLock);
                        events.Add(FromDataLock(lastSeenLock, EventStatus.Removed, provider));
                        fetchNextLastLock = true;
                    }
                }


                if (newDataLocks.Count + updatedDataLocks.Count + deletedDataLocks.Count > PageSize)
                    await WriteDataLocks(newDataLocks, updatedDataLocks, deletedDataLocks, provider);

                if (events.Count > PageSize)
                    await WriteDataLockEvents(events, provider);
            }

            if (newDataLocks.Count + updatedDataLocks.Count + deletedDataLocks.Count > 0)
                await WriteDataLocks(newDataLocks, updatedDataLocks, deletedDataLocks, provider);

            if (events.Count > 0)
                await WriteDataLockEvents(events, provider);

            var updateProviderRequest = new UpdateProviderQueryRequest {Provider = provider};
            var updateProviderResponse = await _mediator.SendAsync(updateProviderRequest).ConfigureAwait(false);
            if (!updateProviderResponse.IsValid)
                throw new ApplicationException($"Failed to update provider {provider.Ukprn} submission date", updateProviderResponse.Exception);
        }

        private async Task WriteDataLockEvents(List<DataLockEvent> events, ProviderEntity provider)
        {
            var writeDataLockEvents = new WriteDataLockEventsQueryRequest {DataLockEvents = events};

            var dataLockEventsResponse = await _mediator.SendAsync(writeDataLockEvents).ConfigureAwait(false);
            if (!dataLockEventsResponse.IsValid)
                throw new ApplicationException($"Failed to save new data lock events for provider {provider.Ukprn}", dataLockEventsResponse.Exception);

            events.Clear();
        }

        private async Task WriteDataLocks(List<DataLock> newDataLocks, List<DataLock> updatedDataLocks, List<DataLock> deletedDataLocks, ProviderEntity provider)
        {
            var writeDataLocks = new WriteDataLocksQueryRequest
            {
                NewDataLocks = newDataLocks,
                UpdatedDataLocks = updatedDataLocks,
                RemovedDataLocks = deletedDataLocks
            };

            var response = await _mediator.SendAsync(writeDataLocks).ConfigureAwait(false);
            if (!response.IsValid)
                throw new ApplicationException($"Failed to update data locks for provider {provider.Ukprn}", response.Exception);

            newDataLocks.Clear();
            updatedDataLocks.Clear();
            deletedDataLocks.Clear();
        }

        private static DataLockEvent FromDataLock(DataLock current, EventStatus status, ProviderEntity provider)
        {
            if (!current.AimSequenceNumber.HasValue)
                throw new ApplicationException("DataLock has no AimSequenceNumber, cannot create data lock event. Ukprn:");

            return new DataLockEvent
            {
                Ukprn = current.Ukprn,
                PriceEpisodeIdentifier = current.PriceEpisodeIdentifier,
                AimSeqNumber = current.AimSequenceNumber.Value,
                LearnRefNumber = current.LearnerReferenceNumber,
                Status = status,
                Uln = current.Uln,
                ApprenticeshipId = current.CommitmentId,
                EmployerAccountId = current.EmployerAccountId,
                ProcessDateTime = DateTime.UtcNow,
                IlrFileName = provider.IlrFileName,
                HasErrors = current.ErrorCodes != null && current.ErrorCodes.Count > 0,

                IlrEndpointAssessorPrice = current.IlrEndpointAssessorPrice,
                IlrFrameworkCode = current.IlrFrameworkCode,
                IlrPathwayCode = current.IlrPathwayCode,
                IlrPriceEffectiveFromDate = current.IlrPriceEffectiveFromDate,
                IlrProgrammeType = current.IlrProgrammeType,
                IlrStandardCode = current.IlrStandardCode,
                IlrPriceEffectiveToDate = current.IlrPriceEffectiveToDate,
                IlrStartDate = current.IlrStartDate,
                IlrTrainingPrice = current.IlrTrainingPrice,

                Errors = current.ErrorCodes == null ? null : current.ErrorCodes.Select(c => new DataLockEventError { ErrorCode = c}).ToArray()
            };
        }

        private static bool AreEqual(DataLock x, DataLock y)
        {
            if ((x.ErrorCodes == null) != (y.ErrorCodes == null))
                return false;

            if (x.ErrorCodes != null && !x.ErrorCodes.SequenceEqual(y.ErrorCodes))
                return false;

            if ((x.CommitmentVersions == null) != (y.CommitmentVersions == null))
                return false;

            if (x.CommitmentVersions != null && !x.CommitmentVersions.SequenceEqual(y.CommitmentVersions))
                return false;

            return true;
        }

        private static int Compare(DataLock x, DataLock y)
        {
            if (x == null && y == null) return 0;

            if (x == null) return 1;

            if (y == null) return -1;

            if (x.Ukprn < y.Ukprn) return -1;
            if (x.Ukprn > y.Ukprn) return 1;

            var learnerRefCompare = string.Compare(x.LearnerReferenceNumber, y.LearnerReferenceNumber, StringComparison.Ordinal);
            if (learnerRefCompare != 0) return learnerRefCompare;

            var priceEpisodeCompare = string.Compare(x.PriceEpisodeIdentifier, y.PriceEpisodeIdentifier, StringComparison.Ordinal);
            if (priceEpisodeCompare != 0) return priceEpisodeCompare;

            if (x.AimSequenceNumber.GetValueOrDefault(0) < y.AimSequenceNumber.GetValueOrDefault(0)) return -1;
            if (x.AimSequenceNumber.GetValueOrDefault(0) > y.AimSequenceNumber.GetValueOrDefault(0)) return 1;

            return 0;
        }

        private async Task<Queue<DataLock>> GetLastDataLocks(ProviderEntity provider, int lastLockPage)
        {
            var query = new GetLatestDataLocksQueryRequest {Ukprn = provider.Ukprn, PageNumber = lastLockPage, PageSize = PageSize};
            var response = await _mediator.SendAsync(query).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException($"Failed to get last data locks for provider {provider.Ukprn}", response.Exception);

            return response.Result?.Items == null ? new Queue<DataLock>() : new Queue<DataLock>(response.Result.Items);
        }

        private async Task<Queue<DataLock>> GetCurrentDataLocks(ProviderEntity provider, int pageNumber)
        {
            var query = new GetCurrentDataLocksQueryRequest {Ukprn = provider.Ukprn, PageNumber = pageNumber, PageSize = PageSize};
            var response = await _mediator.SendAsync(query).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException($"Failed to get current data locks for provider {provider.Ukprn}", response.Exception);

            var dataLocks = response.Result?.Items;
            if (dataLocks == null) return new Queue<DataLock>();
            return new Queue<DataLock>(dataLocks);
        }

        private async Task<IList<ProviderEntity>> GetProviders()
        {
            var query = new GetProvidersQueryRequest {UpdatedOnly = true};
            var response = await _mediator.SendAsync(query).ConfigureAwait(false);
            if (!response.IsValid)
                throw new ApplicationException("Failed to get provider list", response.Exception);
            return response.Result;
        }

        private async Task<int> RecordProcessStart(ProviderEntity provider)
        {
            var request = new RecordProcessorRunRequest
            {
                Ukprn = provider.Ukprn,
                StartTimeUtc = DateTime.UtcNow,
                IlrSubmissionDateTime = provider.IlrSubmissionDateTime,
                IsInitialRun = provider.RequiresInitialImport
            };

            var response = await _mediator.SendAsync(request).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException("Failed to record process start", response.Exception);
            return response.RunId;
        }

        private async Task RecordProcessEnd(long ukprn, int runId, string error = null)
        {
            var request = new RecordProcessorRunRequest
            {
                Ukprn = ukprn,
                RunId = runId,
                FinishTimeUtc = DateTime.UtcNow,
                IsSuccess = error == null,
                Error = error
            };

            var response = await _mediator.SendAsync(request).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException("Failed to record process end", response.Exception);
        }
    }
}
