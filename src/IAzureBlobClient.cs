using Azure.RestApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
	public interface IAzureBlobClient
	{
		Task<AzureResponse> PutBlobAsync(string container, string contentName, byte[] content);
		Task<byte[]> GetBlobOrDefaultAsync(string container, string contentName);
		Task<AzureResponse> DeleteBlobAsync(string container, string contentName);
		Task<IEnumerable<IBlobData>> ListBlobsAsync(string container);
	}
}
