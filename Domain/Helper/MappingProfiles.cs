using AutoMapper;
using Domain.Dto;
using Domain.Identity;
using Domain.Relations;

namespace Domain.Helper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
        
        CreateMap<User, UserForOrderDto>(); 
        CreateMap<User, UserDto>(); 

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.ProductInOrders, opt => opt.MapFrom(src => src.ProductInOrders));

        CreateMap<ProductInOrder, ProductInOrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Product.ImagePath))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        
        CreateMap<ProductInShoppingCart, ShoppingCartItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Product.ImagePath))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Product.Price));
            
        CreateMap<ShoppingCartDto, ShoppingCartForLoggedInUserDto>();

        CreateMap<ShoppingCart, ShoppingCartForLoggedInUserDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductInShoppingCarts))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.ProductInShoppingCarts.Sum(item => item.Quantity * item.Product.Price)));
    }
}