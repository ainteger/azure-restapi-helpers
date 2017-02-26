using System;
using Xunit;

namespace Azure.RestApi.Tests
{
    public class ApiHandlerTests
    {
        private const string FAKE_STORAGEKEY = "SSjwpaQxpuE39Ge1rtQSFG2d+SuyCdLfBavFFvWkgGuiJ3snVmHN7YNYsd1rQ6FVlco2GzCbfkwOplcrHJTgNA==";
        private const string FAKE_ACCOUNTNAME = "testaccount";

        [Theory]
        [InlineData("queue", "test", "/testaccount/test/messages")]        
        public void GetCanonicalizedResource(string azureFunction, string container, string expected)
        {
            //Given
            var url = new Uri($"https://{FAKE_ACCOUNTNAME}.{azureFunction}.core.windows.net/{container}/messages");
            var apiHandler = new ApiHandler(FAKE_ACCOUNTNAME,FAKE_STORAGEKEY, "queue", false);

            //When
            var actual = apiHandler.GetCanonicalizedResource(url, FAKE_ACCOUNTNAME);

            //Then
            Assert.Equal(expected, actual);
        }
    }
}
