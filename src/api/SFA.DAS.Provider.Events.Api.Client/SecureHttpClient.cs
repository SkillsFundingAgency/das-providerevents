using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.Client
{
    internal class SecureHttpClient : ISecureHttpClient
    {
        private readonly IPaymentsEventsApiClientConfiguration _configuration;
        private readonly HttpMessageHandler _handler;

        public SecureHttpClient(IPaymentsEventsApiClientConfiguration configuration)
            : this(configuration, null)
        {
            _configuration = configuration;
        }

        public SecureHttpClient(IPaymentsEventsApiClientConfiguration configuration, HttpMessageHandler handler)
        {
            _configuration = configuration;
            _handler = handler;
        }

        public virtual async Task<string> GetAsync(string url)
        {
            var accessToken = IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant)
                ? await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant)
                : await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);
                
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        private HttpClient GetHttpClient()
        {
            return _handler != null ? new HttpClient(_handler) : new HttpClient();
        }

        private async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }

        private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
        {
            if(!string.IsNullOrEmpty(resource))
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
            }

            return await Task.FromResult(string.Empty);
        }

        private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
        {
            return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
        }
    }

    internal interface ISecureHttpClient
    {
        Task<string> GetAsync(string url);
    }
}
