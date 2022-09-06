using Azure.RestApi.Models;

namespace Azure.RestApi
{
	public interface IAzureStorageHandler
	{
		HttpRequestMessage GetRequest(StorageType storageType, HttpMethod method, string resource, byte[]? requestBody = null, string ifMatch = "");
	}
}
