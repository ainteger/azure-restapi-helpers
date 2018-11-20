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
    public class TableTests
    {
        [Fact]
        public void GivenStrangeString_WhenFiltering_ThenStringIsEncoded()
        {
            //Given            
            var servant = new Table(Substitute.For<IAzureStorageHandler>(), Substitute.For<IWebRequest>());
            var content = "test string '/?:@&=+,$ end test";

            //When
            var result = servant.GetEncodedFilterPropertyValue(content);

            //Then
            result.Equals("test%20string%20''%2F%3F%3A%40%26%3D%2B%2C%24%20end%20test");
        }
    }
}