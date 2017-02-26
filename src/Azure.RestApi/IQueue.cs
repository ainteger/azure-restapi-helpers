using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface IQueue
    {
        Task<HttpResponseMessage> PutMessageAsync(string queue, string messageBody);
    }
}
