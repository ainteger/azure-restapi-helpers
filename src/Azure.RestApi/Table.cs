using Azure.RestApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
            var requestModel = new CreateTableRequest { TableName = tableName };
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

        public async Task<string> GetRowsAsync(string table, string filter = null)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                filter = $"?$filter={filter}";
            }

            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Get, $"{table}(){filter}");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return default(string);
        }

        public async Task<string> GetRowOrDefaultAsync(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Get, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return default(string);
        }

        public async Task<bool> CreateRowAsync(string table, string entityJson)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Post, table, Encoding.UTF8.GetBytes(entityJson));
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(StorageType.Table, HttpMethod.Delete, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')", ifMatch: "*");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public string GetEncodedFilterPropertyValue(string filterPropertyValue)
        {
            return filterPropertyValue
                .Replace(" ", "%20")
                .Replace("'", "''")
                .Replace("/", "%2F")
                .Replace("?", "%3F")
                .Replace(":", "%3A")
                .Replace("@", "%40")
                .Replace("&", "%26")
                .Replace("=", "%3D")
                .Replace("+", "%2B")
                .Replace(",", "%2C")
                .Replace("$", "%24");
        }
    }
}
