using Azure.RestApi.Models;

namespace Azure.RestApi
{
	public interface IAzureTableClient
	{
		Task<AzureResponse> CreateTableAsync(string tableName);
		Task<AzureResponse> DeleteTableAsync(string tableName);
		Task<IEnumerable<string>?> ListTablesAsync();
		Task<string?> GetRowOrDefaultAsync(string table, string partitionKey, string rowKey);
		Task<string?> GetRowsAsync(string table, string? filter = null);
		Task<AzureResponse> CreateRowAsync(string table, string entityJson);
		Task<AzureResponse> DeleteRowAsync(string table, string partitionKey, string rowKey);
		string GetEncodedFilterPropertyValue(string filterPropertyValue);
		Task<AzureResponse> UpdateRowAsync(string table, string partitionKey, string rowKey, string entityJson, bool upsert = false);
	}
}
