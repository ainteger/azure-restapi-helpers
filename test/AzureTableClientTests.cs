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
		}

		[Fact]
		public async Task when_filter_for_partition_key_then_filter_request_are_correct()
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
			apiHandler.GetRequest(Arg.Is(StorageType.Table), Arg.Is(HttpMethod.Get), "faketable()?$filter=PartitionKey eq 'A'").Returns(
					x => { return new HttpRequestMessage(HttpMethod.Get, "http://www.justafake.com/faketable()"); }
			);
			var servant = new AzureTableClient(apiHandler, httpClientFactoryMock);

			//When
			var result = await servant.GetRowsAsync("faketable", "PartitionKey eq 'A'");

			//Then
			fakeHttpMessageHandler.Requests.Contains(new Request { Method = HttpMethod.Get, Url = "http://www.justafake.com/faketable()" });
		}
	}

	public class Given_table
	{
		[Fact]
		public void when_filtering_then_string_is_encoded_for_request()
		{
			//Given            
			var servant = new AzureTableClient(Substitute.For<IAzureStorageHandler>(), Substitute.For<IHttpClientFactory>());
			var content = "test string '/?:@&=+,$ end test";

			//When
			var result = servant.GetEncodedFilterPropertyValue(content);

			//Then
			result.Equals("test%20string%20''%2F%3F%3A%40%26%3D%2B%2C%24%20end%20test");
		}
	}
}