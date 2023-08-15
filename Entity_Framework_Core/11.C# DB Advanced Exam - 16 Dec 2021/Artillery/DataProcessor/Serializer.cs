namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Artillery.Utilities;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shells = context.Shells
                .Where(s => s.ShellWeight > shellWeight)
                .ToArray()
                .Select(s => new
                {
                    s.ShellWeight,
                    s.Caliber,
                    Guns = s.Guns
                            .Where(g => g.GunType.ToString() == "AntiAircraftGun")
                            .ToArray()
                            .Select(g => new
                            {
                                GunType = g.GunType.ToString(),
                                g.GunWeight,
                                g.BarrelLength,
                                Range = g.Range > 3000 ?
                                        "Long-range" : "Regular range"
                            })
                            .OrderByDescending(g => g.GunWeight)
                            .ToArray()
                })
                .OrderBy(s => s.ShellWeight)
                .ToArray();

            return JsonConvert.SerializeObject(shells, Formatting.Indented);
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ArtilleryProfile>();
            });
            var xmlHelper = new XmlHelper();

            var guns = context.Guns
                        .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                        .ProjectTo<ExportGunDto>(config)
                        .OrderBy(g => g.BarrelLength)
                        .ToArray();

            return xmlHelper.Serialize(guns, "Guns");

        }
    }
}
