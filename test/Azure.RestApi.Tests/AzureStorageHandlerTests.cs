using Azure.RestApi.Models;
using System;
using System.Net.Http;
using Xunit;

namespace Azure.RestApi.Tests
{
    public class AzureStorageHandlerTests
    {
        private AzureStorageHandler GetHandler()
        {
            return new AzureStorageHandler(new StorageAuthentication { AccountName = FakeData.ACCOUNTNAME, StorageKey = FakeData.STORAGEKEY });
        }

        [Fact]
        public void GetCanonicalizedResource()
        {
            //Given
            var expected = "/testaccount/";

            //When
            var actual = GetHandler().GetCanonicalizedResource(Models.StorageType.Queue, FakeData.GetBaseUrl(FakeData.QUEUE), FakeData.ACCOUNTNAME);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetRequest()
        {
            //Given
            var queue = "testqueue";

            //When
            var actual = GetHandler().GetRequest(Models.StorageType.Queue, HttpMethod.Get, $"{queue}/messages");

            //Then
            Assert.Equal(HttpMethod.Get, actual.Method);
            Assert.Equal(new Uri("https://testaccount.queue.core.windows.net/testqueue/messages"), actual.RequestUri);
            Assert.Equal("SharedKey", actual.Headers.Authorization.Scheme);
        }

        [Theory]
        [InlineData(StorageType.Queue, "https://testaccount.queue.core.windows.net/testqueue/messages", "SharedKey testaccount:LNp4K2Tjzr863roy7RAwYUr9h3R47zHqT2g7EC5UClY=")]

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
    }
}
