using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Events.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly string _clientToken;

        public SecureHttpClient(string clientToken)
        {
            _clientToken = clientToken;
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
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _clientToken);

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
                    var webEx = (WebException) ex.InnerException;
                    if (webEx.InnerException != null && webEx.InnerException is SocketException)
                    {
                        var sockEx = (SocketException) webEx.InnerException;

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
    }
}
