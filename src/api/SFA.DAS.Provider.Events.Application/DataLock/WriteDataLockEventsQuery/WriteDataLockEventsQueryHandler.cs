using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery
{
    public class WriteDataLockEventsQueryHandler : IAsyncRequestHandler<WriteDataLockEventsQueryRequest, WriteDataLockEventsQueryResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IMapper _mapper;

        public WriteDataLockEventsQueryHandler(IDataLockEventRepository dataLockEventRepository, IMapper mapper)
        {
            _dataLockEventRepository = dataLockEventRepository;
            _mapper = mapper;
        }

        public async Task<WriteDataLockEventsQueryResponse> Handle(WriteDataLockEventsQueryRequest message)
        {
            try
            {
                if (message.DataLockEvents != null && message.DataLockEvents.Count > 0)
                    await _dataLockEventRepository.WriteDataLockEvents(_mapper.Map<IList<DataLockEventEntity>>(message.DataLockEvents));

                return new WriteDataLockEventsQueryResponse
                {
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                return new WriteDataLockEventsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}