using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun
{
    public class RecordProcessorRunHandler : IAsyncRequestHandler<RecordProcessorRunRequest, RecordProcessorRunResponse>
    {
        private readonly IDataLockEventRepository _dataLockEventRepository;

        public RecordProcessorRunHandler(IDataLockEventRepository dataLockEventRepository)
        {
            _dataLockEventRepository = dataLockEventRepository;
        }

        public async Task<RecordProcessorRunResponse> Handle(RecordProcessorRunRequest message)
        {
            try
            {
                var runId = await _dataLockEventRepository.InsertOrUpdateProcessRunRecord(message.RunId, message.Ukprn, message.IlrSubmissionDateTime, message.StartTimeUtc, message.FinishTimeUtc, message.IsInitialRun, message.IsSuccess, message.Error).ConfigureAwait(false);

                if (message.FinishTimeUtc.HasValue)
                {
                    await _dataLockEventRepository.ClearProviderProcessor(message.Ukprn);
                }
                else
                {
                    await _dataLockEventRepository.SetProviderProcessor(message.Ukprn, runId);
                }

                return new RecordProcessorRunResponse
                {
                    IsValid = true,
                    RunId = runId
                };
            }
            catch (Exception ex)
            {
                return new RecordProcessorRunResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}