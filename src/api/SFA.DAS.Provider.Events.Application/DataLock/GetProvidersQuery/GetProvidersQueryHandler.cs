using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery
{
    public class GetProvidersQueryHandler : IAsyncRequestHandler<GetProvidersQueryRequest, GetProvidersQueryResponse>
    {
        private readonly IDataLockRepository _dataLockRepository;
        private readonly IDataLockEventRepository _dataLockEventRepository;

        public GetProvidersQueryHandler(
            IDataLockRepository dataLockRepository, 
            IDataLockEventRepository dataLockEventRepository)
        {
            _dataLockRepository = dataLockRepository;
            _dataLockEventRepository = dataLockEventRepository;
        }

        public async Task<GetProvidersQueryResponse> Handle(GetProvidersQueryRequest message)
        {
            try
            {
                var allProviders = await _dataLockRepository.GetProviders();

                if (!message.UpdatedOnly)
                {
                    return new GetProvidersQueryResponse
                    {
                        IsValid = true,
                        Result = allProviders
                    };
                }

                var processedProviders = (await _dataLockEventRepository.GetProviders()).ToDictionary(p => p.Ukprn, p => p.IlrSubmissionDateTime);

                for (var i = allProviders.Count - 1; i >= 0; i--)
                {
                    var provider = allProviders[i];
                    if (processedProviders.TryGetValue(provider.Ukprn, out var processTime) && processTime >= provider.IlrSubmissionDateTime) 
                        allProviders.RemoveAt(i);
                }

                return new GetProvidersQueryResponse
                {
                    IsValid = true,
                    Result = allProviders
                };
            }
            catch (Exception ex)
            {
                return new GetProvidersQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}