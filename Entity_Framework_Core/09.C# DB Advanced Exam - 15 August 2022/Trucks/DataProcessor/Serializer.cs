namespace Trucks.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;
    using Utilities;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TrucksProfile>();
            });

            var xmlHelper = new XmlHelper();

            var despatchers = context.Despatchers
                .Where(d => d.Trucks.Any())
                .ProjectTo<ExportDespatcher>(mapperConfig)
                .OrderByDescending(d => d.TrucksCount)
                .ThenBy(d => d.Name)
                .ToArray();

            return xmlHelper.Serialize(despatchers, "Despatchers");
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(c => c.ClientsTrucks.Any(t => t.Truck.TankCapacity >= capacity))
                .ToArray()
                .Select(c => new
                {
                    c.Name,
                    Trucks = c.ClientsTrucks
                            .Where(ct => ct.Truck.TankCapacity >= capacity)
                            .ToArray()
                            .Select(ct => new
                            {
                                TruckRegistrationNumber = ct.Truck.RegistrationNumber,
                                VinNumber = ct.Truck.VinNumber,
                                TankCapacity = ct.Truck.TankCapacity,
                                CargoCapacity = ct.Truck.CargoCapacity,
                                CategoryType = ct.Truck.CategoryType.ToString(),
                                MakeType = ct.Truck.MakeType.ToString(),
                            })
                            .OrderBy(ct => ct.MakeType)
                            .ThenByDescending(ct => ct.CargoCapacity)
                            .ToArray()
                })
                .OrderByDescending(c => c.Trucks.Length)
                .ThenBy(c => c.Name)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);

        }
    }
}
