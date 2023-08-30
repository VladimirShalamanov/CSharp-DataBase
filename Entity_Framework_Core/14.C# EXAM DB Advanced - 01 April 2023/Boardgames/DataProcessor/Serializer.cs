namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using Utilities;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var xmlHelper = new XmlHelper();

            var creators = context.Creators
                          .Where(c => c.Boardgames.Any())
                          .ToArray()
                          .Select(c => new ExportCreatorDto()
                          {
                              BoardgamesCount = c.Boardgames.Count(),
                              CreatorName = $"{c.FirstName} {c.LastName}",
                              Boardgames = c.Boardgames
                                            .ToArray()
                                            .Select(b => new ExportBoardgamesDto()
                                            {
                                                BoardgameName = b.Name,
                                                BoardgameYearPublished = b.YearPublished
                                            })
                                            .OrderBy(b => b.BoardgameName)
                                            .ToArray()
                          })
                          .OrderByDescending(c => c.Boardgames.Length)
                          .ThenBy(c => c.CreatorName)
                          .ToArray();

            return xmlHelper.Serialize(creators, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Where(s => s.BoardgamesSellers
                             .Any(bs => bs.Boardgame.YearPublished >= year &&
                                        bs.Boardgame.Rating <= rating))
                .ToArray()
                .Select(s => new
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                                  .Where(bs => bs.Boardgame.YearPublished >= year &&
                                               bs.Boardgame.Rating <= rating)
                                  .ToArray()
                                  .Select(bs => new
                                  {
                                      Name = bs.Boardgame.Name,
                                      Rating = bs.Boardgame.Rating,
                                      Mechanics = bs.Boardgame.Mechanics,
                                      Category = bs.Boardgame.CategoryType.ToString()
                                  })
                                  .OrderByDescending(bs => bs.Rating)
                                  .ThenBy(bs => bs.Name)
                                  .ToArray()
                })
                .OrderByDescending(s => s.Boardgames.Length)
                .ThenBy(s => s.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
        }
    }
}