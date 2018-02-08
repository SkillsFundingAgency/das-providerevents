using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery
{
    public class WriteDataLocksQueryHandler : 
        IAsyncRequestHandler<WriteDataLocksQueryRequest, WriteDataLocksQueryResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;

        public WriteDataLocksQueryHandler(IDataLockEventRepository dataLockEventRepository)
        {
            _dataLockEventRepository = dataLockEventRepository;
        }

        private string GetString<T>(IList<T> array)
        {
            if (array == null || array.Count == 0)
                return null;

            return JsonConvert.SerializeObject(array);
        }

        public async Task<WriteDataLocksQueryResponse> Handle(WriteDataLocksQueryRequest message)
        {
            try
            {
                List<DataLockEntity> entities = null;

                if (message.NewDataLocks != null)
                    entities = message.NewDataLocks.Select(d => new DataLockEntity
                    {
                        Ukprn = d.Ukprn,
                        AimSequenceNumber = d.AimSequenceNumber,
                        LearnerReferenceNumber = d.LearnerReferenceNumber,
                        PriceEpisodeIdentifier = d.PriceEpisodeIdentifier,
                        ErrorCodes = GetString(d.ErrorCodes),
                        Commitments = GetString(d.Commitments)
                    }).ToList();

                if (entities != null && entities.Any())
                    await _dataLockEventRepository.WriteDataLocks(entities);

                entities = new List<DataLockEntity>();

                if (message.UpdatedDataLocks != null)
                    entities = message.UpdatedDataLocks.Select(d => new DataLockEntity
                    {
                        Ukprn = d.Ukprn,
                        AimSequenceNumber = d.AimSequenceNumber,
                        LearnerReferenceNumber = d.LearnerReferenceNumber,
                        PriceEpisodeIdentifier = d.PriceEpisodeIdentifier,
                        ErrorCodes = GetString(d.ErrorCodes),
                        Commitments = GetString(d.Commitments),

                    }).ToList();

                if (message.RemovedDataLocks != null)
                    entities.AddRange(message.RemovedDataLocks.Select(d => new DataLockEntity
                        {
                            Ukprn = d.Ukprn,
                            AimSequenceNumber = d.AimSequenceNumber,
                            LearnerReferenceNumber = d.LearnerReferenceNumber,
                            PriceEpisodeIdentifier = d.PriceEpisodeIdentifier,
                            ErrorCodes = GetString(d.ErrorCodes),
                            Commitments = GetString(d.Commitments),
                            DeletedUtc = DateTime.UtcNow
                        })
                        .ToList());

                if (entities.Any())
                    await _dataLockEventRepository.UpdateDataLocks(entities);

                return new WriteDataLocksQueryResponse
                {
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                return new WriteDataLocksQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}