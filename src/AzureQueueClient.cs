using Azure.RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Azure.RestApi
{
	public class AzureQueueClient : IAzureQueueClient
	{
		private IAzureStorageHandler AzureStorageHandler { get; }
		private IHttpClientFactory HttpFactory { get; }

		public AzureQueueClient(IAzureStorageHandler azureStorageHandler, IHttpClientFactory httpFactory)
		{
			AzureStorageHandler = azureStorageHandler;
			HttpFactory = httpFactory;
		}

		public async Task<IEnumerable<string>> ListQueuesAsync()
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Get, "?comp=list");
			var response = await HttpFactory.CreateClient().SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				XElement xml = XElement.Parse(result);

				return xml.Element("Queues").Elements("Queue").Select(q => q.Element("Name").Value);
			}

			return Enumerable.Empty<string>();
		}

		public async Task<AzureResponse> PutMessageAsync(string queue, string messageBody)
		{
			var messageBodyBytes = new UTF8Encoding().GetBytes(messageBody);
			var messageBodyBase64 = Convert.ToBase64String(messageBodyBytes);
			var message = $"<QueueMessage><MessageText>{messageBodyBase64}</MessageText></QueueMessage>";

			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Post, $"{queue}/messages", Encoding.UTF8.GetBytes(message), null);
			var response = await HttpFactory.CreateClient().SendAsync(request);
			return new AzureResponse(response);
		}

		public async Task<AzureResponse> CreateQueueAsync(string queue)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Put, queue);
			var response = await HttpFactory.CreateClient().SendAsync(request);
			return new AzureResponse(response);
		}

		public async Task<AzureResponse> DeleteQueueAsync(string queue)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, queue);
			var response = await HttpFactory.CreateClient().SendAsync(request);
			return new AzureResponse(response);
		}

		public async Task<string> PeekMessageOrDefaultAsync(string queue)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Get, $"{queue}/messages?peekonly=true");
			var response = await HttpFactory.CreateClient().SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				return result;
			}

			return default(string);
		}

		public async Task<string> PopMessageOrDefaultAsync(string queue)
		{
			var message = await GetMessageOrDefaultAsync(queue);

			if (message == null)
			{
				return default(string);
			}

			await DeleteMessageAsync(queue, message.Id, message.PopReceipt);
			return message.Content;
		}

		public async Task<IQueueMessage> GetMessageOrDefaultAsync(string queue)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Get, $"{queue}/messages");
			var response = await HttpFactory.CreateClient().SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();

				var xml = XElement.Parse(result);

				if (xml.Elements("QueueMessage").Count() > 0)
				{
					var queueMessageElement = xml.Element("QueueMessage");

					var messageBody64 = queueMessageElement.Element("MessageText").Value;
					var messsageBodyBytes = Convert.FromBase64String(messageBody64);

					return new QueueMessage
					{
						Id = Guid.Parse(queueMessageElement.Element("MessageId").Value),
						PopReceipt = queueMessageElement.Element("PopReceipt").Value,
						Content = new UTF8Encoding().GetString(messsageBodyBytes)
					};
				}
			}

			return default(QueueMessage);
		}

		public async Task<AzureResponse> DeleteMessageAsync(string queue, Guid messageId, string popReceipt)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, $"{queue}/messages/{messageId}?popreceipt={Uri.EscapeDataString(popReceipt)}");
			var response = await HttpFactory.CreateClient().SendAsync(request);
			return new AzureResponse(response);
		}

		public async Task<AzureResponse> ClearMessagesAsync(string queue)
		{
			var request = AzureStorageHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, $"{queue}/messages");
			var response = await HttpFactory.CreateClient().SendAsync(request);
			return new AzureResponse(response);
		}
	}
}