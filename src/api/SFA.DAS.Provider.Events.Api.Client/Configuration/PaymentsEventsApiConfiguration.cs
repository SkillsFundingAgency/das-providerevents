namespace SFA.DAS.Provider.Events.Api.Client.Configuration
{
    /// <summary>
    /// Api configuration
    /// </summary>
    public class PaymentsEventsApiConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ApiBaseUrl { get; }
        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
        public string ClientToken { get; }
    }
}