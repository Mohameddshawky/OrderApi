using AutoMapper;
using Application.DTOs;
using Core.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));
        
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.OriginalItemTotal, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)))
            .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null));
            
        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForMember(dest => dest.OldStatus, opt => opt.MapFrom(src => src.OldStatus.HasValue ? src.OldStatus.Value.ToString() : null))
            .ForMember(dest => dest.NewStatus, opt => opt.MapFrom(src => src.NewStatus.ToString()));
            
        CreateMap<CreateOrderItemDto, OrderItem>();
        
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());
    }
}
