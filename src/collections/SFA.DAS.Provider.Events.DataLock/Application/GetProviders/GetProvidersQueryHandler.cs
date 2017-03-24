using System;
using System.Linq;
using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetProviders
{
    public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQueryRequest, GetProvidersQueryResponse>
    {
        private readonly IProviderRepository _providerRepository;

        public GetProvidersQueryHandler(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public GetProvidersQueryResponse Handle(GetProvidersQueryRequest message)
        {
            try
            {
                var providerEntities = _providerRepository.GetAllProviders();

                return new GetProvidersQueryResponse
                {
                    IsValid = true,
                    Items = providerEntities == null
                        ? null
                        : providerEntities.Select(p =>
                            new Domain.Provider
                            {
                                Ukprn = p.Ukprn
                            }).ToArray()
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