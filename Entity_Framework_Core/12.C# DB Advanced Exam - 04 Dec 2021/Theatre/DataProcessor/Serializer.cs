namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Xml.Linq;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.DataProcessor.ExportDto;
    using Theatre.Utilities;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres
                .ToArray()
                .Where(t => t.NumberOfHalls >= numbersOfHalls &&
                        t.Tickets.Count() >= 20)
                .Select(t => new
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                                    .Where(ti => ti.RowNumber <= 5)
                                    .Sum(ti => ti.Price),
                    Tickets = t.Tickets
                                .Where(ti => ti.RowNumber <= 5)
                                .Select(ti => new
                                {
                                    Price = ti.Price,
                                    RowNumber = ti.RowNumber
                                })
                                .OrderByDescending(ti => ti.Price)
                                .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToArray();

            return JsonConvert.SerializeObject(theatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            var xmlHelper = new XmlHelper();

            var plays = context.Plays
                .ToArray()
                .Where(p => p.Rating <= raiting)
                .Select(p => new ExportPlaysDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                            .ToArray()
                            .Where(a => a.IsMainCharacter)
                            .Select(a => new ExportActorsDto()
                            {
                                FullName = a.FullName,
                                MainCharacter = $"Plays main character in '{p.Title}'."
                            })
                            .OrderByDescending(a => a.FullName)
                            .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(t => t.Genre)
                .ToArray();

            return xmlHelper.Serialize(plays, "Plays");
        }
    }
}
