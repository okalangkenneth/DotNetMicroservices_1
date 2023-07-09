using AutoMapper;
using Ordering_API.Entities;
using Ordering_API.Handlers.Commands.CheckoutOrder;
using Ordering_API.Handlers.Commands.UpdateOrder;
using Ordering_API.Handlers.Queries;

namespace Ordering_API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrdersVm>().ReverseMap();
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
        }
    }
}
