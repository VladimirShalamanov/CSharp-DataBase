// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.Text;
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using Utilities;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xml = new XmlHelper();
            var sb = new StringBuilder();

            var projectsDtos = xml.Deserialize<ImportProjectsDto[]>(xmlString, "Projects");

            var projects = new List<Project>();

            foreach (ImportProjectsDto pDto in projectsDtos)
            {
                if (!IsValid(pDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime pOpenDate;
                bool isValidPrOpen = DateTime.TryParseExact(pDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out pOpenDate);
                if (!isValidPrOpen)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                DateTime? pDueDate = null;
                if (!string.IsNullOrWhiteSpace(pDto.DueDate))
                {
                    DateTime dueDate;
                    bool isValidPrDue = DateTime.TryParseExact(pDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate);
                    if (!isValidPrDue)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    pDueDate = dueDate;
                }

                var p = new Project()
                {
                    Name = pDto.Name,
                    OpenDate = pOpenDate,
                    DueDate = pDueDate
                };


                foreach (ImportTaskDto tDto in pDto.Tasks)
                {
                    DateTime tOpenDate;
                    bool isValidOpenDate = DateTime.TryParseExact(tDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tOpenDate);
                    
                    DateTime tDueDate;
                    bool isValidDueDate = DateTime.TryParseExact(tDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tDueDate);

                    if (!IsValid(tDto) ||
                        !isValidOpenDate ||
                        !isValidDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (tOpenDate < pOpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (pDueDate.HasValue && tDueDate > pDueDate.Value)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    p.Tasks.Add(new Task()
                    {
                        Name = tDto.Name,
                        OpenDate = tOpenDate,
                        DueDate = tDueDate,
                        ExecutionType = (ExecutionType)tDto.ExecutionType,
                        LabelType = (LabelType)tDto.LabelType
                    });
                }

                projects.Add(p);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, p.Name, p.Tasks.Count()));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var employeesDtos = JsonConvert.DeserializeObject<ImportEmployeesDto[]>(jsonString);

            var employees = new List<Employee>();

            foreach (ImportEmployeesDto eDto in employeesDtos!)
            {
                if (!IsValid(eDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var e = new Employee()
                {
                    Username = eDto.Username,
                    Email = eDto.Email,
                    Phone = eDto.Phone
                };

                foreach (int tId in eDto.Tasks.Distinct())
                {
                    var t = context.Tasks.Find(tId);
                    if (t == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    e.EmployeesTasks.Add(new EmployeeTask() { Task = t });
                }

                employees.Add(e);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, e.Username, e.EmployeesTasks.Count()));
            }

            context.AddRange(employees);
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