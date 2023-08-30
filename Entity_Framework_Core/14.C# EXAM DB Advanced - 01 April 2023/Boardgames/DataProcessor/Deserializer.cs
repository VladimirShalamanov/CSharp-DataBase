namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Utilities;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var xmlHelper = new XmlHelper();
            var sb = new StringBuilder();

            var creatorsDtos = xmlHelper.Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            ICollection<Creator> creators = new HashSet<Creator>();

            foreach (ImportCreatorDto cDto in creatorsDtos)
            {
                if (!IsValid(cDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var c = new Creator()
                {
                    FirstName = cDto.FirstName,
                    LastName = cDto.LastName
                };

                foreach (var bDto in cDto.Boardgames)
                {
                    if (!IsValid(bDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    c.Boardgames.Add(new Boardgame()
                    {
                        Name = bDto.Name,
                        Rating = bDto.Rating,
                        YearPublished = bDto.YearPublished,
                        CategoryType = (CategoryType)bDto.CategoryType,
                        Mechanics = bDto.Mechanics
                    });
                }

                creators.Add(c);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, c.FirstName, c.LastName, c.Boardgames.Count()));
            }

            context.AddRange(creators);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var sellersDtos = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            ICollection<Seller> sellers = new HashSet<Seller>();

            foreach (ImportSellerDto sDto in sellersDtos!)
            {
                if (!IsValid(sDto) || string.IsNullOrEmpty(sDto.Website))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var s = new Seller()
                {
                    Name = sDto.Name,
                    Address = sDto.Address,
                    Country = sDto.Country,
                    Website = sDto.Website
                };

                foreach (int idDto in sDto.Boardgames.Distinct())
                {
                    Boardgame boardgame = context.Boardgames.Find(idDto);
                    if (boardgame == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    s.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        Boardgame = boardgame
                    });
                }

                sellers.Add(s);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, s.Name, s.BoardgamesSellers.Count()));
            }

            context.Sellers.AddRange(sellers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
