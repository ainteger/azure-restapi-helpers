using Azure.RestApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
	public interface IBlobHandler
	{
		Task<bool> PutBlobAsync(string container, string contentName, byte[] content);
		Task<byte[]> GetBlobOrDefaultAsync(string container, string contentName);
		Task<bool> DeleteBlobAsync(string container, string contentName);
		Task<IEnumerable<IBlobData>> ListBlobsAsync(string container);
	}
}
