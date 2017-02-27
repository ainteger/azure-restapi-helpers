using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Table : ITable
    {
        private ApiHandler ApiHandler { get; }
        private HttpClient Client { get; }

        public Table(StorageAuthentication storageAuthentication)
        {
            ApiHandler = new ApiHandler(storageAuthentication.AccountName, storageAuthentication.StorageKey, "table", true);
            Client = new HttpClient();
        }

        public async Task<Entity> GetRowAsync<Entity>(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(HttpMethod.Get, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')");
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
            var request = ApiHandler.GetRequest(HttpMethod.Post, table, json);
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(HttpMethod.Delete, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')", ifMatch: "*");
            var response = await Client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
