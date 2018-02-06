using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery
{
    public class GetCurrentDataLocksQueryHandler : IAsyncRequestHandler<GetCurrentDataLocksQueryRequest, GetCurrentDataLocksQueryResponse>
    {
        private readonly IValidator<GetCurrentDataLocksQueryRequest> _validator;
        private readonly IDataLockRepository _dataLockRepository;

        public GetCurrentDataLocksQueryHandler(
            IValidator<GetCurrentDataLocksQueryRequest> validator, 
            IDataLockRepository dataLockRepository)
        {
            _validator = validator;
            _dataLockRepository = dataLockRepository;
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

                var dataLocks = await _dataLockRepository.GetDataLocks(message.Ukprn, message.PageNumber, message.PageSize);

                return new GetCurrentDataLocksQueryResponse
                {
                    IsValid = true,
                    Result = dataLocks
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