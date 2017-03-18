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
    public class Queue : IQueue
    {
        private IAzureStorageHandler ApiHandler { get; }
        private IWebRequest WebRequest { get; }

        public Queue(IAzureStorageHandler apiHandler, IWebRequest webRequest)
        {
            ApiHandler = apiHandler;
            WebRequest = webRequest;
        }

        public async Task<IEnumerable<string>> ListQueuesAsync()
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Get, "?comp=list");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                XElement xml = XElement.Parse(result);

                return xml.Element("Queues").Elements("Queue").Select(q => q.Element("Name").Value);
            }

            return Enumerable.Empty<string>();
        }

        public async Task<bool> PutMessageAsync(string queue, string messageBody)
        {
            var messageBodyBytes = new UTF8Encoding().GetBytes(messageBody);
            var messageBodyBase64 = Convert.ToBase64String(messageBodyBytes);
            var message = $"<QueueMessage><MessageText>{messageBodyBase64}</MessageText></QueueMessage>";

            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Post, $"{queue}/messages", Encoding.UTF8.GetBytes(message), null);
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateQueueAsync(string queue)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Put, queue);
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQueueAsync(string queue)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, queue);
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> PeekMessageAsync(string queue)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Get, $"{queue}/messages?peekonly=true");
            var response = await WebRequest.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return default(string);
        }

        public async Task<string> PopMessageAsync(string queue)
        {
            var message = await GetMessageAsync(queue);
            await DeleteMessageAsync(queue, message.Id, message.PopReceipt);
            return message.Content;
        }

        public async Task<IQueueMessage> GetMessageAsync(string queue)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Get, $"{queue}/messages");
            var response = await WebRequest.SendAsync(request);

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

        public async Task<bool> DeleteMessageAsync(string queue, Guid messageId, string popReceipt)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, $"{queue}/messages/{messageId}?popreceipt={Uri.EscapeDataString(popReceipt)}");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ClearMessagesAsync(string queue)
        {
            var request = ApiHandler.GetRequest(StorageType.Queue, HttpMethod.Delete, $"{queue}/messages");
            var response = await WebRequest.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
