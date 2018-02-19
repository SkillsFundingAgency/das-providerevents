using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery
{
    public class GetCurrentDataLocksQueryHandler : IAsyncRequestHandler<GetCurrentDataLocksQueryRequest, GetCurrentDataLocksQueryResponse>
    {
        private readonly IValidator<GetCurrentDataLocksQueryRequest> _validator;
        private readonly IDataLockRepository _dataLockRepository;
        private readonly IMapper _mapper;

        public GetCurrentDataLocksQueryHandler(
            IValidator<GetCurrentDataLocksQueryRequest> validator, 
            IDataLockRepository dataLockRepository, IMapper mapper)
        {
            _validator = validator;
            _dataLockRepository = dataLockRepository;
            _mapper = mapper;
        }

        public async Task<GetCurrentDataLocksQueryResponse> Handle(GetCurrentDataLocksQueryRequest message)
        {
            try
            {
                var validationResult = await _validator.Validate(message);

                if (!validationResult.IsValid)
                {
                    return new GetCurrentDataLocksQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var dataLockEvents = await _dataLockRepository.GetDataLocks(message.Ukprn, message.PageNumber, message.PageSize);

                return new GetCurrentDataLocksQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Api.Types.DataLock>
                    {
                        Items = _mapper.Map<Api.Types.DataLock[]>(dataLockEvents),
                        PageNumber = message.PageNumber,
                        TotalNumberOfPages = -1
                    }
                };
            }
            catch (Exception ex)
            {
                return new GetCurrentDataLocksQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}