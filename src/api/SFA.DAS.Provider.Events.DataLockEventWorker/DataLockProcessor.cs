using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery;
using SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public DataLockProcessor(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task ProcessDataLocks()
        {
            _logger.Debug("ProcessDataLocks started");

            var providers = await GetProviders();

            if (providers == null || providers.Count == 0)
            {
                _logger.Debug("No providers found. Execution complete.");
                return;
            }

            Parallel.ForEach(providers, async provider =>
            {
                try
                {
                    var fetchNextCurrentLock = true;
                    var fetchNextLastLock = true;

                    DataLock currentLock = null;
                    DataLock lastLock = null;

                    Queue<DataLock> currentDataLocks = null;
                    Queue<DataLock> lastLocks = null;
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
                            if ((currentDataLocks == null || currentDataLocks.Count == 0) && !currentLocksDone)
                            {
                                currentDataLocks = await GetCurrentDataLocks(provider, currentLockPage).ConfigureAwait(false);
                                if (currentDataLocks?.Count == 0)
                                    currentLocksDone = true;
                                else
                                    currentLockPage++;
                            }

                            if (currentDataLocks != null && currentDataLocks.Count > 0)
                                currentLock = currentDataLocks.Dequeue();
                            else
                                currentLock = null;

                            fetchNextCurrentLock = false;
                        }

                        if (fetchNextLastLock)
                        {
                            if ((lastLocks == null || lastLocks.Count == 0) && !lastLocksDone)
                            {
                                lastLocks = await GetLastDataLocks(provider, lastLockPage).ConfigureAwait(false);
                                if (lastLocks?.Count == 0)
                                    lastLocksDone = true;
                                else
                                    lastLockPage++;
                            }

                            if (lastLocks != null && lastLocks.Count > 0)
                                lastLock = lastLocks.Dequeue();
                            else
                                lastLock = null;

                            fetchNextLastLock = false;
                        }

                        if (currentLock == null && lastLock == null)
                            break;

                        var compare = Compare(currentLock, lastLock);

                        if (compare == 0)
                        {
                            // same lock
                            if (!AreEqual(currentLock, lastLock))
                            {
                                events.Add(FromDataLock(currentLock, EventStatus.Updated));
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
                                events.Add(FromDataLock(currentLock, EventStatus.New));
                                fetchNextCurrentLock = true;
                            }
                            else
                            {
                                // removed data lock
                                deletedDataLocks.Add(lastLock);
                                events.Add(FromDataLock(lastLock, EventStatus.Removed));
                                fetchNextLastLock = true;
                            }
                        }
                    }

                    var writeDataLocks = new WriteDataLocksQueryRequest
                    {
                        NewDataLocks = newDataLocks, 
                        UpdatedDataLocks = updatedDataLocks, 
                        RemovedDataLocks = deletedDataLocks
                    };

                    var response = await _mediator.SendAsync(writeDataLocks).ConfigureAwait(false);
                    if (!response.IsValid)
                        throw new ApplicationException($"Failed to update data locks for provider {provider.Ukprn}", response.Exception);


                    var writeDataLockEvents = new WriteDataLockEventsQueryRequest { DataLockEvents = events };

                    var dataLockEventsResponse = await _mediator.SendAsync(writeDataLockEvents).ConfigureAwait(false);
                    if (!dataLockEventsResponse.IsValid)
                        throw new ApplicationException($"Failed to save new data lock events for provider {provider.Ukprn}", dataLockEventsResponse.Exception);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error processing data locks for provider {provider.Ukprn}");
                }
            });
        }

        private static DataLockEvent FromDataLock(DataLock current, EventStatus status)
        {
            if (!current.AimSequenceNumber.HasValue)
                throw new ApplicationException("DataLock has no AimSequenceNumber, cannot create data lock event. Ukprn:");

            return new DataLockEvent
            {
                Ukprn = current.Ukprn,
                PriceEpisodeIdentifier = current.PriceEpisodeIdentifier,
                AimSeqNumber = current.AimSequenceNumber.Value,                
                LearnRefNumber = current.LearnerReferenceNumber,
                Status = status
            };
        }

        private static bool AreSame(DataLock x, DataLock y)
        {
            return x.Ukprn == y.Ukprn && Compare(x, y) == 0;
        }

        private static bool AreEqual(DataLock x, DataLock y)
        {
            if (!AreSame(x, y))
                return false;

            if ((x.ErrorCodes == null) != (y.ErrorCodes == null))
                return false;

            if (x.ErrorCodes != null && !x.ErrorCodes.SequenceEqual(y.ErrorCodes))
                return false;

            if ((x.Commitments == null) != (y.Commitments == null))
                return false;

            if (x.Commitments != null && !x.Commitments.SequenceEqual(y.Commitments))
                return false;

            return true;
        }

        private static int Compare(DataLock x, DataLock y)
        {
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
            var query = new GetLatestDataLocksQueryRequest {Ukprn = provider.Ukprn, PageNumber = lastLockPage, PageSize = 10000};
            var response = await _mediator.SendAsync(query).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException($"Failed to get last data locks for provider {provider.Ukprn}", response.Exception);

            return response.Result.Items == null ? new Queue<DataLock>() : new Queue<DataLock>(response.Result.Items);
        }

        private async Task<Queue<DataLock>> GetCurrentDataLocks(ProviderEntity provider, int pageNumber)
        {
            var query = new GetCurrentDataLocksQueryRequest {Ukprn = provider.Ukprn, PageNumber = pageNumber, PageSize = 10000};
            var response = await _mediator.SendAsync(query).ConfigureAwait(false);

            if (!response.IsValid)
                throw new ApplicationException($"Failed to get current data locks for provider {provider.Ukprn}", response.Exception);

            var dataLocks = response.Result.Items;
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
    }
}
