using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface ITable
    {
        Task<Result> GetRowAsync<Result>(string table, string partitionKey, string rowKey);
    }
}
