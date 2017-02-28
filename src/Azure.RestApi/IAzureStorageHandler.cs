using Azure.RestApi.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace Azure.RestApi
{
    public interface IAzureStorageHandler
    {
        HttpRequestMessage GetRequest(
            StorageType storageType,
            HttpMethod method,
            string resource,
            string requestBody = null,
            SortedList<string, string> headers = null,
            string ifMatch = "",
            string md5 = "");
    }
}
