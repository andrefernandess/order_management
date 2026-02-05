using AutoMapper;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer mappings
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<Customer, CustomerDetailDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.RecentOrders, opt => opt.MapFrom(src => 
                src.Orders.OrderByDescending(o => o.CreatedAt).Take(5)));

        CreateMap<Address, AddressDto>();

        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductDetailDto>();

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Total.Currency))
            .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount));

        CreateMap<Order, OrderDetailDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal.Amount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
            .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => src.ShippingCost.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Total.Currency));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount));
    }
}
