using AutoMapper;
using Basket.API.Entities;
using Basket.API.Events;

namespace Basket.API.Mappings
{
    public class BasketMappingProfile : Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
