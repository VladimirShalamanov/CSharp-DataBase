namespace Theatre.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    using Data;
    using Newtonsoft.Json;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;
    using Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var minTime = new TimeSpan(1, 0, 0);

            var validGenres = new string[] { "Drama", "Comedy", "Romance", "Musical" };

            var xmlHelper = new XmlHelper();
            var sb = new StringBuilder();
            var plays = new HashSet<Play>();

            var playsDtos = xmlHelper.Deserialize<ImportPlayDto[]>(xmlString, "Plays");

            foreach (ImportPlayDto pD in playsDtos)
            {
                var dtoTime = TimeSpan.ParseExact(pD.Duration, "c", CultureInfo.InvariantCulture);

                if (!IsValid(pD) || !validGenres.Contains(pD.Genre) ||
                    (dtoTime < minTime))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                
                var p = new Play()
                {
                    Title = pD.Title,
                    Duration = TimeSpan.ParseExact(pD.Duration, "c", CultureInfo.InvariantCulture),
                    Rating = pD.Rating,
                    Genre = (Genre)Enum.Parse(typeof(Genre), pD.Genre),
                    Description = pD.Description,
                    Screenwriter = pD.Screenwriter
                };

                plays.Add(p);
                sb.AppendLine(string.Format(SuccessfulImportPlay, p.Title, p.Genre, pD.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var xmlHelper = new XmlHelper();
            var sb = new StringBuilder();
            var casts = new HashSet<Cast>();

            var castDtos = xmlHelper.Deserialize<ImportCastDto[]>(xmlString, "Casts");

            foreach (ImportCastDto cD in castDtos)
            {
                if (!IsValid(cD))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var c = new Cast()
                {
                    FullName = cD.FullName,
                    IsMainCharacter = cD.IsMainCharacter,
                    PhoneNumber = cD.PhoneNumber,
                    PlayId = cD.PlayId
                };

                casts.Add(c);
                string mainCharacter = c.IsMainCharacter ? "main" : "lesser";
                sb.AppendLine(string.Format(SuccessfulImportActor, c.FullName, mainCharacter));
            }

            context.Casts.AddRange(casts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var theatresDtos = JsonConvert.DeserializeObject<ImportTtheatersTicketsDto[]>(jsonString);

            var theatres = new HashSet<Theatre>();

            foreach (ImportTtheatersTicketsDto theaD in theatresDtos!)
            {
                if (!IsValid(theaD))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var t = new Theatre()
                {
                    Name = theaD.Name,
                    NumberOfHalls = theaD.NumberOfHalls,
                    Director = theaD.Director
                };

                var tic = new List<Ticket>();
                foreach (ImportTicketsDto ticD in theaD.Tickets)
                {
                    if (!IsValid(ticD))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    tic.Add(new Ticket()
                    {
                        Price = ticD.Price,
                        RowNumber = ticD.RowNumber,
                        PlayId = ticD.PlayId,
                        Theatre = t
                    });
                }

                t.Tickets = tic;
                theatres.Add(t);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, t.Name, tic.Count));
            }

            context.Theatres.AddRange(theatres);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
