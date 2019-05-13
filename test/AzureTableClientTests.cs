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
	public class Given_table_with_data
	{
		[Fact]
		public async Task when_filter_is_null_then_all_rows_are_returned()
		{
			//Given
			var fakeValue = "{\"value\":[{\"PartitionKey\":\"1\", \"RowKey\": \"A\", \"Value\": 4},{\"PartitionKey\":\"2\", \"RowKey\": \"B\", \"Value\": 4}]}";

			var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
			var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(fakeValue, Encoding.UTF8, "application/json")
			});
			httpClientFactoryMock.CreateClient().Returns(new HttpClient(fakeHttpMessageHandler));

			var apiHandler = Substitute.For<IAzureStorageHandler>();
			apiHandler.GetRequest(Arg.Is(StorageType.Table), Arg.Is(HttpMethod.Get), "faketable()").Returns(
					x => { return new HttpRequestMessage(HttpMethod.Get, "http://www.justafake.com/faketable()"); }
			);
			var servant = new AzureTableClient(apiHandler, httpClientFactoryMock);

			//When
			var result = await servant.GetRowsAsync("faketable");

			//Then
			fakeHttpMessageHandler.Requests.Contains(new Request { Method = HttpMethod.Get, Url = "http://www.justafake.com/faketable()" });

			var data = JsonConvert.DeserializeObject(result);
		}
	}
}