using System.Runtime.CompilerServices;
using Azure.RestApi.Models;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azure.RestApi.Tests
{
	public class Container : HttpMessageHandler, IContainer
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}
	}
	public interface IContainer { }

	public class BlobHandlerTests
	{
		[Fact]
		public async Task GivenCorrectDataExist_WhenPutBlobWithData_ThenResultIsSuccess()
		{
			//Given
			var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
			var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
			httpClientFactoryMock.CreateClient().Returns(new HttpClient(fakeHttpMessageHandler));

			var apiHandler = Substitute.For<IAzureStorageHandler>();
			apiHandler.GetRequest(Arg.Is(StorageType.Blob), Arg.Is(HttpMethod.Put), Arg.Any<string>(), Arg.Any<byte[]>()).Returns(
					x => { return new HttpRequestMessage(HttpMethod.Put, "http://www.justafake.com/something"); }
			);
			var servant = new AzureBlobClient(apiHandler, httpClientFactoryMock);
			var content = Encoding.UTF8.GetBytes("arandomstring");

			//When
			var result = await servant.PutBlobAsync("test", "test.jpg", content);

			//Then
			result.Equals(true);
		}
	}
}