using Azure.RestApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Blob : IBlob
    {
        private IAzureStorageHandler ApiHandler { get; }
        private IWebRequest WebRequest { get; }

        public Blob(IAzureStorageHandler apiHandler, IWebRequest webRequest)
        {
            ApiHandler = apiHandler;
            WebRequest = webRequest;
        }

        public async Task<bool> PutBlobAsync(string container, string contentName, byte[] content)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Put, $"{container}/{contentName}", content);
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]> GetBlobAsync(string container, string contentName)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Get, $"{container}/{contentName}");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return default(byte[]);
        }

        public async Task<bool> DeleteBlobAsync(string container, string contentName)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Delete, $"{container}/{contentName}");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
