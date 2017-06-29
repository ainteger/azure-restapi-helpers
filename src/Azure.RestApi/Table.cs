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

        public Table(IAzureStorageHandler apiHandler, IWebRequest webRequest)
        {
            ApiHandler = apiHandler;
            WebRequest = webRequest;
        }

        public async Task<bool> CreateTableAsync(string tableName)
        {
            var requestModel = new CreateTableMessage { TableName = tableName };
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Post, "Tables", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestModel)));
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTableAsync(string tableName)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Delete, $"Tables('{tableName}')");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<string>> ListTables()
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Get, $"Tables");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<GetTablesResponse>(json);

                if (data != null)
                {
                    return data.Value.Select(table => table.TableName);
                }
            }

            return default(IEnumerable<string>);
        }

        public async Task<Entity> GetRowOrDefaultAsync<Entity>(string table, string partitionKey, string rowKey)
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
