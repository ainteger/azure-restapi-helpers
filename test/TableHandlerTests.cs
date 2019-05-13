using System.Net.Http;
using NSubstitute;
using Xunit;

namespace Azure.RestApi.Tests
{
	public class TableHandlerTests
	{
		[Fact]
		public void GivenStrangeString_WhenFiltering_ThenStringIsEncoded()
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