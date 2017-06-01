using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface ITable
    {
        Task<Entity> GetRowOrDefaultAsync<Entity>(string table, string partitionKey, string rowKey);
        Task<bool> CreateRowAsync<Entity>(string table, Entity entity);
        Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey);
    }
}
