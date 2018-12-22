using System;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public interface IPaymentsEventsApiConfiguration
    {
        /// <summary>
        /// The JWT token issued to your service for access to the API
        /// </summary>
        [Obsolete("Jwt token usage is obsolete. Use Azure AD authentication.")]
        string ClientToken { get; set; }

        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        string ApiBaseUrl { get; set; }
        
        string ClientId { get; }
        string ClientSecret { get; }
        string IdentifierUri { get; }
        string Tenant { get; }
    }
}