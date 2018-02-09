using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.UpdateProviderQuery
{
    public class UpdateProviderQueryHandler : IAsyncRequestHandler<UpdateProviderQueryRequest, UpdateProviderQueryResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;

        public UpdateProviderQueryHandler(IDataLockEventRepository dataLockEventRepository)
        {
            _dataLockEventRepository = dataLockEventRepository;
        }

        public async Task<UpdateProviderQueryResponse> Handle(UpdateProviderQueryRequest message)
        {
            try
            {
                if (message.Provider != null)
                    await _dataLockEventRepository.UpdateProvider(message.Provider).ConfigureAwait(false);

                return new UpdateProviderQueryResponse { IsValid = true };
            }
            catch (Exception ex)
            {
                return new UpdateProviderQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}