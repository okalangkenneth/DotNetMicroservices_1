using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Dicount.API.Services
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

        public void PublishMessage(string exchange, string routingKey, object message)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange, routingKey, null, body);
        }

        public string ConsumeMessage()
        {
            var result = _channel.BasicGet("", true);
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
