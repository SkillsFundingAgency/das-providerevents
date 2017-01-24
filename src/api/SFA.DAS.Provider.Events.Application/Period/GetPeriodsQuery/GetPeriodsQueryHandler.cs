﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Mapping;

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
                var periodEntities = await _periodRepository.GetPeriods();
                return new GetPeriodsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<Domain.Period[]>(periodEntities)
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