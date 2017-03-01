using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface IWebRequest
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
