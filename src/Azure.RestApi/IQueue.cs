using Azure.RestApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface IQueue
    {
        Task<IEnumerable<string>> ListQueuesAsync();
        Task<bool> PutMessageAsync(string queue, string messageBody);
        Task<bool> CreateQueueAsync(string queue);
        Task<bool> DeleteQueueAsync(string queue);
        Task<string> PeekMessageAsync(string queue);
        Task<string> PopMessageAsync(string queue);
        Task<IQueueMessage> GetMessageAsync(string queue);
        Task<bool> DeleteMessageAsync(string queue, Guid messageId, string popReceipt);
        Task<bool> ClearMessagesAsync(string queue);
    }
}
