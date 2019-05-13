using System;

namespace Azure.RestApi.Models
{
	public class QueueMessage : IQueueMessage
	{
		public Guid Id { get; set; }
		public string Content { get; set; }
		public string PopReceipt { get; set; }
	}
}
