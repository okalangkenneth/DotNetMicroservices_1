using AutoMapper;
using Basket.API.Entities;
using Basket.API.Events;
using Basket.API.Repositories;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IMapper _mapper;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            IBasketRepository repository,
            IMessageService messageService,
            IMapper mapper,
            ILogger<BasketController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // Fetch the current basket from Redis
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
            {
                _logger.LogError("Basket for user {UserName} not found.", basketCheckout.UserName);
                return BadRequest($"No basket found for user '{basketCheckout.UserName}'.");
            }

            // Map checkout request -> event, stamping in the Redis basket total
            var checkoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            checkoutEvent.TotalPrice = basket.TotalPrice;

            // Publish to RabbitMQ — Ordering.API is listening on this queue
            _messageService.PublishMessage("basket.checkoutqueue", checkoutEvent);
            _logger.LogInformation("BasketCheckout event published for user {UserName}, total {Total}.",
                basketCheckout.UserName, checkoutEvent.TotalPrice);

            // Clear the basket now that checkout is in flight
            await _repository.DeleteBasket(basketCheckout.UserName);

            return Accepted();
        }
    }
}
