using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface ITable
    {
        Task<bool> CreateTableAsync(string tableName);
        Task<bool> DeleteTableAsync(string tableName);
        Task<IEnumerable<string>> ListTables();
        Task<string> GetRowOrDefaultAsync(string table, string partitionKey, string rowKey);
        Task<string> GetRowsAsync(string table, string filter = null);
        Task<bool> CreateRowAsync(string table, string entityJson);
        Task<bool> DeleteRowAsync(string table, string partitionKey, string rowKey);
        string GetEncodedFilterPropertyValue(string filterPropertyValue);
    }
}
