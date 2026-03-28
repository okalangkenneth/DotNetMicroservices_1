using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Basket.API.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;

        public MessageService(IConfiguration configuration)
        {
            var host = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
            ConnectWithRetry(host);
        }

        private void ConnectWithRetry(string host)
        {
            var factory = new ConnectionFactory { HostName = host, DispatchConsumersAsync = true };
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    return;
                }
                catch (Exception)
                {
                    if (attempt == 5) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(attempt * 2));
                }
            }
        }

        public IModel Channel => _channel;

        public void PublishMessage(string routingKey, object message)
        {
            _channel.QueueDeclare(queue: routingKey,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(exchange: "",
                                  routingKey: routingKey,
                                  basicProperties: props,
                                  body: body);
        }

        public string ConsumeMessage()
        {
            var result = _channel.BasicGet("basket.checkoutqueue", true);
            return result != null ? Encoding.UTF8.GetString(result.Body.ToArray()) : null;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
