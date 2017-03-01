using Azure.RestApi.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Table : ITable
    {
        private IAzureStorageHandler ApiHandler { get; }
        private IWebRequest WebRequest { get; }

        public Table(IAzureStorageHandler apiHandler, IWebRequest webRequest, StorageAuthentication storageAuthentication)
        {
            ApiHandler = apiHandler;
            WebRequest = webRequest;
        }

        public async Task<Entity> GetRowAsync<Entity>(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Get, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')");
            var response = await WebRequest.SendAsync(request);

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
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Post, table, Encoding.UTF8.GetBytes(json));
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Delete, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')", ifMatch: "*");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
