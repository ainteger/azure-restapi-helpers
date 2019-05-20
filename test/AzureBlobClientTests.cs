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
	public class Given_blob
	{
		[Fact]
		public async Task when_put_blob_request_then_response_should_be_ok()
		{
			//Given

			var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
			var httpClient = new HttpClient(fakeHttpMessageHandler);

			var apiHandler = Substitute.For<IAzureStorageHandler>();
			apiHandler.GetRequest(Arg.Is(StorageType.Blob), Arg.Is(HttpMethod.Put), Arg.Any<string>(), Arg.Any<byte[]>()).Returns(
					x => { return new HttpRequestMessage(HttpMethod.Put, "http://www.justafake.com/something"); }
			);
			var servant = new AzureBlobClient(apiHandler, httpClient);
			var content = Encoding.UTF8.GetBytes("arandomstring");

			//When
			var result = await servant.PutBlobAsync("test", "test.jpg", content);

			//Then
			result.Equals(true);
		}
	}
}