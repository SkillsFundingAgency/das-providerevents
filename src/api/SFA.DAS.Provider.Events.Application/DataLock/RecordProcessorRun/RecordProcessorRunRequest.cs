using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.RecordProcessorRun
{
    public class RecordProcessorRunRequest : IAsyncRequest<RecordProcessorRunResponse>
    {
        public int? RunId { get; set; }
        public long Ukprn { get; set; }
        public DateTime? IlrSubmissionDateTime { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? FinishTimeUtc { get; set; }
        public bool? IsInitialRun { get; set; }
        public bool? IsSuccess { get; set; }
        public string Error { get; set; }
    }
}