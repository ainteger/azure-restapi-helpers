using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Azure.RestApi
{
    public class Queue : IQueue, IDisposable
    {
        private ApiHandler ApiHandler { get; }
        private HttpClient Client { get; }

        public Queue(StorageAuthentication storageAuthentication)
        {
            ApiHandler = new ApiHandler(storageAuthentication.AccountName, storageAuthentication.StorageKey, "queue", false);
            Client = new HttpClient();
        }

        /// <summary>
        /// Put message to Azure queue
        /// </summary>
        /// <param name="queue">Name of queue</param>
        /// <param name="messageBody">Message encoded in UTF-8</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutMessageAsync(string queue, string messageBody)
        {
            var messageBodyBytes = new UTF8Encoding().GetBytes(messageBody);
            var messageBodyBase64 = Convert.ToBase64String(messageBodyBytes);
            var message = "<QueueMessage><MessageText>" + messageBodyBase64 + "</MessageText></QueueMessage>";

            var request = ApiHandler.GetRequest(HttpMethod.Post, queue + "/messages", message, null);
            return await Client.SendAsync(request);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
