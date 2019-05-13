using Azure.RestApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Azure.RestApi
{
	public class AzureBlobClient : IAzureBlobClient
	{
		private IAzureStorageHandler AzureStorageHandler { get; }
		private IHttpClientFactory HttpFactory { get; }

		public AzureBlobClient(IAzureStorageHandler azureStorageHandler, IHttpClientFactory httpFactory)
		{
			AzureStorageHandler = azureStorageHandler;
			HttpFactory = httpFactory;
		}

		public async Task<AzureResponse> PutBlobAsync(string container, string contentName, byte[] content)
		{
			if (contentName.StartsWith("/"))
			{
				contentName = contentName.Substring(1);
			}

			var request = AzureStorageHandler.GetRequest(StorageType.Blob, HttpMethod.Put, $"{container}/{contentName}", content);
			var response = await GetClient().SendAsync(request);
			return AzureResponse.Parse(response);
		}

		public async Task<byte[]> GetBlobOrDefaultAsync(string container, string contentName)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Blob, HttpMethod.Get, $"{container}/{contentName}");
			var response = await GetClient().SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsByteArrayAsync();
			}

			return default(byte[]);
		}

		public async Task<AzureResponse> DeleteBlobAsync(string container, string contentName)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Blob, HttpMethod.Delete, $"{container}/{contentName}");
			var response = await GetClient().SendAsync(request);
			return AzureResponse.Parse(response);
		}

		public async Task<IEnumerable<IBlobData>> ListBlobsAsync(string container)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Blob, HttpMethod.Get, $"{container}?restype=container&comp=list");
			var response = await GetClient().SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();

				var xml = XElement.Parse(result);

				return xml.Element("Blobs").Elements("Blob").Select(b => new BlobData
				{
					Name = b.Element("Name").Value,
					Url = b.Element("Url").Value,
					ContentType = b.Element("Properties")?.Element("Content-Type")?.Value,
					ContentLength = b.Element("Properties")?.Element("Content-Length")?.Value
				});
			}

			return default(IEnumerable<IBlobData>);
		}

		private HttpClient GetClient()
		{
			return HttpFactory.CreateClient();
		}
	}
}
