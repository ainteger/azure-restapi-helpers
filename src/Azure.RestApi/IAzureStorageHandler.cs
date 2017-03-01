using Azure.RestApi.Models;
using System.Net.Http;

namespace Azure.RestApi
{
    public interface IAzureStorageHandler
    {
        HttpRequestMessage GetRequest(StorageType storageType, HttpMethod method, string resource, byte[] requestBody = null, string ifMatch = "");
    }
}
