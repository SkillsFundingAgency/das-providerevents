using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery
{
    public class GetDataLockEventsQueryHandler : 
        IAsyncRequestHandler<GetDataLockEventsQueryRequest, GetDataLockEventsQueryResponse>
    {
        private readonly IValidator<GetDataLockEventsQueryRequest> _validator;
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IMapper _mapper;

        public GetDataLockEventsQueryHandler(
            IValidator<GetDataLockEventsQueryRequest> validator, 
            IDataLockEventRepository dataLockEventRepository, 
            IMapper mapper)
        {
            _validator = validator;
            _dataLockEventRepository = dataLockEventRepository;
            _mapper = mapper;
        }

        public async Task<GetDataLockEventsQueryResponse> Handle(GetDataLockEventsQueryRequest message)
        {
            try
            {
                var validationResult = await _validator.Validate(message);

                if (!validationResult.IsValid)
                {
                    return new GetDataLockEventsQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var pageOfEntities = await _dataLockEventRepository.GetDataLockEvents(
                    message.SinceEventId == 0 ? null : message.SinceEventId, 
                    message.SinceTime, 
                    message.EmployerAccountId, 
                    message.Ukprn == 0 ? null : message.Ukprn, 
                    message.PageNumber, 
                    message.PageSize
                ).ConfigureAwait(false);

                return new GetDataLockEventsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<PageOfResults<DataLockEvent>>(pageOfEntities)
                };
            }
            catch (Exception ex)
            {
                return new GetDataLockEventsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}