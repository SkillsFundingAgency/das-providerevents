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
        private readonly IDataLockRepository _dataLockEventsRepository;
        private readonly IMapper _mapper;

        public GetDataLockEventsQueryHandler(
            IValidator<GetDataLockEventsQueryRequest> validator, 
            IDataLockRepository dataLockEventsRepository, 
            IMapper mapper)
        {
            _validator = validator;
            _dataLockEventsRepository = dataLockEventsRepository;
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
                            _dataLockEventsRepository.GetDataLockEventsForAccountAndProviderSinceTime(
                                message.EmployerAccountId, message.Ukprn, message.SinceTime.Value, message.PageNumber,
                                message.PageSize)
                        : await
                            _dataLockEventsRepository.GetDataLockEventsForAccountAndProviderSinceId(
                                message.EmployerAccountId, message.Ukprn, message.SinceEventId, message.PageNumber,
                                message.PageSize);
                }
                else if (message.Ukprn > 0)
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await
                            _dataLockEventsRepository.GetDataLockEventsForProviderSinceTime(message.Ukprn,
                                message.SinceTime.Value, message.PageNumber, message.PageSize)
                        : await
                            _dataLockEventsRepository.GetDataLockEventsForProviderSinceId(message.Ukprn,
                                message.SinceEventId, message.PageNumber, message.PageSize);
                }
                else if (!string.IsNullOrEmpty(message.EmployerAccountId))
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await
                            _dataLockEventsRepository.GetDataLockEventsForAccountSinceTime(message.EmployerAccountId,
                                message.SinceTime.Value,
                                message.PageNumber, message.PageSize)
                        : await
                            _dataLockEventsRepository.GetDataLockEventsForAccountSinceId(message.EmployerAccountId,
                                message.SinceEventId,
                                message.PageNumber, message.PageSize);
                }
                else if (message.SinceTime.HasValue)
                {
                    pageOfEntities = await _dataLockEventsRepository.GetDataLockEventsSinceTime(message.SinceTime.Value, message.PageNumber, message.PageSize);
                }
                else
                {
                    pageOfEntities = await _dataLockEventsRepository.GetDataLockEventsSinceId(message.SinceEventId, message.PageNumber, message.PageSize);
                }

                var eventIds = pageOfEntities.Items.Select(x => x.DataLockEventId.ToString()).Distinct().ToArray();

                var errors = await _dataLockEventsRepository.GetDataLockErrorsForEvents(eventIds);
                var periods = await _dataLockEventsRepository.GetDataLockPeriodsForEvent(eventIds);
                var apprenticeships = await _dataLockEventsRepository.GetDataLockApprenticeshipsForEvent(eventIds);
                foreach (var entity in pageOfEntities.Items)
                {
                    entity.Errors = errors.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                    entity.Periods = periods.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                    entity.Apprenticeships = apprenticeships.Where(x => x.DataLockEventId == entity.DataLockEventId).ToArray();
                }

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