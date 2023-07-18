namespace ProductShop;

using AutoMapper;
using Data;
using Newtonsoft.Json;
using DTOs.Import;
using Models;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Linq;

public class StartUp
{
    public static void Main()
    {
        ProductShopContext context = new ProductShopContext();
        //string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");

        //string res = ImportCategoryProducts(context, inputJson);
        string res = GetCategoriesByProductsCount(context);
        Console.WriteLine(res);
    }

    // 01
    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

        ICollection<User> validUsers = new HashSet<User>();

        foreach (var u in userDtos!)
        {
            User user = mapper.Map<User>(u);
            validUsers.Add(user);
        }

        context.Users.AddRange(validUsers);
        context.SaveChanges();

        return $"Successfully imported {validUsers.Count}";
    }

    // 02
    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

        var products = mapper.Map<Product[]>(productDtos);

        context.Products.AddRange(products);
        context.SaveChanges();

        return $"Successfully imported {products.Length}";
    }

    // 03
    public static string ImportCategories(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var categoryDtos =
            JsonConvert.DeserializeObject<ImportCategoryGto[]>(inputJson);

        ICollection<Category> validCategories = new HashSet<Category>();

        foreach (var cDto in categoryDtos!)
        {
            if (!string.IsNullOrEmpty(cDto.Name))
            {
                Category category = mapper.Map<Category>(cDto);
                validCategories.Add(category);
            }
        }

        context.Categories.AddRange(validCategories);
        context.SaveChanges();

        return $"Successfully imported {validCategories.Count}";
    }

    // 04
    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryProductDto[] cpDtos =
            JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

        ICollection<CategoryProduct> validEntries = new HashSet<CategoryProduct>();

        foreach (ImportCategoryProductDto cp in cpDtos!)
        {
            if (!context.Categories.Any(c => c.Id == cp.CategoryId) ||
                !context.Products.Any(p => p.Id == cp.ProductId))
            {
                continue;
            }
            CategoryProduct catPro = mapper.Map<CategoryProduct>(cp);
            validEntries.Add(catPro);
        }

        context.CategoriesProducts.AddRange(validEntries);
        context.SaveChanges();

        return $"Successfully imported {validEntries.Count}";
    }

    // 05
    public static string GetProductsInRange(ProductShopContext context)
    {
        var products = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .OrderBy(p => p.Price)
            .Select(p => new
            {
                name = p.Name,
                price = p.Price,
                seller = p.Seller.FirstName + " " + p.Seller.LastName
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(products, Formatting.Indented);
    }

    // 06
    public static string GetSoldProducts(ProductShopContext context)
    {
        var usersWithSoldProducts = context.Users
            .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                soldProducts = u.ProductsSold
                    .Where(p => p.Buyer != null)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    buyerFirstName = p.Buyer.FirstName,
                    buyerLastName = p.Buyer.LastName
                })
                    .ToArray()
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented);
    }

    // 07
    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var categories = context.Categories
            .OrderByDescending(c => c.CategoriesProducts.Count)
            .Select(c => new
            {
                category = c.Name,
                productsCount = c.CategoriesProducts.Count,
                averagePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("f2"),
                totalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price).ToString("f2")
            })
            .ToArray();

        return JsonConvert.SerializeObject(categories, Formatting.Indented);
    }

    // 08
    public static string GetUsersWithProducts(ProductShopContext context)
    {
        var users = context.Users
            .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            .Select(u => new
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                age = u.Age,
                soldProducts = new
                {
                    count = u.ProductsSold.Count(p => p.Buyer != null),
                    products = u.ProductsSold
                                .Where(p => p.Buyer != null)
                                .Select(p => new
                                {
                                    name = p.Name,
                                    price = p.Price
                                })
                                .ToArray()
                }
            })
            .OrderByDescending(u => u.soldProducts.count)
            .AsNoTracking()
            .ToArray();

        var userWrapperDto = new
        {
            usersCount = users.Length,
            users = users
        };

        return JsonConvert.SerializeObject(userWrapperDto,
            Formatting.Indented,
            new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
    }

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));
    }
}