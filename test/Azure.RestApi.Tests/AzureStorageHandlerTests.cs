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
    }
}
