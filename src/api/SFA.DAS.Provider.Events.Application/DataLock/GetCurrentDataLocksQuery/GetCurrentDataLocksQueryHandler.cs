using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
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

                var dataLockEvents = await _dataLockRepository.GetDataLocks(message.Ukprn, message.PageNumber, message.PageSize);

                return new GetCurrentDataLocksQueryResponse
                {
                    IsValid = true,
                    Result = new PageOfResults<Api.Types.DataLock>
                    {
                        Items = dataLockEvents.Select(ToDataLock).ToArray(),
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

        private static Api.Types.DataLock ToDataLock(DataLockEntity e)
        {
            var dataLock = new Api.Types.DataLock
            {
                Ukprn = e.Ukprn,
                LearnerReferenceNumber = e.LearnerReferenceNumber,
                PriceEpisodeIdentifier = e.PriceEpisodeIdentifier,
                AimSequenceNumber = e.AimSequenceNumber,
                IlrEndpointAssessorPrice = e.IlrEndpointAssessorPrice,
                IlrFrameworkCode = e.IlrFrameworkCode,
                IlrPathwayCode = e.IlrPathwayCode,
                IlrPriceEffectiveFromDate = e.IlrPriceEffectiveFromDate,
                IlrPriceEffectiveToDate = e.IlrPriceEffectiveToDate,
                IlrProgrammeType = e.IlrProgrammeType,
                IlrStandardCode = e.IlrStandardCode,
                IlrStartDate = e.IlrStartDate,
                IlrTrainingPrice = e.IlrTrainingPrice,
                Uln = e.Uln
            };

            if (!string.IsNullOrEmpty(e.ErrorCodes))
                dataLock.ErrorCodes = JsonConvert.DeserializeObject<List<string>>(e.ErrorCodes);

            if (!string.IsNullOrEmpty(e.CommitmentVersions))
                dataLock.CommitmentVersions = JsonConvert.DeserializeObject<List<DataLockEventApprenticeship>>(e.CommitmentVersions);

            return dataLock;
        }
    }
}