namespace SFA.DAS.Provider.Events.Api.Client.Configuration
{
    public class PaymentsEventsApiClientConfiguration : IPaymentsEventsApiClientConfiguration
    {
        public string ApiBaseUrl { get; }
        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
    }
}