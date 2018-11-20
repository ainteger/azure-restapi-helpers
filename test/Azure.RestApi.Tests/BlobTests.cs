using Azure.RestApi.Models;
using System;
using System.Net.Http;
using Xunit;
using NSubstitute;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace Azure.RestApi.Tests
{
    public class BlobTests
    {
        [Fact]
        public async Task GivenCorrectDataExist_WhenPutBlobWithData_ThenResultIsSuccess()
        {
            //Given
            var webRequest = Substitute.For<IWebRequest>();
            webRequest.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(x =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });
            var apiHandler = Substitute.For<IAzureStorageHandler>();
            apiHandler.GetRequest(Arg.Is(StorageType.Blob), Arg.Is(HttpMethod.Put), Arg.Any<string>(), Arg.Any<byte[]>()).Returns(
                x => { return new HttpRequestMessage(); }
            );
            var servant = new Blob(apiHandler, webRequest);
            var content = Encoding.UTF8.GetBytes("arandomstring");

            //When
            var result = await servant.PutBlobAsync("test", "test.jpg", content);

            //Then
            result.Equals(true);
        }
    }
}