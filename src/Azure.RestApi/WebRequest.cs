using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
	public class WebRequest : IWebRequest
	{
		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			using (var client = new HttpClient())
			{
				return await client.SendAsync(request);
			}
		}
	}
}
