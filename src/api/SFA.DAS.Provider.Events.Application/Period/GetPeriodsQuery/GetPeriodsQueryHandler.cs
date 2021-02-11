using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.Period.GetPeriodsQuery
{
    public class GetPeriodsQueryHandler : IAsyncRequestHandler<GetPeriodsQueryRequest, GetPeriodsQueryResponse>
    {
        private readonly IPeriodRepository _periodRepository;
        private readonly IMapper _mapper;

        public GetPeriodsQueryHandler(IPeriodRepository periodRepository, IMapper mapper)
        {
            _periodRepository = periodRepository;
            _mapper = mapper;
        }

        public async Task<GetPeriodsQueryResponse> Handle(GetPeriodsQueryRequest message)
        {
            try
            {
                var periodEntities = await _periodRepository.GetPeriods().ConfigureAwait(false);
                return new GetPeriodsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<Data.CollectionPeriod[]>(periodEntities)
                };
            }
            catch (Exception ex)
            {
                return new GetPeriodsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}