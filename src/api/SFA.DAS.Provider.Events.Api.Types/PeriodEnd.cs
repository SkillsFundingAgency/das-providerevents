using System;
using Newtonsoft.Json;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class PeriodEnd
    {
        public string Id { get; set; }
        public CalendarPeriod CalendarPeriod { get; set; }
        public ReferenceDataDetails ReferenceData { get; set; }
        public DateTime CompletionDateTime { get; set; }

        [JsonProperty("_links")]
        public PeriodEndLinks Links { get; set; }
    }
}