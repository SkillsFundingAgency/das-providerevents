using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
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

                PageOfResults<DataLockEventEntity> pageOfEntities;


                if (message.Ukprn > 0 && !string.IsNullOrEmpty(message.EmployerAccountId))
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await
                            _dataLockEventRepository.GetDataLockEventsForAccountAndProviderSinceTime(
                                message.EmployerAccountId, message.Ukprn, message.SinceTime.Value, message.PageNumber,
                                message.PageSize)
                        : await
                            _dataLockEventRepository.GetDataLockEventsForAccountAndProviderSinceId(
                                message.EmployerAccountId, message.Ukprn, message.SinceEventId, message.PageNumber,
                                message.PageSize);
                }
                else if (message.Ukprn > 0)
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await
                            _dataLockEventRepository.GetDataLockEventsForProviderSinceTime(message.Ukprn,
                                message.SinceTime.Value, message.PageNumber, message.PageSize)
                        : await
                            _dataLockEventRepository.GetDataLockEventsForProviderSinceId(message.Ukprn,
                                message.SinceEventId, message.PageNumber, message.PageSize);
                }
                else if (!string.IsNullOrEmpty(message.EmployerAccountId))
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await
                            _dataLockEventRepository.GetDataLockEventsForAccountSinceTime(message.EmployerAccountId,
                                message.SinceTime.Value,
                                message.PageNumber, message.PageSize)
                        : await
                            _dataLockEventRepository.GetDataLockEventsForAccountSinceId(message.EmployerAccountId,
                                message.SinceEventId,
                                message.PageNumber, message.PageSize);
                }
                else if (message.SinceTime.HasValue)
                {
                    pageOfEntities = await _dataLockEventRepository.GetDataLockEventsSinceTime(message.SinceTime.Value, message.PageNumber, message.PageSize);
                }
                else
                {
                    pageOfEntities = await _dataLockEventRepository.GetDataLockEventsSinceId(message.SinceEventId, message.PageNumber, message.PageSize);
                }

                var eventIds = pageOfEntities.Items.Select(x => x.DataLockEventId.ToString()).Distinct().ToArray();

                var errors = await _dataLockEventRepository.GetDataLockErrorsForEvents(eventIds);
                //var periods = await _dataLockEventRepository.GetDataLockPeriodsForEvent(eventIds);
                var apprenticeships = await _dataLockEventRepository.GetDataLockApprenticeshipsForEvent(eventIds);
                foreach (var entity in pageOfEntities.Items)
                {
                    entity.Errors = errors.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                    //entity.Periods = periods.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                    entity.Apprenticeships = apprenticeships.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                }

                return new GetDataLockEventsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<Api.Types.PageOfResults<DataLockEvent>>(pageOfEntities)
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