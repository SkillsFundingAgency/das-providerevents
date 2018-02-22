using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetHistoricDataLockEventsQuery
{
    public class GetHistoricDataLockEventsQueryHandler : IAsyncRequestHandler<GetHistoricDataLockEventsQueryRequest, GetHistoricDataLockEventsQueryResponse>
    {
        private readonly IDataLockRepository _dataLockRepository;
        private readonly IMapper _mapper;

        public GetHistoricDataLockEventsQueryHandler(
            IDataLockRepository dataLockRepository, 
            IMapper mapper)
        {
            _dataLockRepository = dataLockRepository;
            _mapper = mapper;
        }

        public async Task<GetHistoricDataLockEventsQueryResponse> Handle(GetHistoricDataLockEventsQueryRequest message)
        {
            try
            {
                var pageOfEntities = await _dataLockRepository.GetDataLockEvents(
                    message.Ukprn,
                    message.PageNumber, 
                    message.PageSize
                ).ConfigureAwait(false);

                return new GetHistoricDataLockEventsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<PageOfResults<DataLockEvent>>(pageOfEntities)
                };
            }
            catch (Exception ex)
            {
                return new GetHistoricDataLockEventsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}