using Azure.RestApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
	public interface IAzureQueueClient
	{
		Task<IEnumerable<string>> ListQueuesAsync();
		Task<AzureResponse> PutMessageAsync(string queue, string messageBody);
		Task<AzureResponse> CreateQueueAsync(string queue);
		Task<AzureResponse> DeleteQueueAsync(string queue);
		Task<string> PeekMessageOrDefaultAsync(string queue);
		Task<string> PopMessageOrDefaultAsync(string queue);
		Task<IQueueMessage> GetMessageOrDefaultAsync(string queue);
		Task<AzureResponse> DeleteMessageAsync(string queue, Guid messageId, string popReceipt);
		Task<AzureResponse> ClearMessagesAsync(string queue);
	}
}
