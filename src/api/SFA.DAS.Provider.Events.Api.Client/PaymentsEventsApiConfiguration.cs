using System;

namespace SFA.DAS.Provider.Events.Api.Client
{
    /// <summary>
    /// Api configuration
    /// </summary>
    public class PaymentsEventsApiConfiguration : IPaymentsEventsApiConfiguration
    {
        /// <summary>
        /// The JWT token issued to your service for access to the API
        /// </summary>
        [Obsolete("Jwt token usage is obsolete. Use Azure AD authentication.")]
        public string ClientToken { get; set; }

        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        public string ApiBaseUrl { get; set; }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
        public string Tenant { get; }
    }
}