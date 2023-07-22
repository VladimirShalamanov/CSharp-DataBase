namespace ProductShop;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Utilities;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using static ProductShop.DTOs.Export._8_Exports;

public class StartUp
{
    public static void Main()
    {
        ProductShopContext context = new ProductShopContext();

        //string inputXml = File.ReadAllText(@"../../../Datasets/categories-products.xml");
        //string res = ImportCategoryProducts(context, inputXml);

        string res = GetUsersWithProducts(context);

        Console.WriteLine(res);
    }

    // 01
    public static string ImportUsers(ProductShopContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();
        XmlHelper xmlHelper = new XmlHelper();

        var usersDto = xmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

        ICollection<User> users = new List<User>();
        foreach (var u in usersDto)
        {
            if (!string.IsNullOrEmpty(u.FirstName) ||
                !string.IsNullOrEmpty(u.LastName))
            {
                users.Add(mapper.Map<User>(u));
            }
        }

        context.Users.AddRange(users);
        context.SaveChanges();

        return $"Successfully imported {users.Count}";
    }

    // 02
    public static string ImportProducts(ProductShopContext context, string inputXml)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var prodictsDto = xmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

        ICollection<Product> products = mapper.Map<Product[]>(prodictsDto);

        context.Products.AddRange(products);
        context.SaveChanges();

        return $"Successfully imported {products.Count}";
    }

    // 03
    public static string ImportCategories(ProductShopContext context, string inputXml)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var categoriesDto = xmlHelper.Deserialize<ImportCategorieDto[]>(inputXml, "Categories");

        ICollection<Category> categories = new List<Category>();

        foreach (var c in categoriesDto)
        {
            if (!string.IsNullOrEmpty(c.Name))
            {
                categories.Add(mapper.Map<Category>(c));
            }
        }

        context.Categories.AddRange(categories);
        context.SaveChanges();

        return $"Successfully imported {categories.Count}";
    }

    // 04
    public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var categoriesProductsDto = xmlHelper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");
        var cnxCat = context.Categories
                     .Select(c => c.Id)
                     .ToArray();
        var cnxPro = context.Products
                     .Select(p => p.Id)
                     .ToArray();

        ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();

        foreach (var cp in categoriesProductsDto)
        {
            if (cnxCat.Any(id => id == cp.CategoryId) &&
                cnxPro.Any(id => id == cp.ProductId))
            {
                categoryProducts.Add(mapper.Map<CategoryProduct>(cp));
            }
        }

        context.CategoryProducts.AddRange(categoryProducts);
        context.SaveChanges();

        return $"Successfully imported {categoryProducts.Count}";
    }

    // 05
    public static string GetProductsInRange(ProductShopContext context)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var products = context.Products
            .Where(p => p.Price > 500 && p.Price < 1000)
            .OrderBy(p => p.Price)
            .Take(10)
            .ProjectTo<ExportProductsInRange>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize(products, "Products");
    }

    // 06
    public static string GetSoldProducts(ProductShopContext context)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var users = context.Users
                    .Where(u => u.ProductsSold.Any())
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Take(5)
                    .ProjectTo<ExportSoldProductsArray>(mapper.ConfigurationProvider)
                    .ToArray();

        return xmlHelper.Serialize(users, "Users");
    }

    // 07
    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var categories = context.Categories
                         .ProjectTo<ExportCategoriesByProducts>(mapper.ConfigurationProvider)
                         .OrderByDescending(c => c.CountOfProducts)
                         .ThenBy(c => c.TotalRevenue)
                         .ToArray();

        return xmlHelper.Serialize(categories, "Categories");
    }

    // 08
    public static string GetUsersWithProducts(ProductShopContext context)
    {
        var mapper = CreateMapper();
        var xmlHelper = new XmlHelper();

        var users = context.Users
                    .Where(u => u.ProductsSold.Any())
                    .OrderByDescending(u => u.ProductsSold.Count())
                    .Select(u => new UserInfo()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SouldProducts = new SoldProductsCount()
                        {
                            Count = u.ProductsSold.Count,
                            Products = u.ProductsSold.Select(ps => new SoldProduct()
                            {
                                Name = ps.Name,
                                Price = ps.Price
                            })
                            .OrderByDescending(ps => ps.Price)
                            .ToArray()
                        }
                    })
                    .Take(10)
                    .AsNoTracking()
                    .ToArray();

        var userCountDto = new ExportUserCount()
        {
            Count = context.Users.Count(u => u.ProductsSold.Any()),
            Users = users
        };

        return xmlHelper.Serialize<ExportUserCount>(userCountDto, "Users");
    }
    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));
    }
}