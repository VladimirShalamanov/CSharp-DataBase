namespace CarDealer;

using AutoMapper;
using CarDealer.DTOs.Export;
using Data;
using DTOs.Import;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

public class StartUp
{
    public static void Main()
    {
        CarDealerContext context = new CarDealerContext();

        //string inputJson = File.ReadAllText(@"../../../Datasets/sales.json");

        //string res = ImportSales(context, inputJson);

        string res = GetSalesWithAppliedDiscount(context);

        Console.WriteLine(res);
    }

    // 09
    public static string ImportSuppliers(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var suppliersDtos = JsonConvert.DeserializeObject<ImpoortSupplierDto[]>(inputJson);

        ICollection<Supplier> suppliers = mapper.Map<Supplier[]>(suppliersDtos);

        context.Suppliers.AddRange(suppliers);
        context.SaveChanges();

        return $"Successfully imported {suppliers.Count}.";
    }

    // 10
    public static string ImportParts(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

        ICollection<Part> validParts = new HashSet<Part>();

        foreach (var p in partsDtos!)
        {
            if (context.Suppliers.Any(s => s.Id == p.SupplierId))
            {
                //Part part = mapper.Map<Part>(p);
                validParts.Add(mapper.Map<Part>(p));
            }
        }

        context.Parts.AddRange(validParts);
        context.SaveChanges();

        return $"Successfully imported {validParts.Count}.";
    }

    // 11
    //public static string ImportCars(CarDealerContext context, string inputJson)
    //{
    //    IMapper mapper = CreateMapper();

    //    var carsDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

    //    ICollection<Car> validCars = new HashSet<Car>();

    //    foreach (var cDt in carsDtos!)
    //    {
    //        Car car = new Car()
    //        {
    //            Make = cDt.Make,
    //            Model = cDt.Model,
    //            TravelledDistance = cDt.TravelledDistance
    //        };

    //        ICollection<PartCar> currPartCar = new HashSet<PartCar>();

    //        foreach (int pId in cDt.PartsId.Distinct())
    //        {
    //            if (context.Parts.Any(p => p.Id == pId))
    //            {
    //                currPartCar.Add(new PartCar() { PartId = pId });
    //            }
    //        }

    //        car.PartsCars = currPartCar;

    //        validCars.Add(car);
    //    }

    //    context.Cars.AddRange(validCars);
    //    context.SaveChanges();

    //    return $"Successfully imported {validCars.Count}.";
    //}

    // 12
    public static string ImportCustomers(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

        ICollection<Customer> customers = mapper.Map<Customer[]>(customerDtos);

        context.Customers.AddRange(customers);
        context.SaveChanges();

        return $"Successfully imported {customers.Count}.";
    }

    // 13
    public static string ImportSales(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        var salesDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

        ICollection<Sale> sales = mapper.Map<Sale[]>(salesDtos);
        //foreach (var sD in salesDtos!)
        //{
        //    if (context.Customers.Any(c => c.Id == sD.CustomerId))
        //    {
        //        sales.Add(mapper.Map<Sale>(sD));
        //    }
        //}

        context.Sales.AddRange(sales);
        context.SaveChanges();

        return $"Successfully imported {sales.Count}.";
    }

    // 14
    public static string GetOrderedCustomers(CarDealerContext context)
    {
        var customers = context.Customers
            .OrderBy(c => c.BirthDate)
            .ThenBy(c => c.IsYoungDriver == false)
            .Select(c => new
            {
                c.Name,
                BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                c.IsYoungDriver
            })
            .ToArray();

        return JsonConvert.SerializeObject(customers, Formatting.Indented);
    }

    // 15
    //public static string GetCarsFromMakeToyota(CarDealerContext context)
    //{
    //    var carsToyota = context.Cars
    //                    .Where(c => c.Make == "Toyota")
    //                    .Select(c => new
    //                    {
    //                        c.Id,
    //                        c.Make,
    //                        c.Model,
    //                        c.TravelledDistance
    //                    })
    //                    .OrderBy(c => c.Model)
    //                    .ThenByDescending(c => c.TravelledDistance)
    //                    .AsNoTracking()
    //                    .ToArray();

    //    return JsonConvert.SerializeObject(carsToyota, Formatting.Indented);
    //}

    // 16

    public static string GetLocalSuppliers(CarDealerContext context)
    {
        var suppliers = context.Suppliers
                        .Where(s => s.IsImporter == false)
                        .Select(s => new
                        {
                            s.Id,
                            s.Name,
                            PartsCount = s.Parts.Count,
                        })
                        .AsNoTracking()
                        .ToArray();

        return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
    }

    // 17
    //public static string GetCarsWithTheirListOfParts(CarDealerContext context)
    //{
    //    var cars = context.Cars
    //                .Select(c => new
    //                {
    //                    car = new
    //                    {
    //                        c.Make,
    //                        c.Model,
    //                        c.TraveledDistance,
    //                    },
    //                    parts = c.PartsCars.Select(p => new
    //                    {
    //                        p.Part.Name,
    //                        Price = p.Part.Price.ToString("f2")
    //                    })
    //                    .ToArray()
    //                })
    //                .AsNoTracking()
    //                .ToArray();

    //    return JsonConvert.SerializeObject(cars, Formatting.Indented);
    //}

    // 18
    public static string GetTotalSalesByCustomer(CarDealerContext context)
    {
        var customers = context.Customers
                        .Include(c => c.Sales)
                        .ThenInclude(c => c.Car)
                        .ThenInclude(pc => pc.PartsCars)
                        .ThenInclude(p => p.Part)
                        .Where(c => c.Sales.Count > 0)
                        .Select(c => new
                        {
                            fullName = c.Name,
                            boughtCars = c.Sales.Count(),
                            spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
                        })
                        .OrderByDescending(c => c.spentMoney)
                        .ThenByDescending(c => c.boughtCars)
                        .ToArray();

        return JsonConvert.SerializeObject(customers,
                                           Formatting.Indented,
                                           new JsonSerializerSettings()
                                           {
                                               NullValueHandling = NullValueHandling.Ignore
                                           });
    }

    // 19
    public static string GetSalesWithAppliedDiscount(CarDealerContext context)
    {
        var car = context.Sales
            .Take(10)
            .Select(c => new
            {
                car = new
                {
                    c.Car.Make,
                    c.Car.Model,
                    c.Car.TraveledDistance
                },
                customerName = c.Customer.Name,
                discount = c.Discount.ToString("f2"),
                price = c.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                priceWithDiscount = (c.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (c.Discount * 0.01m))).ToString("f2")
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(car, Formatting.Indented);
    }


    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CarDealerProfile>();
        }));
    }
}