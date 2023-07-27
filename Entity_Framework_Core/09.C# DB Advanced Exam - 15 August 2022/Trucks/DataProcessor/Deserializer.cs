namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Text;
    using Newtonsoft.Json;

    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using DataProcessor.ImportDto;
    using Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlHelper = new XmlHelper();

            ImportDespatcher[] despatcherDtos = xmlHelper.Deserialize<ImportDespatcher[]>(xmlString, "Despatchers");

            var despatchers = new List<Despatcher>();

            foreach (ImportDespatcher dDto in despatcherDtos)
            {
                if (!IsValid(dDto) || string.IsNullOrEmpty(dDto.Position))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var d = new Despatcher() { Name = dDto.Name, Position = dDto.Position };

                foreach (ImportDespatcherWithTruck tDto in dDto.Trucks)
                {
                    if (!IsValid(tDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck t = new Truck()
                    {
                        RegistrationNumber = tDto.RegistrationNumber,
                        VinNumber = tDto.VinNumber,
                        TankCapacity = tDto.TankCapacity,
                        CargoCapacity = tDto.CargoCapacity,
                        CategoryType = (CategoryType)tDto.CategoryType,
                        MakeType = (MakeType)tDto.MakeType
                    };

                    d.Trucks.Add(t);
                }

                despatchers.Add(d);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, d.Name, d.Trucks.Count));
            }

            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var sb = new StringBuilder();

            ImportClient[] clientsDtos = JsonConvert.DeserializeObject<ImportClient[]>(jsonString);

            var clients = new List<Client>();

            foreach (ImportClient cDto in clientsDtos)
            {
                if (!IsValid(cDto) || cDto.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client c = new Client()
                {
                    Name = cDto.Name,
                    Nationality = cDto.Nationality,
                    Type = cDto.Type,
                };

                foreach (int tId in cDto.Trucks.Distinct())
                {
                    Truck t = context.Trucks.Find(tId);
                    if (t == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    c.ClientsTrucks.Add(new ClientTruck() { Truck = t });
                }

                clients.Add(c);
                sb.AppendLine(string.Format(SuccessfullyImportedClient, c.Name, c.ClientsTrucks.Count));
            }

            context.Clients.AddRange(clients);
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