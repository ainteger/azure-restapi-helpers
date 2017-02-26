using System.Threading.Tasks;

namespace Azure.RestApi
{
    public interface IQueue
    {
        Task<bool> PutMessageAsync(string queue, string messageBody);
        Task<bool> CreateQueueAsync(string queue);
        Task<bool> DeleteQueueAsync(string queue);
        Task<string> PeekMessageAsync(string queue);
        Task<string> GetMessageAsync(string queue, bool delete);
        Task<bool> DeleteMessageAsync(string queue, string messageId, string popReceipt);
        Task<bool> ClearMessagesAsync(string queue);
    }
}
