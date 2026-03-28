using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ordering_API.Events;
using Ordering_API.Handlers.Commands.CheckoutOrder;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering_API.EventConsumer
{
    public class BasketCheckoutConsumer : BackgroundService
    {
        private readonly ILogger<BasketCheckoutConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "basket.checkoutqueue";

        public BasketCheckoutConsumer(
            ILogger<BasketCheckoutConsumer> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            ConnectWithRetry(configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost");
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
                    _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("RabbitMQ connect attempt {Attempt} failed: {Msg}", attempt, ex.Message);
                    if (attempt == 5) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(attempt * 2));
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Received BasketCheckout event: {Body}", body);

                var checkoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(body);
                await HandleCheckoutEvent(checkoutEvent);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task HandleCheckoutEvent(BasketCheckoutEvent evt)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new CheckoutOrderCommand
            {
                UserName = evt.UserName,
                TotalPrice = evt.TotalPrice,
                FirstName = evt.FirstName,
                LastName = evt.LastName,
                EmailAddress = evt.EmailAddress,
                AddressLine = evt.AddressLine,
                Country = evt.Country,
                State = evt.State,
                ZipCode = evt.ZipCode,
                CardName = evt.CardName,
                CardNumber = evt.CardNumber,
                Expiration = evt.Expiration,
                CVV = evt.CVV,
                PaymentMethod = evt.PaymentMethod
            };

            var orderId = await mediator.Send(command);
            _logger.LogInformation("Order {OrderId} created for user {UserName}.", orderId, evt.UserName);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
