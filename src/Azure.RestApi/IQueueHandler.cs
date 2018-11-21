using Azure.RestApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.RestApi
{
	public interface IQueueHandler
	{
		/// <summary>
		/// List queues for storage
		/// </summary>
		/// <returns>Name of queues</returns>
		Task<IEnumerable<string>> ListQueuesAsync();
		/// <summary>
		/// Put message to Azure queue
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <param name="messageBody">Message encoded in UTF-8</param>
		/// <returns>If succeeded</returns>
		Task<bool> PutMessageAsync(string queue, string messageBody);
		/// <summary>
		/// Creates a queue
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>If succeded</returns>
		Task<bool> CreateQueueAsync(string queue);
		/// <summary>
		/// Delete queue
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>If succeded</returns>
		Task<bool> DeleteQueueAsync(string queue);
		/// <summary>
		/// Peek message in queue
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>Message text</returns>
		Task<string> PeekMessageOrDefaultAsync(string queue);
		/// <summary>
		/// Get message and delete it
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>Message text</returns>
		Task<string> PopMessageOrDefaultAsync(string queue);
		/// <summary>
		/// Get message without delete it
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>Message context</returns>
		Task<IQueueMessage> GetMessageOrDefaultAsync(string queue);
		/// <summary>
		/// Delete message
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <param name="messageId">Message id</param>
		/// <param name="popReceipt">Receipt</param>
		/// <returns>If succeded</returns>
		Task<bool> DeleteMessageAsync(string queue, Guid messageId, string popReceipt);
		/// <summary>
		/// Clear all messages in queue
		/// </summary>
		/// <param name="queue">Name of queue</param>
		/// <returns>If succeded</returns>
		Task<bool> ClearMessagesAsync(string queue);
	}
}
