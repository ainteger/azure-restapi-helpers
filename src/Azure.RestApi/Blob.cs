using Azure.RestApi.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Blob : IBlob, IDisposable
    {
        private IAzureStorageHandler ApiHandler { get; }
        private HttpClient Client { get; }

        public Blob(IAzureStorageHandler apiHandler, StorageAuthentication storageAuthentication)
        {
            ApiHandler = apiHandler;
            Client = new HttpClient();
        }

        public async Task<bool> PutBlobAsync(string container, string contentName, byte[] content)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Put, $"{container}/{contentName}", content);
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]> GetBlobAsync(string container, string contentName)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Get, $"{container}/{contentName}");
            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return default(byte[]);
        }

        public async Task<bool> DeleteBlobAsync(string container, string contentName)
        {
            var request = ApiHandler.GetRequest(StorageType.Blob, HttpMethod.Delete, $"{container}/{contentName}");
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
