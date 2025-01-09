using AutoMapper;
using order_orderline.Application.DTOs;
using order_orderline.Domain.Entites;

namespace order_orderline.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderLine, OrderLineDto>().ReverseMap().ForMember(dest => dest.OrderLineId, opt => opt.Ignore()); // Prevent mapping of primary key;
        }
    }
}
