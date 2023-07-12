using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public class ConsumeRabbitMQBackgroundService : BackgroundService
    {
        private readonly IMessageService _messageService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConsumeRabbitMQBackgroundService(IMessageService messageService, IServiceScopeFactory serviceScopeFactory)
        {
            _messageService = messageService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_messageService.Channel);

            consumer.Received += async (ch, ea) =>
            {
                // received message  
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IBasketRepository>();
                    await HandleMessage(content, repository);
                }
                _messageService.Channel.BasicAck(ea.DeliveryTag, false);
            };

            _messageService.Channel.BasicConsume("ProductQuantityUpdated", false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(string content, IBasketRepository repository)
        {
            // Deserialize the message
            var message = JsonConvert.DeserializeObject<ProductUpdate>(content);

            // Get the current basket
            var basket = await repository.GetBasket("defaultUser"); // replace "defaultUser" with the actual user

            // Update the product quantity in the basket
            var item = basket.Items.FirstOrDefault(i => i.ProductId == message.ProductId);
            if (item != null)
            {
                item.Quantity = message.Quantity;
                await repository.UpdateBasket(basket);
            }
        }
    }
}

