using HR.Context;
using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly HrContext _context;

        JsonSerializerOptions options = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        public EmployeeController(HrContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await GetEmployeesWithFilters(null, null, 0);
                
            return View(employees);
        }

        public async Task<IActionResult> GetReport (DateTime? dateStart, DateTime? dateEnd, int typeReport)
        {
            List<Employee> employees = await GetEmployeesWithFilters(dateStart, dateEnd, typeReport);

            return Json(employees, options);

        }

        private async Task<List<Employee>> GetEmployeesWithFilters (DateTime? dateStart, DateTime? dateEnd, int typeReport)
        {
            if (dateStart == null)
            {
                dateStart = new DateTime(1900, 01, 01);
            }
            if (dateEnd == null)
            {
                dateEnd = new DateTime(2999, 01, 01);
            }
            List<Employee> employees = new List<Employee>();
            var groupEmplByPersonId = _context.Employees.AsNoTracking()
                    .Include(p => p.Position)
                    .Include(p => p.Person)
                    .GroupBy(e => e.PersonId);
            switch (typeReport)
            {
                case 1:
                    foreach (var eg in groupEmplByPersonId)
                    {
                        if (eg.Count() == 1 && eg.First().DateAdmissionTransfer >= dateStart
                            && eg.First().DateAdmissionTransfer <= dateEnd)
                            employees.Add(eg.First());
                        else if (eg.Count() > 1)
                        {
                            var tempOrderEmp = eg.OrderBy(e => e.DateAdmissionTransfer).ToList();
                            var lastEmpl = tempOrderEmp.First();
                            if (lastEmpl.DateAdmissionTransfer >= dateStart
                                    && lastEmpl.DateAdmissionTransfer <= dateEnd)
                                employees.Add(lastEmpl);

                            for (int i = 1; i < tempOrderEmp.Count(); i++)
                            {
                                if ((tempOrderEmp[i].DateAdmissionTransfer.Date
                                    - lastEmpl.DateDismissalTransfer.Date).TotalDays != 1
                                    && lastEmpl.DateAdmissionTransfer >= dateStart
                                    && lastEmpl.DateAdmissionTransfer <= dateEnd)
                                    employees.Add(tempOrderEmp[i]);

                                lastEmpl = tempOrderEmp[i];
                            }
                        }
                    }
                    break;
                case 2:
                    foreach (var eg in groupEmplByPersonId)
                    {
                        if (eg.Count() > 1)
                        {
                            var tempOrderEmp = eg.OrderBy(e => e.DateAdmissionTransfer).ToList();
                            var lastEmpl = tempOrderEmp.First();
                            for (int i = 1; i < tempOrderEmp.Count(); i++)
                            {
                                if ((tempOrderEmp[i].DateAdmissionTransfer.Date
                                    - lastEmpl.DateDismissalTransfer.Date).TotalDays == 1
                                    && lastEmpl.DateAdmissionTransfer >= dateStart
                                    && lastEmpl.DateAdmissionTransfer <= dateEnd)
                                    employees.Add(tempOrderEmp[i]);

                                lastEmpl = tempOrderEmp[i];
                            }
                        }
                    }
                    break;
                case 3:
                    foreach (var eg in groupEmplByPersonId)
                    {
                        if (eg.Count() == 1 && eg.First().DateDismissalTransfer >= dateStart
                            && eg.First().DateDismissalTransfer <= dateEnd)
                            employees.Add(eg.First());
                        else if (eg.Count() > 1)
                        {
                            var tempOrderEmp = eg.OrderBy(e => e.DateDismissalTransfer).ToList();
                            var lastEmpl = tempOrderEmp.First();
                            for (int i = 1; i < tempOrderEmp.Count(); i++)
                            {
                                if ((tempOrderEmp[i].DateDismissalTransfer.Date
                                    - lastEmpl.DateAdmissionTransfer.Date).TotalDays != -1
                                    && lastEmpl.DateDismissalTransfer >= dateStart
                                    && lastEmpl.DateDismissalTransfer <= dateEnd)
                                    employees.Add(lastEmpl);

                                lastEmpl = tempOrderEmp[i];
                            }
                        }
                        else if (eg.Last().DateDismissalTransfer >= dateStart
                            && eg.Last().DateDismissalTransfer <= dateEnd)
                        {
                            employees.Add(eg.Last());
                        }
                    }
                    break;
                default:
                    employees = await _context.Employees.AsNoTracking()
                    .Include(p => p.Position)
                    .Include(p => p.Person)
                    .ToListAsync();
                    break;
            }
            return employees;
        }

        public async Task<IActionResult> CreateOrEditEmployee(int id)
        {
            List<Person> persons = await _context.Persons
                .ToListAsync();
            ViewBag.Persons = new SelectList(persons, "Id", "FullName");
            List<int> ints = await _context.Employees.Where(e => e.DateDismissalTransfer >= DateTime.Now && e.Id != id)
                .Select(e => e.PositionId).ToListAsync();
            List<Position> positions = await _context.Positions
                .Where(p => !ints.Contains(p.Id))
                .ToListAsync();
            ViewBag.Positions = new SelectList(positions, "Id", "NamePosition");
            Employee employee = new Employee();
            employee.DateAdmissionTransfer = DateTime.Now;
            if (id != 0)
            {
                employee = await _context.Employees.Where(e => e.Id == id)
                .FirstOrDefaultAsync();
            }
            return View("CreateOrEditEmployee", employee);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEditEmployee(Employee employee)
        {
            try
            {
                if (employee?.Id != 0)
                {
                    Employee findEmployeeInDb = await _context.Employees.AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == employee.Id);
                    if (findEmployeeInDb != null)
                    {
                        employee.ModifiedDateRecord = DateTime.Now;
                        _context.Employees.Update(employee);
                    }
                    else
                    {
                        return RedirectToAction("~/Views/Shared/Error.cshtml");
                    }
                }
                else
                {
                    employee.DateCreationRecord = DateTime.Now;
                    await _context.Employees.AddAsync(employee);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("~/Views/Shared/Error.cshtml");
            }

        }
    }
}
