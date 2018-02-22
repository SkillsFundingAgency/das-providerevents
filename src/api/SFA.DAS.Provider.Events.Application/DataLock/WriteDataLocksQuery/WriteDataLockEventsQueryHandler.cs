using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery
{
    public class WriteDataLocksQueryHandler : 
        IAsyncRequestHandler<WriteDataLocksQueryRequest, WriteDataLocksQueryResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IMapper _mapper;

        public WriteDataLocksQueryHandler(IDataLockEventRepository dataLockEventRepository, IMapper mapper)
        {
            _dataLockEventRepository = dataLockEventRepository;
            _mapper = mapper;
        }

        public async Task<WriteDataLocksQueryResponse> Handle(WriteDataLocksQueryRequest message)
        {
            try
            {
                IList<DataLockEntity> entities = null;

                if (message.NewDataLocks != null)
                    entities = _mapper.Map<IList<DataLockEntity>>(message.NewDataLocks);

                if (entities != null && entities.Any())
                    await _dataLockEventRepository.WriteDataLocks(entities);

                entities = new List<DataLockEntity>();

                if (message.UpdatedDataLocks != null)
                    entities = _mapper.Map<IList<DataLockEntity>>(message.UpdatedDataLocks);

                if (message.RemovedDataLocks != null)
                    entities = entities.Concat(_mapper.Map<IList<DataLockEntity>>(message.RemovedDataLocks).Select(e =>
                    {
                        e.DeletedUtc = DateTime.UtcNow;
                        return e;
                    })).ToList();

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