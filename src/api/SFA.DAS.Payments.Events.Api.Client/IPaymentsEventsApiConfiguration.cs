namespace SFA.DAS.Payments.Events.Api.Client
{
    public interface IPaymentsEventsApiConfiguration
    {
        /// <summary>
        /// The JWT token issued to your service for access to the API
        /// </summary>
        string ClientToken { get; set; }

        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        string ApiBaseUrl { get; set; }
    }
}