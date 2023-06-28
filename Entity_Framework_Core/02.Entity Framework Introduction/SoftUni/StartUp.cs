using Microsoft.VisualBasic;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SoftUni;

public class StartUp
{

    static void Main(string[] args)
    {
        SoftUniContext context = new SoftUniContext();

        string res = RemoveTown(context);

        Console.WriteLine(res);
    }

    // 03
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        var sb = new StringBuilder();

        var eployees = context.Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        foreach (var e in eployees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    // 04
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        var sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.Salary > 50000)
            .OrderBy(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            })
            .ToArray();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    // 05
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        var sb = new StringBuilder();

        var employeesRnD = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                DepartmentName = e.Department.Name,
                e.Salary
            })
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .ToArray();

        foreach (var e in employeesRnD)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    // 06
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address newAddress = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        Employee? employee = context.Employees
            .FirstOrDefault(e => e.LastName == "Nakov");

        employee!.Address = newAddress;
        context.SaveChanges();

        var employeeAddresses = context.Employees
            .OrderByDescending(e => e.AddressId)
            .Select(e => e.Address!.AddressText)
            .Take(10)
            .ToArray();

        return string.Join(Environment.NewLine, employeeAddresses);
    }

    // 07
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        var employees = context.Employees
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager!.FirstName,
                ManagerLastName = e.Manager!.LastName,
                Projects = e.EmployeesProjects
                    .Where(ep => ep.Project.StartDate.Year >= 2001 &&
                                ep.Project.StartDate.Year <= 2003)
                    .Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = p.Project.EndDate.HasValue ?
                                p.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                                : "not finished"
                    })
                    .ToArray()
            })
            .ToArray();

        var sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

            foreach (var p in e.Projects)
            {
                sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    // 08
    public static string GetAddressesByTown(SoftUniContext context)
    {
        var employees = context.Addresses
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town!.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .Select(e => new
            {
                e.AddressText,
                TownName = e.Town!.Name,
                EmployeeCount = e.Employees.Count
            })
            .ToArray();


        var sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.AddressText}, {e.TownName} - {e.EmployeeCount} employees");
        }

        return sb.ToString().TrimEnd();
    }

    // 09
    public static string GetEmployee147(SoftUniContext context)
    {
        var employee = context.Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects
                    .OrderBy(p => p.Project.Name)
                    .Select(p => new
                    {
                        ProjectName = p.Project.Name
                    })
                    .ToArray()
            })
            .ToArray();

        var sb = new StringBuilder();

        foreach (var e in employee)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

            foreach (var p in e.Projects)
            {
                sb.AppendLine($"{p.ProjectName}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    // 10
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                        .Select(e => new
                        {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle
                        })
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .ToArray()
                })
                .ToArray();

        var sb = new StringBuilder();

        foreach (var d in departments)
        {
            sb.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");

            foreach (var e in d.Employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    // 11
    public static string GetLatestProjects(SoftUniContext context)
    {
        var projects = context.Projects
            .OrderByDescending(p => p.StartDate)
            .Take(10)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            })
            .OrderBy(p => p.Name)
            .ToArray();

        var sb = new StringBuilder();

        foreach (var p in projects)
        {
            sb.AppendLine(p.Name);
            sb.AppendLine(p.Description);
            sb.AppendLine(p.StartDate);
        }

        return sb.ToString().TrimEnd();
    }

    // 12
    public static string IncreaseSalaries(SoftUniContext context)
    {
        string[] deps = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

        foreach (var e in context.Employees
                            .Where(d => deps.Contains(d.Department.Name))
                            .ToArray())
        {
            e.Salary *= 1.12m;
        }

        context.SaveChanges();

        var employees = context.Employees
            .Where(d => deps.Contains(d.Department.Name))
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();


        var sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
        }

        return sb.ToString().TrimEnd();
    }

    // 13
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        var employees = context.Employees
            .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        var sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
        }

        return sb.ToString().TrimEnd();
    }

    // 14
    public static string DeleteProjectById(SoftUniContext context)
    {
        var projectsToDelete = context.EmployeesProjects
            .Where(p => p.ProjectId == 2)
            .ToArray();
        context.EmployeesProjects.RemoveRange(projectsToDelete);

        var prDel = context.Projects.Find(2)!;
        context.Projects.Remove(prDel);

        context.SaveChanges();

        var projectsNames = context.Projects
            .Take(10)
            .Select(p => p.Name)
            .ToArray();

        return string.Join(Environment.NewLine, projectsNames);
    }

    // 15
    public static string RemoveTown(SoftUniContext context)
    {
        var townToDel = context.Towns
            .First(t => t.Name == "Seattle");

        var addressToDel = context.Addresses
            .Where(t => t.TownId == townToDel.TownId);

        int addressCnt = addressToDel.Count();

        var employeesToDel = context.Employees
            .Where(e => addressToDel.Any(a => a.AddressId == e.AddressId))
            .ToArray();

        foreach (var e in employeesToDel)
        {
            e.AddressId = null;
        }
        context.Addresses.RemoveRange(addressToDel);
        context.Towns.Remove(townToDel);

        context.SaveChanges();

        return $"{addressCnt} addresses in Seattle were deleted";
    }
}