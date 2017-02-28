using Azure.RestApi.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Table : ITable, IDisposable
    {
        private IAzureStorageHandler ApiHandler { get; }
        private HttpClient Client { get; }

        public Table(IAzureStorageHandler apiHandler, StorageAuthentication storageAuthentication)
        {
            ApiHandler = apiHandler;
            Client = new HttpClient();
        }

        public async Task<Entity> GetRowAsync<Entity>(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Get, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')");
            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Entity>(json);
            }

            return default(Entity);
        }

        public async Task<bool> CreateRowAsync<Entity>(string table, Entity entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Post, table, json);
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Delete, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')", ifMatch: "*");
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
