using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace Azure.RestApi.Tests
{
	public class FakeHttpMessageHandler : DelegatingHandler
	{
		private HttpResponseMessage FakeResponse;

		public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
		{
			FakeResponse = responseMessage;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return await Task.FromResult(FakeResponse);
		}
	}
}