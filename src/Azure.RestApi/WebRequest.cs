using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class WebRequest : IWebRequest, IDisposable
    {
        private HttpClient Client { get; }

        public WebRequest()
        {
            Client = new HttpClient();
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await Client.SendAsync(request);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
