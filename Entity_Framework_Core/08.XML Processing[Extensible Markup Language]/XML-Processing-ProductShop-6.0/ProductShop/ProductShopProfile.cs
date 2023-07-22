namespace ProductShop;

using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using static ProductShop.DTOs.Export._8_Exports;

public class ProductShopProfile : Profile
{
    public ProductShopProfile()
    {
        // user
        this.CreateMap<ImportUserDto, User>();

        this.CreateMap<User, _8_Exports.ExportUserCount>();

        this.CreateMap<User, _8_Exports.UserInfo>();

        // prod
        this.CreateMap<ImportProductDto, Product>();

        this.CreateMap<Product, ExportProductsInRange>()
            .ForMember(d => d.BuyerFullName,
                       op => op.MapFrom(s => s.Buyer!.FirstName + " " + s.Buyer.LastName));

        this.CreateMap<Product, ExportSoldProductsArray>();

        this.CreateMap<Product, _8_Exports.SoldProduct>();

        // cate
        this.CreateMap<ImportCategorieDto, Category>();

        this.CreateMap<Category, ExportCategoriesByProducts>()
            .ForMember(d => d.CountOfProducts,
                       op => op.MapFrom(s => s.CategoryProducts.Count))
            .ForMember(d => d.AveragePrice,
                       op => op.MapFrom(s => s.CategoryProducts
                                              .Select(cp => cp.Product)
                                              .Average(cp => cp.Price)))
            .ForMember(d => d.TotalRevenue,
                       op => op.MapFrom(s => s.CategoryProducts
                                              .Select(cp => cp.Product)
                                              .Sum(cp => cp.Price)));

        // cate-prod
        this.CreateMap<ImportCategoryProductDto, CategoryProduct>();
    }
}
