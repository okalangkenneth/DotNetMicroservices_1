using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace Basket.API.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel Channel => _channel;

        public void PublishMessage(string routingKey, object message)
        {
            _channel.QueueDeclare(queue: routingKey,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange: "",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }

        public string ConsumeMessage()
        {
            var result = _channel.BasicGet("ProductQuantityUpdated", true);
            if (result != null)
            {
                return Encoding.UTF8.GetString(result.Body.ToArray());
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
