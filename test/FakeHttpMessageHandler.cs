using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace Azure.RestApi.Tests
{
	public class FakeHttpMessageHandler : DelegatingHandler
	{
		private HttpResponseMessage FakeResponse;

		public List<Request> Requests { get; private set; } = new List<Request>();

		public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
		{
			FakeResponse = responseMessage;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Requests.Add(new Request { Method = request.Method, Url = request.RequestUri.ToString() });
			return await Task.FromResult(FakeResponse);
		}
	}

	public class Request : IEquatable<Request>
	{
		public HttpMethod Method { get; set; }
		public string Url { get; set; }

		public bool Equals(Request other)
		{
			return Method == other.Method && Url == other.Url;
		}
	}
}