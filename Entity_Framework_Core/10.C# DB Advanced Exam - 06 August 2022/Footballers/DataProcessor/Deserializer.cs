namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            var xmlHelper = new XmlHelper();
            var sb = new StringBuilder();

            var coachesDtos = xmlHelper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            ICollection<Coach> coaches = new List<Coach>();

            foreach (ImportCoachDto cD in coachesDtos)
            {
                if (!IsValid(cD) || string.IsNullOrEmpty(cD.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var c = new Coach() { Name = cD.Name, Nationality = cD.Nationality };

                foreach (ImportFootballerDto fD in cD.Footballers)
                {
                    if (!IsValid(fD))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime StartDate;
                    DateTime EndDate;

                    bool isValidDateStart = DateTime.TryParseExact(fD.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                                    DateTimeStyles.None, out StartDate);
                    bool isValidDateEnd = DateTime.TryParseExact(fD.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                                    DateTimeStyles.None, out EndDate);

                    if (!isValidDateStart || !isValidDateEnd)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (StartDate >= EndDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var f = new Footballer()
                    {
                        Name = fD.Name,
                        ContractStartDate = StartDate,
                        ContractEndDate = EndDate,
                        BestSkillType = (BestSkillType)fD.BestSkillType,
                        PositionType = (PositionType)fD.PositionType
                    };

                    c.Footballers.Add(f);
                }

                coaches.Add(c);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, c.Name, c.Footballers.Count));
            }

            context.Coaches.AddRange(coaches);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var teamsDtos = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            ICollection<Team> teams = new List<Team>();

            foreach (ImportTeamDto tD in teamsDtos!)
            {
                if (!IsValid(tD) || string.IsNullOrEmpty(tD.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var t = new Team()
                {
                    Name = tD.Name,
                    Nationality = tD.Nationality,
                    Trophies = tD.Trophies
                };

                if (t.Trophies == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                foreach (int tf in tD.Footballers.Distinct())
                {
                    Footballer f = context.Footballers.Find(tf)!;
                    if (f == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    t.TeamsFootballers.Add(new TeamFootballer() { Footballer = f });
                }

                teams.Add(t);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, t.Name, t.TeamsFootballers.Count));
            }

            context.Teams.AddRange(teams);
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
