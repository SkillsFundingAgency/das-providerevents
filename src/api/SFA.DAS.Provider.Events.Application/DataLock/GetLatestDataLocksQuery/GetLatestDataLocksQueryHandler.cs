using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery
{
    public class GetLatestDataLocksQueryHandler : IAsyncRequestHandler<GetLatestDataLocksQueryRequest, GetLatestDataLocksQueryResponse>
    {
        private readonly IValidator<GetLatestDataLocksQueryRequest> _validator;
        private readonly IDataLockEventRepository _dataLockEventRepository;
        private readonly IMapper _mapper;

        public GetLatestDataLocksQueryHandler(
            IValidator<GetLatestDataLocksQueryRequest> validator, 
            IDataLockEventRepository dataLockEventRepository, IMapper mapper)
        {
            _validator = validator;
            _dataLockEventRepository = dataLockEventRepository;
            _mapper = mapper;
        }

        public async Task<GetLatestDataLocksQueryResponse> Handle(GetLatestDataLocksQueryRequest message)
        {
            try
            {
                var validationResult = await _validator.Validate(message);

                if (!validationResult.IsValid)
                {
                    return new GetLatestDataLocksQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var entities = await _dataLockEventRepository.GetLastDataLocks(message.Ukprn, message.PageNumber, message.PageSize);

                var dataLocks = _mapper.Map<Api.Types.DataLock[]>(entities.Items);

                return new GetLatestDataLocksQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Api.Types.DataLock>
                    {
                        Items = dataLocks,
                        PageNumber = entities.PageNumber,
                        TotalNumberOfPages = entities.TotalNumberOfPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new GetLatestDataLocksQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}