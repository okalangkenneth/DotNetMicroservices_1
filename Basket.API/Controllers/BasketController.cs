using Basket.API.Entities.Basket.API.Entities;
using Basket.API.Repositories;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IMessageService _messageService;

        public BasketController(IBasketRepository repository, IMessageService messageService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // Calculate the quantity change for each product in the basket
            var currentBasket = await _repository.GetBasket(basket.UserName);
            var quantityChanges = CalculateQuantityChanges(currentBasket, basket);

            // Update the basket in the repository
            var updatedBasket = await _repository.UpdateBasket(basket);

            // Publish a message to the ProductQuantityUpdated queue for each quantity change
            foreach (var change in quantityChanges)
            {
                _messageService.PublishMessage("ProductQuantityUpdated", new { ProductId = change.Key, Quantity = change.Value });
            }

            return Ok(updatedBasket);
        }

        private Dictionary<string, int> CalculateQuantityChanges(ShoppingCart currentBasket, ShoppingCart newBasket)
        {
            var quantityChanges = new Dictionary<string, int>();

            foreach (var newItem in newBasket.Items)
            {
                var currentItem = currentBasket.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);
                var quantityChange = currentItem != null ? newItem.Quantity - currentItem.Quantity : newItem.Quantity;
                quantityChanges[newItem.ProductId] = quantityChange;
            }

            return quantityChanges;
        }


        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }
}


