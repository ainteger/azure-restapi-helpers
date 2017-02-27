using System;

namespace Azure.RestApi.Models
{
    public interface IQueueMessage
    {
        Guid Id { get; }
        string Content { get; }
        string PopReceipt { get; }
    }
}
