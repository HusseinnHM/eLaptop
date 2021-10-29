using AutoMapper;
using eLaptop.Models;
using eLaptop.ViewModels;
using eLaptop.ViewModels.Account;
using eLaptop.ViewModels.Home;
using eLaptop.ViewModels.Order;
using eLaptop.ViewModels.Product;

namespace eLaptop
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Company, CompanyVM>().ReverseMap();
            CreateMap<ProductType, ProductTypeVM>().ReverseMap();
            CreateMap<Product, HomeProductVM>().ReverseMap();
            CreateMap<Product, ProductIndexVM>().ReverseMap();
            CreateMap<Product, ProductUpsertVM>().ReverseMap();
            CreateMap<Product, HomeDetailsVM>().ReverseMap();
            CreateMap<Order, OrderCompleteVM>().ReverseMap();
            CreateMap<Order, OrderAllVM>().ReverseMap();
            CreateMap<OrderItem, OrderItemVM>().ReverseMap();
            CreateMap<OrderItem, ShoppingCartItem>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemVM>().ReverseMap();
            CreateMap<ShoppingCartItem, OrderItemVM>().ReverseMap();
            CreateMap<ApplicationUser, UpdateInfoVM>().ReverseMap();
            CreateMap<ApplicationUser, RegisterVM>().ReverseMap();
        }
    }
}