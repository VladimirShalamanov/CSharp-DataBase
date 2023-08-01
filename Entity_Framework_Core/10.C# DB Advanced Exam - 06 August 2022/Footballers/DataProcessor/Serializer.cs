namespace Footballers.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ExportDto;
    using Footballers.Utilities;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text.Json.Nodes;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FootballersProfile>();
            });
            //var mapper = new Mapper(config);
            var xmlHelper = new XmlHelper();

            var coaches = context.Coaches
                .Where(c => c.Footballers.Any())
                .ProjectTo<ExportCoachDto>(config)
                .OrderByDescending(c => c.FootballersCount)
                .ThenBy(c => c.Name)
                .ToArray();

            return xmlHelper.Serialize(coaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {

            var teams = context.Teams
                .Where(t => t.TeamsFootballers
                            .Any(f => f.Footballer.ContractStartDate >= date))
                .ToArray()
                .Select(t => new
                {
                    Name = t.Name,
                    Footballers = t.TeamsFootballers
                            .Where(f => f.Footballer.ContractStartDate >= date)
                            .ToArray()
                            .OrderByDescending(f => f.Footballer.ContractEndDate)
                            .ThenBy(f => f.Footballer.Name)
                            .Select(f => new
                            {
                                FootballerName = f.Footballer.Name,
                                ContractStartDate = f.Footballer.ContractStartDate
                                                .ToString("d", CultureInfo.InvariantCulture),
                                ContractEndDate = f.Footballer.ContractEndDate
                                                .ToString("d", CultureInfo.InvariantCulture),
                                BestSkillType = f.Footballer.BestSkillType.ToString(),
                                PositionType = f.Footballer.PositionType.ToString()
                            })
                            .ToArray()
                })
                .OrderByDescending(t => t.Footballers.Length)
                .ThenBy(t => t.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(teams, Formatting.Indented);
        }
    }
}
