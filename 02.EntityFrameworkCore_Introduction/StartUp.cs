using System.Globalization;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using SoftUniContext dbContext = new SoftUniContext();
            dbContext.Database.EnsureCreated();

            string result = RemoveTown(dbContext);
            Console.WriteLine(result);
        }

        // Problem 03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context
                .Employees
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

            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }
        // Problem 04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb= new StringBuilder();
            var employees = context
                .Employees
                .Where(e=> e.Salary>50000)
                .Select(e=> new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e=>e.FirstName)
                .ToArray();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary.ToString("F2")}");

            }
            return sb.ToString().TrimEnd();
                


        }
        // Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesRnD = context
                .Employees
                .Where(e => e.Department.Name.Equals("Research and Development"))
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
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} from Research and Development - ${e.Salary.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }
        // Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            const string newAddressText = "Vitoshka 15";
            const int newAddressTownId = 4;
            
            Address newAddress = new Address()
            {
                AddressText = newAddressText,
                TownId = newAddressTownId
            };

            Employee nakovEmployee = context
                .Employees
                .First(e => e.LastName.Equals("Nakov"));
            nakovEmployee.Address=newAddress;
           
            context.SaveChanges();

            string?[] addresses=context
                .Employees
                .Where(e=>e.AddressId.HasValue)
                .OrderByDescending(e=>e.AddressId)
                .Select(e=>e.Address!.AddressText)
                .Take(10)
                .ToArray();

            return String.Join(Environment.NewLine, addresses);

        }

        // Problem 07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesWithProjects= context
                .Employees
                .Select(e=> new
                {
                    FirstName=e.FirstName,
                    LastName=e.LastName,
                    ManagerFirstName=e.Manager==null ? null : e.Manager.FirstName,
                    ManagerLastName=e.Manager== null ? null : e.Manager.LastName,
                    Projects=e.EmployeesProjects
                        .Select(ep=>ep.Project)
                        .Where(p=> p.StartDate.Year>=2001 &&
                                   p.StartDate.Year<=2003)
                        .Select(p=> new
                        {
                            ProjectName=p.Name,
                            p.StartDate,
                            p.EndDate
                        })
                        .ToArray()
                })
                .Take(10)
                .ToArray();
            foreach (var e in employeesWithProjects)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {
                    string startDateFormatted = p.StartDate
                        .ToString("M/d/yyyy h:mm:ss tt");
                    string endDateFormatted = p.EndDate.HasValue ?
                        p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";
                    sb.AppendLine($"--{p.ProjectName} - {startDateFormatted} - {endDateFormatted}");
                }
            }
            return sb.ToString().TrimEnd();
        }
        // Problem 08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            string[] addressesInfo = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => $"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees")
                .ToArray();

            return string.Join(Environment.NewLine, addressesInfo);
        }
        // Problem 09
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147Info = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.Select(p => new { p.Project.Name }).OrderBy(p => p.Name).ToArray()
                })
                .FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee147Info.FirstName} {employee147Info.LastName} - {employee147Info.JobTitle}");
            sb.Append(string.Join(Environment.NewLine, employee147Info.Projects.Select(p => p.Name)));

            return sb.ToString().TrimEnd();

        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departmentsInfo = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerName = d.Manager.FirstName + " " + d.Manager.LastName,
                    Employees = d.Employees
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .Select(e => new
                        {
                            EmployeeData = $"{e.FirstName} {e.LastName} - {e.JobTitle}"
                        })
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var d in departmentsInfo)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerName}");
                sb.Append(string.Join(Environment.NewLine, d.Employees.Select(e => e.EmployeeData)));
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projectsInfo = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var p in projectsInfo)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal salaryModifier = 1.12m;
            string[] departmentNames = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employeesForSalaryIncrease = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .ToArray();

            foreach (var e in employeesForSalaryIncrease)
            {
                e.Salary *= salaryModifier;
            }

            context.SaveChanges();

            string[] emplyeesInfoText = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, emplyeesInfoText);
        }
        //Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            string[] employeesInfoText = context.Employees
                .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, employeesInfoText);
        }
        // Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            const int deleteProjectId = 2; 

            IEnumerable<EmployeeProject> employeeProjectsDelete = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == deleteProjectId)
                .ToArray();
            context.EmployeesProjects.RemoveRange(employeeProjectsDelete);

            Project? deleteProject = context
                .Projects
                .Find(deleteProjectId);
            if (deleteProject != null)
            {
                context.Projects.Remove(deleteProject);
            }

            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Select(p => p.Name)
                .Take(10)
                .ToArray();

            return String.Join(Environment.NewLine, projectNames);
        }
        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault();

            Address[] addressesToDelete = context.Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToArray();

            Employee[] employeesToRemoveAddressFrom = context.Employees
                .Where(e => addressesToDelete
                    .Contains(e.Address))
                .ToArray();

            foreach (Employee e in employeesToRemoveAddressFrom)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{addressesToDelete.Count()} addresses in Seattle were deleted";
        }

        private static void RestoreDatabase(SoftUniContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
