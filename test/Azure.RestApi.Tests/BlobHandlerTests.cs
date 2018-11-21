using Azure.RestApi.Models;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Azure.RestApi.Tests
{
	public class BlobHandlerTests
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
            var servant = new BlobHandler(apiHandler, webRequest);
            var content = Encoding.UTF8.GetBytes("arandomstring");

            //When
            var result = await servant.PutBlobAsync("test", "test.jpg", content);

            //Then
            result.Equals(true);
        }
    }
}