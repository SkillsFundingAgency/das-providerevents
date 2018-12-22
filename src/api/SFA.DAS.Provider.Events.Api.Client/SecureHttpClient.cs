using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.Provider.Events.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly IPaymentsEventsApiConfiguration _clientConfiguration;

        public SecureHttpClient(IPaymentsEventsApiConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }
        protected SecureHttpClient()
        {
            // So we can mock for testing
        }

        public virtual async Task<string> GetAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (_clientConfiguration != null)
                    {
                        var authenticationResult = await GetAuthenticationResult(_clientConfiguration);
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                    }
                    client.DefaultRequestHeaders.Add("api-version", "2");

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (UriFormatException ex)
            {
                throw new BadRequestException("Url is malformed", ex);
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException is WebException)
                {
                    var webEx = (WebException)ex.InnerException;
                    if (webEx.InnerException != null && webEx.InnerException is SocketException)
                    {
                        var sockEx = (SocketException)webEx.InnerException;

                        throw new ServerUnavailableException(ex, sockEx.SocketErrorCode);
                    }
                }
                throw new ApiException("Protocol error with API - " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApiException("Unexpected error in API - " + ex.Message, ex);
            }
        }

        private async Task<AuthenticationResult> GetAuthenticationResult(IPaymentsEventsApiConfiguration configuration)
        {
            var authority = $"https://login.microsoftonline.com/{configuration.Tenant}";
            var clientCredential = new ClientCredential(configuration.ClientId, configuration.ClientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(configuration.IdentifierUri, clientCredential);
            return result;
        }
    }
}
