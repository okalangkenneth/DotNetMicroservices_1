using RabbitMQ.Client;
using System;

namespace Basket.API.Services
{
    public interface IMessageService : IDisposable
    {
        IModel Channel { get; }
        void PublishMessage(string routingKey, object message);
        string ConsumeMessage();
    }
}


