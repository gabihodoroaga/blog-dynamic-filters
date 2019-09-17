using AutoMapper;

namespace DynamicFilters.Models
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Order, OrderViewModel>()
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(o => o.Customer.Name))
                .ForMember(m => m.CustomerCity, opt => opt.MapFrom(o => o.Customer.City));
        }
    }
}