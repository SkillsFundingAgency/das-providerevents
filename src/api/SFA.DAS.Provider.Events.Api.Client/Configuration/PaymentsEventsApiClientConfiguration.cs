namespace SFA.DAS.Provider.Events.Api.Client.Configuration
{
    public class PaymentsEventsApiClientConfiguration : IPaymentsEventsApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}