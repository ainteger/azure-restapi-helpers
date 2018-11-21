using Azure.RestApi.Models;
using System;
using System.Net.Http;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Options;

namespace Azure.RestApi.Tests
{
	public class AzureStorageHandlerTests
	{
		private AzureStorageHandler GetHandler()
		{
			var settings = Substitute.For<IOptions<AzureRestApiOptions>>();
			settings.Value.Returns(new AzureRestApiOptions
			{
				StorageAccountName = FakeData.ACCOUNTNAME,
				StorageKey = FakeData.STORAGEKEY
			});

			return new AzureStorageHandler(settings);
		}

		[Fact]
		public void GetCanonicalizedResource()
		{
			//Given
			var expected = "/testaccount/";


			//When
			var actual = GetHandler().GetCanonicalizedResource(StorageType.Queue, FakeData.GetBaseUrl(FakeData.QUEUE), FakeData.ACCOUNTNAME);

			//Then
			actual.Equals(expected);
		}

		[Fact]
		public void GetRequest()
		{
			//Given
			var queue = "testqueue";

			//When
			var actual = GetHandler().GetRequest(StorageType.Queue, HttpMethod.Get, $"{queue}/messages");

			//Then
			Assert.Equal(HttpMethod.Get, actual.Method);
			Assert.Equal(new Uri("https://testaccount.queue.core.windows.net/testqueue/messages"), actual.RequestUri);
			Assert.Equal("SharedKey", actual.Headers.Authorization.Scheme);
		}

		[Theory]
		[InlineData(StorageType.Queue, "https://testaccount.queue.core.windows.net/testqueue/messages", "SharedKey testaccount:LNp4K2Tjzr863roy7RAwYUr9h3R47zHqT2g7EC5UClY=")]
		[InlineData(StorageType.Blob, "https://testaccount.blob.core.windows.net/testblob/testblob", "SharedKey testaccount:VP1o60IMARJv3EcTExvm1NIJmoZUUmQ8QKzUwNxI+ic=")]
		[InlineData(StorageType.Table, "https://testaccount.blob.core.windows.net/testtable(PartitionKey='testpartitionkey',RowKey='testrowkey')", "SharedKeyLite testaccount:uQ3szkBt7rY4WVSMSW/prIZNHAOiOBLdwENtQLi28C4=")]
		public void GetAuthorizationHeader(StorageType storageType, string url, string expected)
		{
			//Given
			var method = HttpMethod.Get;
			var timestamp = new DateTime(2017, 1, 1, 12, 34, 2);
			var fakeRequest = new HttpRequestMessage(method, url);

			//When
			var actual = GetHandler().GetAuthorizationHeader(storageType, method, timestamp, fakeRequest, "");

			//Then
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GetSignature()
		{
			//Given
			var expected = "H/5kpHyEA6a0kf+YI8CMDgiaw12aCPBi04s7JRHdFpM=";

			//When
			var actual = GetHandler().GetSignature("justtestingarandomvalueisgood");

			//Then
			Assert.Equal(expected, actual);
		}
	}
}
