using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery
{
    public class GetLatestDataLocksQueryHandler : IAsyncRequestHandler<GetLatestDataLocksQueryRequest, GetLatestDataLocksQueryResponse>
    {
        private readonly IValidator<GetLatestDataLocksQueryRequest> _validator;
        private readonly IDataLockEventRepository _dataLockEventRepository;

        public GetLatestDataLocksQueryHandler(
            IValidator<GetLatestDataLocksQueryRequest> validator, 
            IDataLockEventRepository dataLockEventRepository)
        {
            _validator = validator;
            _dataLockEventRepository = dataLockEventRepository;
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

                var dataLocks = new List<Api.Types.DataLock>();

                foreach (var entity in entities.Items)
                {
                    var dataLock = new Api.Types.DataLock
                    {
                        Ukprn = entity.Ukprn,
                        LearnerReferenceNumber = entity.LearnerReferenceNumber,
                        AimSequenceNumber = entity.AimSequenceNumber,
                        PriceEpisodeIdentifier = entity.PriceEpisodeIdentifier
                    };

                    if (!string.IsNullOrEmpty(entity.ErrorCodes))
                        dataLock.ErrorCodes = JsonConvert.DeserializeObject<List<string>>(entity.ErrorCodes);

                    if (!string.IsNullOrEmpty(entity.CommitmentVersions))
                        dataLock.CommitmentVersions = JsonConvert.DeserializeObject<List<DataLockEventApprenticeship>>(entity.CommitmentVersions);

                    dataLocks.Add(dataLock);
                }

                return new GetLatestDataLocksQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Api.Types.DataLock>
                    {
                        Items = dataLocks.ToArray(),
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