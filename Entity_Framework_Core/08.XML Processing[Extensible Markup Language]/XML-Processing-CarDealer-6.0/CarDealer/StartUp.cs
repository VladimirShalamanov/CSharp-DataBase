namespace CarDealer;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.DTOs.Export;
using Data;
using DTOs.Import;
using Models;
using System.IO;
using Utilities;

public class StartUp
{
    public static void Main()
    {
        CarDealerContext context = new CarDealerContext();
        //string inputXml = File.ReadAllText("../../../Datasets/sales.xml");

        //string res = ImportSales(context, inputXml);
        string res = GetTotalSalesByCustomer(context);

        Console.WriteLine(res);
    }

    // 09
    public static string ImportSuppliers(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateAutoMapper();

        XmlHelper xmlHelper = new XmlHelper();
        ImportSupplierDto[] supplierDtos = xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

        ICollection<Supplier> suppliers =
            new HashSet<Supplier>();
        foreach (var sD in supplierDtos)
        {
            if (!string.IsNullOrEmpty(sD.Name))
            {
                suppliers.Add(mapper.Map<Supplier>(sD));
            }
        }

        context.Suppliers.AddRange(suppliers);
        context.SaveChanges();

        return $"Successfully imported {suppliers.Count}";
    }

    // 10
    public static string ImportParts(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateAutoMapper();

        XmlHelper xmlHelper = new XmlHelper();
        ImportPartDto[] partDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");
        ICollection<int> suppIds = context.Suppliers
                                .Select(s => s.Id)
                                .ToArray();

        ICollection<Part> parts = new HashSet<Part>();
        foreach (var pD in partDtos)
        {
            if (!string.IsNullOrEmpty(pD.Name) &&
                pD.SupplierId.HasValue &&
                suppIds.Any(Id => Id == pD.SupplierId))
            {
                parts.Add(mapper.Map<Part>(pD));
            }
        }

        context.Parts.AddRange(parts);
        context.SaveChanges();

        return $"Successfully imported {parts.Count}";
    }

    // 11
    public static string ImportCars(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateAutoMapper();

        XmlHelper xmlHelper = new XmlHelper();
        ImportCarDto[] carDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");
        ICollection<int> partIds = context.Parts
                                    .Select(s => s.Id)
                                    .ToArray();

        ICollection<Car> cars = new HashSet<Car>();

        foreach (var cD in carDtos)
        {
            if (!string.IsNullOrEmpty(cD.Make) &&
                !string.IsNullOrEmpty(cD.Model))
            {
                Car car = mapper.Map<Car>(cD);

                foreach (var pD in cD.Parts.DistinctBy(p => p.PartId))
                {
                    if (partIds.Any(id => id == pD.PartId))
                    {
                        car.PartsCars.Add(new PartCar()
                        {
                            PartId = pD.PartId,
                        });
                    }
                }

                cars.Add(car);
            }
        }

        context.Cars.AddRange(cars);
        context.SaveChanges();

        return $"Successfully imported {cars.Count}";
    }

    // 12
    public static string ImportCustomers(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateAutoMapper();

        XmlHelper xmlHelper = new XmlHelper();
        ImportCustomerDto[] customerDtos = xmlHelper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

        ICollection<Customer> customers = new HashSet<Customer>();
        foreach (var cD in customerDtos)
        {
            if (!string.IsNullOrEmpty(cD.Name))
            {
                customers.Add(mapper.Map<Customer>(cD));
            }
        }

        context.Customers.AddRange(customers);
        context.SaveChanges();

        return $"Successfully imported {customers.Count}";
    }

    // 13
    public static string ImportSales(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ImportSaleDto[] saleDtos = xmlHelper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");

        ICollection<int> carsIds = context.Cars
                                    .Select(c => c.Id)
                                    .ToArray();

        ICollection<Sale> sales = new HashSet<Sale>();
        foreach (var sD in saleDtos)
        {
            if (sD.CarId.HasValue &&
                carsIds.Any(id => id == sD.CarId.Value))
            {
                sales.Add(mapper.Map<Sale>(sD));
            }
        }

        context.Sales.AddRange(sales);
        context.SaveChanges();

        return $"Successfully imported {sales.Count}";
    }

    // 14
    public static string GetCarsWithDistance(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ExportCarDto[] cars = context.Cars
            .Where(c => c.TraveledDistance > 2000000)
            .OrderBy(c => c.Make)
            .ThenBy(c => c.Model)
            .Take(10)
            .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize<ExportCarDto[]>(cars, "cars");
    }

    // 15
    public static string GetCarsFromMakeBmw(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ExportBmwCarDto[] cars = context.Cars
            .Where(c => c.Make == "BMW")
            .OrderBy(c => c.Model)
            .ThenByDescending(c => c.TraveledDistance)
            .ProjectTo<ExportBmwCarDto>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize(cars, "cars");
    }

    // 16
    public static string GetLocalSuppliers(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ExportLocalSupplierDto[] suppliers = context.Suppliers
            .Where(s => s.IsImporter == false)
            .ProjectTo<ExportLocalSupplierDto>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize(suppliers, "suppliers");
    }

    // 17
    public static string GetCarsWithTheirListOfParts(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ExportCarWithParts[] cars = context.Cars
            .OrderByDescending(c => c.TraveledDistance)
            .ThenBy(c => c.Model)
            .Take(5)
            .ProjectTo<ExportCarWithParts>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize(cars, "cars");
    }

    // 18
    public static string GetTotalSalesByCustomer(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        var tempDto = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                            ? s.Car.PartsCars.Sum(p => Math.Round((double)p.Part.Price * 0.95, 2))
                            : s.Car.PartsCars.Sum(p => (double)p.Part.Price)
                    }).ToArray(),
                })
                .ToArray();

        ExportSalesByCustomer[] totalSalesDtos = tempDto
                .OrderByDescending(t => t.SalesInfo.Sum(s => s.Prices))
                .Select(t => new ExportSalesByCustomer()
                {
                    Name = t.FullName,
                    BoughtCarsCount = t.BoughtCars,
                    SpentMoney = t.SalesInfo.Sum(s => s.Prices)
                    .ToString("f2")
                })
                .ToArray();


        return xmlHelper.Serialize(totalSalesDtos, "customers");
    }

    // 19
    public static string GetSalesWithAppliedDiscount(CarDealerContext context)
    {
        IMapper mapper = CreateAutoMapper();
        XmlHelper xmlHelper = new XmlHelper();

        var sales = context.Sales
            .ProjectTo<ExportSalesWithDiscount>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize(sales, "sales");
    }
    private static IMapper CreateAutoMapper()
    => new Mapper(new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<CarDealerProfile>();
    }));
}