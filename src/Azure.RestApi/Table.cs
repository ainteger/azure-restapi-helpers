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

        public async Task<Result> GetRowAsync<Result>(string table, string partitionKey, string rowKey)
        {
            var request = ApiHandler.GetRequest(HttpMethod.Get, $"{table}(PartitionKey='{partitionKey}',RowKey='{rowKey}')");
            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Result>(json);
            }

            return default(Result);
        }
    }
}
